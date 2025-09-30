using System.Collections;
using UnityEngine;

public class ExplosiveGhost : MonoBehaviour
{
    public float stepDelay = 0.35f;
    public float moveSpeed = 6f;
    public bool isDetonating = true;
    public GameObject explosionPrefab;

    private Vector3Int currentCell;

    public void detonate()
    {
        GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion, 0.5f);
        Destroy(gameObject);
    }

    private void Start()
    {
        currentCell = GridManager.I.WorldToCell(transform.position);
        GridManager.I.SetOccupant(currentCell, gameObject);
        transform.position = GridManager.I.CellToWorldCenter(currentCell);
    }

}