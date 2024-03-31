using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// overarching game controller that handles wave management and player lives
public class ScoreManager : MonoBehaviour
{
    public Text scoreText;
    public Text waveText;
    private int score = 0;
    private int waveCount = 0;
    private int lives = 3;
    public GameObject WaveObject;
    public GameObject Player;
    public GameObject[] livesIcons;
    public bool respawning;
    private GameObject curPlayer;

    public GameObject goSound;
    public GameObject music;

    private void Start()
    {
        curPlayer = Instantiate(Player, new Vector3(0, -4), Quaternion.identity);
        NewWave();
    }

    public void ScorePoints(int points)
    {
        score += points;
        scoreText.text = score.ToString("00000");
    }

    public void NewWave()
    {
        waveCount++;
        if(waveCount > 5)
            waveText.text = "Congratulations, you won!\nI didn't expect you to get this far.";
        else
            StartCoroutine(WaveSpawn());
    }

    public IEnumerator WaveSpawn()
    {
        if (waveCount != 1)
        {
            if (waveCount != 5)
                waveText.text = "Great work!\nNew wave incoming.";
            else
                waveText.text = "Great work!\nLast wave incoming!";
            yield return new WaitForSeconds(3);
        }
        waveText.text = "Wave: " + waveCount + " / 5";
        yield return new WaitForSeconds(2);
        waveText.text = "GO!";
        Instantiate(goSound);
        yield return new WaitForSeconds(1);
        Instantiate(WaveObject, new Vector2(0, 2.5f), Quaternion.identity);
        FindObjectOfType<EnemyManager>().Player = curPlayer;
        waveText.text = "";
    }

    public void CallHelper()
    {
        lives--;
        livesIcons[lives].SetActive(false);
        if (lives != 0)
            StartCoroutine(RespawnPlayer());
        else
        {
            waveText.text = "GAME OVER :(\nFinal Score:\n" + score.ToString("00000");
            Destroy(music);
        }
    }

    public IEnumerator RespawnPlayer()
    {
        StartCoroutine(FindObjectOfType<EnemyManager>().ResetWave(waveState.swaying));
        waveText.text = "Respawning \nLives left: " + lives;
        yield return new WaitForSeconds(2);

        waveText.text = "Ready";
        yield return new WaitForSeconds(2);
        waveText.text = "";
        curPlayer = Instantiate(Player, new Vector3(0, -4), Quaternion.identity);
        FindObjectOfType<EnemyManager>().Player = curPlayer;
        respawning = false;
    }
}
