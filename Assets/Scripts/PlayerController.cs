using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f; // used for smoothing
    public GameObject Bullet;
    private bool isMoving = false;
    private Vector3Int currentCell;
    private int facingDir;

    private void Start()
    {
        currentCell = GridManager.I.WorldToCell(transform.position);
        GridManager.I.SetOccupant(currentCell, gameObject);
        transform.position = GridManager.I.CellToWorldCenter(currentCell);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ghost"))
        {
            GameManager.I.OnPlayerCaught();
            isMoving = false;
            Debug.Log("Player is caught by ghost");
        }
        if(other.CompareTag("Collectible")){
            CollectItem(other.gameObject);
            Debug.Log("Player collected collectible");
        }
    }

    private void ShootBullet()
    {
        Debug.Log("Player shot bullet: " + facingDir);
        Instantiate(Bullet, transform.position + new Vector3(0, -0.1f, 0), Quaternion.identity);
        switch (facingDir) {
            case 0:
                Bullet.GetComponent<BulletBehavior>().facingDir.x = 0;
                Bullet.GetComponent<BulletBehavior>().facingDir.y = -1;
                break;
            case 1:
                Bullet.GetComponent<BulletBehavior>().facingDir.x = 0;
                Bullet.GetComponent<BulletBehavior>().facingDir.y = 1;
                break;
            case 2:
                Bullet.GetComponent<BulletBehavior>().facingDir.x = 1;
                Bullet.GetComponent<BulletBehavior>().facingDir.y = 0;
                break;
            case 3:
                Bullet.GetComponent<BulletBehavior>().facingDir.x = -1;
                Bullet.GetComponent<BulletBehavior>().facingDir.y = 0;
                break;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) ShootBullet();
        if (isMoving) return;

        Vector2Int dir = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
            dir = Vector2Int.up;
            facingDir = 0;
        } else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
            dir = Vector2Int.down;
            facingDir = 1;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            dir = Vector2Int.left;
            facingDir = 2;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
            dir = Vector2Int.right;
            facingDir = 3;
        }
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
        int success = GridManager.I.SetOccupant(dest, gameObject);

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

        GridManager.I.RemoveOccupant(currentCell);
        currentCell = dest;
        isMoving = false;
    }

    private void CollectItem(GameObject collectible)
    {
        Debug.Log("Collected: " + collectible.name);
        GridManager.I.RemoveOccupant(GridManager.I.WorldToCell(collectible.transform.position));
        Destroy(collectible);

        // Note: GameManager.I.OnCollectiblePicked() is called by Collectible.OnTriggerEnter2D
        // so we don't need to call it here to avoid double counting
    }
}
