using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PushableBlock : MonoBehaviour
{
    public float pushSpeed = 10f;
    private Vector3Int currentCell;

    private void Start()
    {
        currentCell = GridManager.I.WorldToCell(transform.position);
        GridManager.I.SetOccupant(currentCell, gameObject);
        gameObject.tag = "Block";
        transform.position = GridManager.I.CellToWorldCenter(currentCell);
    }

    //public bool CanBePushedTo(Vector3Int dest)
    //{
    //    if (GridManager.I.HasWall(dest)) return false;
    //    if (!GridManager.I.HasFloor(dest)) return false;
    //    if (GridManager.I.IsOccupied(dest)) return false;

    //    return true;
    //}

    public bool CanBePushedTo(Vector3Int dest)
    {
        if (GridManager.I.HasWall(dest)) return false;
        if (!GridManager.I.HasFloor(dest)) return false;

        var occ = GridManager.I.GetOccupant(dest);
        if (occ != null && !occ.CompareTag("Collectible"))
            return false;

        return true;
    }


    public void PushTo(Vector3Int dest, float delay = 0f)
    {
        StartCoroutine(PushCoroutine(dest, delay));
    }

    //private IEnumerator PushCoroutine(Vector3Int dest, float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    GridManager.I.RemoveOccupant(currentCell);
    //    GridManager.I.SetOccupant(dest, gameObject);

    //    Vector3 start = transform.position;
    //    Vector3 end = GridManager.I.CellToWorldCenter(dest);
    //    float t = 0f;
    //    float duration = Vector3.Distance(start, end) / pushSpeed;

    //    while (t < duration)
    //    {
    //        t += Time.deltaTime;
    //        transform.position = Vector3.Lerp(start, end, t / duration);
    //        yield return null;
    //    }
    //    transform.position = end;
    //    currentCell = dest;
    //}

    private IEnumerator PushCoroutine(Vector3Int dest, float delay)
    {
        yield return new WaitForSeconds(delay);

        GridManager.I.RemoveOccupant(currentCell);
        GridManager.I.SetOccupant(dest, gameObject);

        Vector3 start = transform.position;
        Vector3 end = GridManager.I.CellToWorldCenter(dest);
        float t = 0f;
        float duration = Vector3.Distance(start, end) / pushSpeed;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }

        transform.position = end;
        currentCell = dest;

        // Notify GameManager
        GameManager.I.OnBlockMoved();
    }

}
