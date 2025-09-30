using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public Vector2Int facingDir;
    // Start is called before the first frame update
    public void setFacingDir(Vector2Int dir)
    {
        Debug.Log("Bullet moving: " + dir);
        facingDir.x = dir.x;
        facingDir.y = dir.y;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Block") || other.CompareTag("Wall"))
        {
            Destroy(this.gameObject, 0.1f);
        }
        if (other.CompareTag("ExplosiveGhost"))
        {
            if(other.GetComponent<ExplosiveGhost>().isDetonating){
                Destroy(this.gameObject, 0.1f);
                other.GetComponent<ExplosiveGhost>().detonate();
            }
        }
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(-facingDir.x, -facingDir.y, 0) * 0.01f;
    }
}
