using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shot : MonoBehaviour
{
    public bool enemy = false;

    public void Instan(float speed, bool Enemy)
    {
        enemy = Enemy;
        GetComponent<Rigidbody2D>().velocity = Vector2.up * speed;
    }

    // fairly basic script for bullets that allows for flexibility to be either player or enemy bullet to reduce script number
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Environment"))
            Destroy(gameObject);
        else if(collision.CompareTag("Enemy") && !enemy)
        {
            collision.GetComponent<Enemy>().Damage();
            Destroy(gameObject);
        }
        else if(collision.CompareTag("Player") && enemy)
        {
            collision.GetComponent<PlayerController>().Damage();
            Destroy(gameObject);
        }
    }
}
