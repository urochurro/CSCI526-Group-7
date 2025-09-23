//using System.IO;
//using UnityEngine;
//using UnityEngine.Tilemaps;

//public class LevelLoader : MonoBehaviour
//{
//    public TextAsset levelText;
//    public TileBase floorTile;
//    public TileBase wallTile;
//    public TileBase targetTile;

//    public GameObject playerPrefab;
//    public GameObject blockPrefab;
//    public GameObject ghostPrefab;

//    public Tilemap floorTilemap;
//    public Tilemap wallTilemap;
//    public Tilemap targetTilemap;

//    private void Start()
//    {
//        LoadLevelFromText(levelText.text);
//    }

//    public void LoadLevelFromText(string text)
//    {
//        var lines = text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
//        int height = lines.Length;
//        int width = 0;
//        foreach (var l in lines) if (l.Length > width) width = l.Length;

//        // origin so that first line is top: we'll place rows from top to bottom
//        for (int y = 0; y < height; y++)
//        {
//            var line = lines[y];
//            for (int x = 0; x < line.Length; x++)
//            {
//                char c = line[x];
//                int rx = x;
//                int ry = (height - 1) - y; // flip y so file's first line is top
//                var cell = new Vector3Int(rx, ry, 0);

//                switch (c)
//                {
//                    case '#':
//                        wallTilemap.SetTile(cell, wallTile);
//                        break;
//                    case '.':
//                        floorTilemap.SetTile(cell, floorTile);
//                        break;
//                    case 'T':
//                        floorTilemap.SetTile(cell, floorTile);
//                        targetTilemap.SetTile(cell, targetTile);
//                        break;
//                    case 'B':
//                        floorTilemap.SetTile(cell, floorTile);
//                        Instantiate(blockPrefab, GridManager.I.CellToWorldCenter(cell), Quaternion.identity);
//                        break;
//                    case 'P':
//                        floorTilemap.SetTile(cell, floorTile);
//                        Instantiate(playerPrefab, GridManager.I.CellToWorldCenter(cell), Quaternion.identity);
//                        break;
//                    case 'G':
//                        floorTilemap.SetTile(cell, floorTile);
//                        Instantiate(ghostPrefab, GridManager.I.CellToWorldCenter(cell), Quaternion.identity);
//                        break;
//                    default:
//                        // treat as floor
//                        floorTilemap.SetTile(cell, floorTile);
//                        break;
//                }
//            }
//        }
//    }
//}

using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelLoader : MonoBehaviour
{
    [Header("Level Settings")]
    public TextAsset levelText;
    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase targetTile;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject blockPrefab;
    public GameObject ghostPrefab;
    public GameObject collectiblePrefab; 

    [Header("Tilemaps")]
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public Tilemap targetTilemap;

    private void Start()
    {
        if (levelText == null)
        {
            Debug.LogError("Level text file not assigned!");
            return;
        }

        LoadLevelFromText(levelText.text);
    }

    public void LoadLevelFromText(string text)
    {
        var lines = text.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        int height = lines.Length;
        int width = 0;
        foreach (var l in lines)
            if (l.Length > width) width = l.Length;

        // First pass: lay out all tiles
        for (int y = 0; y < height; y++)
        {
            var line = lines[y];
            for (int x = 0; x < line.Length; x++)
            {
                char c = line[x];
                int rx = x;
                int ry = (height - 1) - y; // flip y so top line is top
                Vector3Int cell = new Vector3Int(rx, ry, 0);

                switch (c)
                {
                    case '#':
                        wallTilemap.SetTile(cell, wallTile);
                        break;
                    case '.':
                    case 'P':
                    case 'B':
                    case 'G':
                        floorTilemap.SetTile(cell, floorTile);
                        break;
                    case 'T':
                        floorTilemap.SetTile(cell, floorTile);
                        targetTilemap.SetTile(cell, targetTile);
                        break;
                    default:
                        floorTilemap.SetTile(cell, floorTile);
                        break;
                }
            }
        }

        // Second pass: instantiate objects after all tiles are set
        for (int y = 0; y < height; y++)
        {
            var line = lines[y];
            for (int x = 0; x < line.Length; x++)
            {
                char c = line[x];
                int rx = x;
                int ry = (height - 1) - y;
                Vector3Int cell = new Vector3Int(rx, ry, 0);
                Vector3 spawnPos = GridManager.I.CellToWorldCenter(cell);

                switch (c)
                {
                    case 'P':
                        Instantiate(playerPrefab, spawnPos, Quaternion.identity);
                        break;
                    case 'B':
                        Instantiate(blockPrefab, spawnPos, Quaternion.identity);
                        break;
                    case 'G':
                        Instantiate(ghostPrefab, spawnPos, Quaternion.identity);
                        break;
                    case 'C': // collectible
                        var col = Instantiate(collectiblePrefab, spawnPos, Quaternion.identity);
                        col.tag = "Collectible"; // tag it
                        GridManager.I.SetOccupant(cell, col);
                        break;
                }
            }
        }
    }
}

