using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum enemyStates
{
    Entering,
    Idling,
    Attacking,
    Returning,
};

public class Enemy : MonoBehaviour
{
    public GameObject explosion;
    public GameObject shot;

    public Sprite[] idle;
    private int idleBkmk;
    private SpriteRenderer myRenderer;

    private Transform mySpot;
    private Vector3 toPlayerJump;

    public float stepCount;
    public float shotSpeed;
    public float moveSpeed;
    private float intAnimate;
    private float intCount = 0;

    public int scorePoints;
    private EnemyManager mrManager;

    private enemyStates myState = enemyStates.Returning;

    public float waitDelay;

    public GameObject enemyDeath;
    public GameObject enemyShoot;

    private void Start()
    {
        myRenderer = GetComponentInChildren<SpriteRenderer>();
        waitDelay = Random.Range(1f, 2f);
    }

    public void Instan(EnemyManager MrManager, Transform MySpot, GameObject thePlayer)
    {
        mrManager = MrManager;
        mySpot = MySpot;
    }

    private void FixedUpdate()
    {
        intAnimate += Time.deltaTime;
        if (intAnimate >= stepCount)
            Animate();

        // Enemy state-based AI system that follows positional target when idling, dashes down when attacking and lerps back to target position when returning
        switch (myState)
        {
            case enemyStates.Idling:
                transform.position = mySpot.position;
                // performs random chance to trigger a flydown
                if(mrManager.curState == waveState.pulsing)
                    intCount += Time.deltaTime;
                if(intCount > waitDelay && mrManager && mrManager.curState == waveState.pulsing)
                {
                    intCount = 0;
                    if (Random.Range(0f, 1.3f) < mrManager.chanceByCount)
                        FlyBy();
                }
                break;
            case enemyStates.Attacking:
                transform.position -= toPlayerJump;
                if (transform.position.y <= -5.5f)
                    BeginReturn();
                intCount += Time.deltaTime;
                if(intCount > waitDelay)
                {
                    intCount = 0;
                    GameObject temp = Instantiate(shot, transform.position + Vector3.down * 0.5f, Quaternion.identity);
                    temp.GetComponent<Shot>().enemy = true;
                    temp.GetComponent<Rigidbody2D>().velocity = new Vector2(toPlayerJump.x, shotSpeed);
                    Instantiate(enemyShoot);
                }
                break;
            case enemyStates.Returning:
                transform.position = Vector3.Lerp(transform.position, mySpot.position, 0.03f);
                if (Mathf.Abs(transform.position.y - mySpot.position.y) < 0.15f && Mathf.Abs(transform.position.x - mySpot.position.x) < 0.15f)
                    myState = enemyStates.Idling;
                break;
        }
    }

    // helper functions for the general AI checker
    public void Animate()
    {
        intAnimate = 0;
        myRenderer.sprite = idle[idleBkmk];
        idleBkmk++;
        if (idleBkmk >= idle.Length) idleBkmk = 0;
    }

    public void Damage()
    {
        Instantiate(enemyDeath);
        Instantiate(explosion, transform.position, Quaternion.identity);
        FindObjectOfType<ScoreManager>().ScorePoints(scorePoints);
        mrManager.EnemyDied();
        Destroy(gameObject);
    }

    public void FlyBy()
    {
        myState = enemyStates.Attacking;
        toPlayerJump = transform.position - Vector3.Lerp(transform.position, mrManager.playerPos, moveSpeed);
    }

    public void BeginReturn()
    {
        myState = enemyStates.Returning;
        transform.position = new Vector2(transform.position.x, 6);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().Damage();
            mrManager.EnemyDied();
            Destroy(gameObject);
        }
    }
}
