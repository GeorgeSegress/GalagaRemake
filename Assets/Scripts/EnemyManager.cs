using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum waveState
{
    swaying,
    pulsing,
    reseting
};

// Overarching wave manager for all enemies
public class EnemyManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs;

    public GameObject rootSpot;
    private Transform[,] rootSpots;
    private GameObject[,] enemies;

    private Vector3 baseScale;

    private Vector3 basePos;
    public waveState curState = waveState.swaying;

    public int waveCount;
    public int enemyCount;
    public float chanceByCount;
    private float intTime;

    public GameObject Player;
    public Vector3 playerPos;

    private void Start()
    {
        GameObject player = FindObjectOfType<PlayerController>().gameObject;
        
        rootSpots = new Transform[5,10];
        enemies = new GameObject[5,10];
        int enemCount = 0;
        // crafts the rows of enemies using transforms for target positions and instantiates enemies
        for (int row = 0; row < 5; row++)
        {
            if (row >= 1) enemCount = 1;
            if (row >= 3) enemCount = 2;
            for (int column = Mathf.Clamp(3 - row, 0, 10); column < Mathf.Clamp(7 + row, 0, 10); column++)
            {
                enemyCount++;
                rootSpots[row, column] = Instantiate(rootSpot, new Vector2((float)(column - 4.5f) * 0.7f, 4f - (float)row * 0.5f), Quaternion.identity).transform;
                rootSpots[row, column].transform.SetParent(transform);
                enemies[row, column] = Instantiate(enemyPrefabs[enemCount], rootSpots[row, column].transform.position + Vector3.up * Random.Range(5f, 12f), Quaternion.identity);
                enemies[row, column].GetComponent<Enemy>().Instan(this, rootSpots[row, column], player);
            }
        }
        transform.localScale *= 0.75f;
        baseScale = transform.localScale;
        chanceByCount = (40f - (float)enemyCount) / 40f;
        basePos = transform.localPosition;
    }

    private void FixedUpdate()
    {
        if (Player)
            playerPos = Player.transform.position;
        switch (curState)
        {
            // moves back and forth, waiting 5 seconds before doing the pulsing movement again
            case waveState.swaying:
                transform.position = basePos + Mathf.Sin(intTime) * Vector3.right;
                intTime += Time.deltaTime;
                if (intTime > 5)
                    StartCoroutine(ResetWave(waveState.pulsing));
                break;
            // stays in position and causes enemy positions to pulse like they do in the original galaga
            case waveState.pulsing:
                transform.localScale = baseScale * (1 + Mathf.Sin(intTime) / 6);
                intTime += Time.deltaTime;
                if (intTime > 5)
                    StartCoroutine(ResetWave(waveState.swaying));
                break;
        }
    }

    public IEnumerator ResetWave(waveState newState)
    {
        curState = waveState.reseting;
        while(Mathf.Abs(transform.localScale.x - baseScale.x) > 0.05f || Mathf.Abs(transform.position.x - basePos.x) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, basePos, 0.05f);
            transform.localScale = Vector3.Lerp(transform.localScale, baseScale, 0.05f);
            yield return null;
        }
        intTime = 0;
        transform.localScale = baseScale;
        transform.position = basePos;
        curState = newState;
    }

    public void EnemyDied()
    {
        enemyCount--;
        chanceByCount = (40f - (float)enemyCount) / 40f;
        if (enemyCount <= 0)
        {
            FindObjectOfType<ScoreManager>().NewWave();
            Destroy(gameObject);
        }
    }
}
