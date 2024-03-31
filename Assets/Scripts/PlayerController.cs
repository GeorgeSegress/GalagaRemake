using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject shot;
    public GameObject explosion;

    private Rigidbody2D myBod;

    public float thrusters;
    public float shootSpeed;
    private float movX = 0;

    public GameObject playerShoot;
    public GameObject playerDeath;

    private void Start()
    {
        myBod= GetComponent<Rigidbody2D>();
    }

    // fairly basic player controller that moves the player according to their inputs and fires accordingly.
    private void Update()
    {
        // Movement
        movX = Input.GetAxis("Horizontal");

        if(movX != 0)
            myBod.velocity = new Vector2(myBod.velocity.x * .70f + movX * thrusters * 0.3f, 0);

        else
            myBod.velocity = new Vector2(myBod.velocity.x * 0.70f, 0);

        // Shooting
        if (Input.GetButtonDown("Fire"))
            Shoot();
    }

    void Shoot()
    {
        Instantiate(playerShoot);
        Instantiate(shot, transform.position + new Vector3(0, 1, 0), Quaternion.identity).GetComponent<Shot>().Instan(shootSpeed, false);
    }

    public void Damage()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Instantiate(playerDeath);
        FindObjectOfType<ScoreManager>().CallHelper();
        Destroy(gameObject);
    }
}
