using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager I { get; private set; }

    [Header("Tilemaps")]
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Tilemap targetTilemap;

    // occupancy map: which GameObject occupies a cell (block, player, ghost)
    private Dictionary<Vector3Int, GameObject> occupancy = new Dictionary<Vector3Int, GameObject>();

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
    }

    // Convert world position to cell and back helpers
    public Vector3Int WorldToCell(Vector3 worldPos) => floorTilemap.WorldToCell(worldPos);
    public Vector3 CellToWorldCenter(Vector3Int cell) => floorTilemap.GetCellCenterWorld(cell);

    public bool HasWall(Vector3Int cell) => wallTilemap.HasTile(cell);
    public bool HasFloor(Vector3Int cell) => floorTilemap.HasTile(cell) || targetTilemap.HasTile(cell);

    public bool IsOccupied(Vector3Int cell) => occupancy.ContainsKey(cell);
    public GameObject GetOccupant(Vector3Int cell)
    {
        occupancy.TryGetValue(cell, out var go);
        return go;
    }

    public int SetOccupant(Vector3Int cell, GameObject go)
    {
        if (go == null) { occupancy.Remove(cell); return 1; }
        if (occupancy.ContainsKey(cell)){
            if (occupancy[cell].CompareTag("Ghost") && go.CompareTag("Player")){
                Debug.Log("Player ran into ghost");
                return 0;
            }
            if (occupancy[cell].CompareTag("Player") && go.CompareTag("Ghost")){
                Debug.Log("Ghost ran into player");
                return 0;
            }
        }
        occupancy[cell] = go;
        return 1;
    }

    public void RemoveOccupant(Vector3Int cell)
    {
        if (occupancy.ContainsKey(cell)) occupancy.Remove(cell);
    }

    // Walkable for pathfinding: floor (or target) and not wall and not occupied by a blocking object.
    public bool IsWalkable(Vector3Int cell)
    {
        if (HasWall(cell)) return false;
        if (!HasFloor(cell)) return false;
        // For pathfinding ghosts should treat blocks as obstacles (they block)
        if (IsOccupied(cell))
        {
            var go = GetOccupant(cell);
            // Allow ghost to walk on target or player (player is endpoint). If occupant is block -> blocked.
            if (go != null && go.CompareTag("Block")) return false;
        }
        return true;
    }

    // neighbors (4-dir)
    public IEnumerable<Vector3Int> GetNeighbors(Vector3Int cell)
    {
        yield return cell + new Vector3Int(1, 0, 0);
        yield return cell + new Vector3Int(-1, 0, 0);
        yield return cell + new Vector3Int(0, 1, 0);
        yield return cell + new Vector3Int(0, -1, 0);
    }

    // Simple A* (returns null if no path). Uses Manhattan distance.
    public List<Vector3Int> FindPath(Vector3Int start, Vector3Int goal)
    {
        if (!IsWalkable(goal) && !(GetOccupant(goal) != null && GetOccupant(goal).CompareTag("Player")))
        {
            // goal must be reachable or be the player (player allowed as endpoint)
            return null;
        }

        var open = new List<Node>();
        var cameFrom = new Dictionary<Vector3Int, Vector3Int>();
        var gScore = new Dictionary<Vector3Int, int>();
        var fScore = new Dictionary<Vector3Int, int>();

        Func<Vector3Int, int> H = (pos) => Mathf.Abs(pos.x - goal.x) + Mathf.Abs(pos.y - goal.y);

        open.Add(new Node(start, H(start)));
        gScore[start] = 0;
        fScore[start] = H(start);

        var closed = new HashSet<Vector3Int>();

        while (open.Count > 0)
        {
            // get node with smallest f
            open.Sort((a, b) => a.f.CompareTo(b.f));
            var current = open[0];
            open.RemoveAt(0);

            if (current.pos == goal)
            {
                // reconstruct path
                var path = new List<Vector3Int>();
                var cur = goal;
                while (!cur.Equals(start))
                {
                    path.Add(cur);
                    cur = cameFrom[cur];
                }
                path.Reverse();
                return path;
            }

            closed.Add(current.pos);

            foreach (var n in GetNeighbors(current.pos))
            {
                if (closed.Contains(n)) continue;
                if (!IsWalkable(n) && n != goal) continue;

                int tentativeG = gScore[current.pos] + 1;
                if (!gScore.ContainsKey(n) || tentativeG < gScore[n])
                {
                    cameFrom[n] = current.pos;
                    gScore[n] = tentativeG;
                    fScore[n] = tentativeG + H(n);
                    if (!open.Exists(x => x.pos == n))
                    {
                        open.Add(new Node(n, fScore[n]));
                    }
                }
            }
        }

        return null; // no path
    }

    private class Node
    {
        public Vector3Int pos;
        public int f;
        public Node(Vector3Int p, int fScore) { pos = p; f = fScore; }
    }

}
