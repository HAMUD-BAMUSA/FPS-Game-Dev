using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    public int damage = 20;          // How much damage this bullet does per hit
    public float bulletLife = 3f;    // Bullet will self-destruct after 3 seconds if it doesn't hit anything

    void Start()
    {
        // Automatically destroy bullet after a few seconds to prevent clutter in the scene
        Destroy(gameObject, bulletLife);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the thing we hit has the tag "Enemy"
        if (collision.collider.CompareTag("Enemy"))
        {
            Enemy Enemy = collision.collider.GetComponent<Enemy>();

            if (Enemy != null)
            {
                // Tell the Enemy to take damage
                Enemy.TakeDamage(damage);
            }
        }

        // Destroy the bullet after hitting anything
        Destroy(gameObject);
    }
}