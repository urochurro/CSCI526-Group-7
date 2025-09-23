using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f; // used for smoothing
    private bool isMoving = false;
    private Vector3Int currentCell;

    private void Start()
    {
        currentCell = GridManager.I.WorldToCell(transform.position);
        GridManager.I.SetOccupant(currentCell, gameObject);
        transform.position = GridManager.I.CellToWorldCenter(currentCell);
    }

    private void Update()
    {
        if (isMoving) return;

        Vector2Int dir = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) dir = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) dir = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) dir = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) dir = Vector2Int.right;
        else return;

        TryMove(dir);
    }

    //private void TryMove(Vector2Int dir)
    //{
    //    var target = currentCell + new Vector3Int(dir.x, dir.y, 0);
    //    if (GridManager.I.HasWall(target)) return;

    //    if (!GridManager.I.IsOccupied(target))
    //    {
    //        // move into empty cell
    //        StartCoroutine(MoveToCell(target));
    //    }
    //    else
    //    {
    //        var occ = GridManager.I.GetOccupant(target);
    //        if (occ != null && occ.CompareTag("Block"))
    //        {
    //            // attempt to push block
    //            var blockScript = occ.GetComponent<PushableBlock>();
    //            var pushTarget = target + new Vector3Int(dir.x, dir.y, 0);
    //            if (blockScript != null && blockScript.CanBePushedTo(pushTarget))
    //            {
    //                // push block then move player into the block's previous cell
    //                blockScript.PushTo(pushTarget, 0.15f);
    //                StartCoroutine(MoveToCell(target));
    //            }
    //            else
    //            {
    //                // cannot push
    //                // optional: play bump sound/animation
    //            }
    //        }
    //    }
    //}
    private void TryMove(Vector2Int dir)
    {
        Vector3Int target = currentCell + new Vector3Int(dir.x, dir.y, 0);

        // Check if player can walk there
        if (GridManager.I.IsWalkable(target))
        {
            StartCoroutine(MoveToCell(target));
            return;
        }

        // Check if a block is there
        GameObject occ = GridManager.I.GetOccupant(target);
        if (occ != null && occ.CompareTag("Block"))
        {
            Vector3Int pushTarget = target + new Vector3Int(dir.x, dir.y, 0);
            var blockScript = occ.GetComponent<PushableBlock>();
            if (blockScript != null && blockScript.CanBePushedTo(pushTarget))
            {
                // push block then move player
                blockScript.PushTo(pushTarget, 0.05f);
                StartCoroutine(MoveToCell(target));
            }
        }
        // else: blocked, do nothing
    }


    private IEnumerator MoveToCell(Vector3Int dest)
    {
        isMoving = true;
        GridManager.I.RemoveOccupant(currentCell);
        GridManager.I.SetOccupant(dest, gameObject);

        Vector3 start = transform.position;
        Vector3 end = GridManager.I.CellToWorldCenter(dest);

        float t = 0f;
        float duration = 1f / moveSpeed;
        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }
        transform.position = end;
        currentCell = dest;
        isMoving = false;

        // Check for collectible
        var occ = GridManager.I.GetOccupant(dest);
        if (occ != null && occ.CompareTag("Collectible"))
        {
            CollectItem(occ);
        }
    }

    private void CollectItem(GameObject collectible)
    {
        Debug.Log("Collected: " + collectible.name);
        GridManager.I.RemoveOccupant(GridManager.I.WorldToCell(collectible.transform.position));
        Destroy(collectible);

        GameManager.I.OnCollectiblePicked(); // optional, to track score
    }
}
