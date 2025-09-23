using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Collectible : MonoBehaviour
{
    private void Reset()
    {
        // Make sure collider is set as trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Notify GameManager
            GameManager.I.OnCollectiblePicked();

            // Make the collectible disappear
            Destroy(gameObject);
        }
    }
}

