//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class GhostAI : MonoBehaviour
//{
//    public float stepDelay = 0.35f;
//    public float moveSpeed = 6f;
//    private Vector3Int currentCell;
//    private Coroutine moveRoutine;

//    private enum State { Patrol, Chase, Trapped }
//    private State state = State.Patrol;

//    private void Start()
//    {
//        currentCell = GridManager.I.WorldToCell(transform.position);
//        GridManager.I.SetOccupant(currentCell, gameObject);
//        transform.position = GridManager.I.CellToWorldCenter(currentCell);
//        moveRoutine = StartCoroutine(BehaviorLoop());
//    }

//    private IEnumerator BehaviorLoop()
//    {
//        while (true)
//        {
//            yield return new WaitForSeconds(stepDelay);
//            var playerGO = GameObject.FindGameObjectWithTag("Player");
//            if (playerGO == null) yield return null;
//            Vector3Int playerCell = GridManager.I.WorldToCell(playerGO.transform.position);

//            var path = GridManager.I.FindPath(currentCell, playerCell);

//            if (path == null)
//            {
//                // can't reach player -> trapped
//                state = State.Trapped;
//                // TODO: play trapped animation
//                continue;
//            }
//            else
//            {
//                state = State.Chase;
//                // follow first step in path
//                if (path.Count > 0)
//                {
//                    Vector3Int next = path[0];
//                    // move to next
//                    yield return MoveToCell(next);
//                }
//            }
//        }
//    }

//    private IEnumerator MoveToCell(Vector3Int dest)
//    {
//        GridManager.I.RemoveOccupant(currentCell);
//        GridManager.I.SetOccupant(dest, gameObject);

//        Vector3 start = transform.position;
//        Vector3 end = GridManager.I.CellToWorldCenter(dest);
//        float t = 0f;
//        float duration = 1f / moveSpeed;

//        while (t < duration)
//        {
//            t += Time.deltaTime;
//            transform.position = Vector3.Lerp(start, end, t / duration);
//            yield return null;
//        }
//        transform.position = end;
//        currentCell = dest;

//        // check if met player
//        var occ = GridManager.I.GetOccupant(currentCell);
//        if (occ != null && occ.CompareTag("Player"))
//        {
//            // caught player -> Game Over logic
//            Debug.Log("Player caught!");
//            // optionally call GameManager
//        }
//    }
//}

using System.Collections;
using UnityEngine;

public class GhostAI : MonoBehaviour
{
    public float stepDelay = 0.35f;
    public float moveSpeed = 6f;
    public bool isDetonating = false;
    public GameObject explosionPrefab;

    private Vector3Int currentCell;
    private Coroutine moveRoutine;

    private enum State { Patrol, Chase, Trapped }
    private State state = State.Patrol;

    public void detonate()
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    
    private void Start()
    {
        currentCell = GridManager.I.WorldToCell(transform.position);
        GridManager.I.SetOccupant(currentCell, gameObject);
        transform.position = GridManager.I.CellToWorldCenter(currentCell);

        moveRoutine = StartCoroutine(BehaviorLoop());
    }

    private IEnumerator BehaviorLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(stepDelay);

            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO == null) yield return null;

            Vector3Int playerCell = GridManager.I.WorldToCell(playerGO.transform.position);
            var path = GridManager.I.FindPath(currentCell, playerCell);

            if (path == null)
            {
                state = State.Trapped;
                Debug.Log("Ghost is now " + state);
                continue;
            }
            else
            {
                state = State.Chase;
                if (path.Count > 0)
                {
                    Vector3Int next = path[0];
                    //Debug.Log("Ghost is now " + state + " and moving to " + next[0] + "," + next[1] + "," + next[2]);
                    yield return MoveToCell(next);
                }
            }
        }
    }

    private IEnumerator MoveToCell(Vector3Int dest)
    {
        // Check if player is in the destination before moving
        //GameObject occ = GridManager.I.GetOccupant(dest);
        // if (occ != null && occ.CompareTag("Player"))
        // {
        //     GameManager.I.OnPlayerCaught();
        //     // Optionally stop ghost movement
        //     yield break;
        // }

        // Move ghost in grid
        //GridManager.I.RemoveOccupant(currentCell);
        if(enabled == false){
            yield break;
        }
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
    }

}

