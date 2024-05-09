using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{   
    public static GameManager instance;

    [Header("Game Control")]
    public float gameTime;
    public float maxGameTime = 15 * 60; //s
    public bool isLive;

    [Header("Player Info")]
    public float health;
    public float maxHealth = 100;
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 10, 25, 50, 100, 150, 220, 300, 400, 500, 600 };

    [Header("Game Object")]
    public Player player;
    public PoolManager pool;
    public LevelUp uiLevelUp;
    public Result uiResult;
    public GameObject EnemyCleaner;

    private void Awake()
    {
        instance = this;
    }

    public void GameStart() //버튼 명령어
    {
        Application.targetFrameRate = 120;
        health = maxHealth;
        uiLevelUp.Select(6); //원거리 무기 제공
        Resume(); //TimeScale을 다시 설정하기 위해서

        AudioManager.instance.PlayBgm(true);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
    }

    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    public void GameClear()
    {
        StartCoroutine(GameClearRoutine());
    }

    private IEnumerator GameOverRoutine()
    {   
        isLive = false;
        yield return new WaitForSeconds(0.5f); //wait for animation
        uiResult.gameObject.SetActive(true);
        uiResult.Lose();
        Stop(); //For TimeScale

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose);
    }

    private IEnumerator GameClearRoutine()
    {
        isLive = false;
        EnemyCleaner.SetActive(true);
        yield return new WaitForSeconds(0.5f); //wait for discard enemies
        uiResult.gameObject.SetActive(true);
        uiResult.Win();
        Stop(); //For TimeScale

        AudioManager.instance.PlayBgm(false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win);
    }

    public void GameRetry() //버튼 명령어
    {
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (!isLive) return;

        gameTime += Time.deltaTime;
        if (gameTime > maxGameTime) {
            gameTime = maxGameTime;
            GameClear();
        }
    }

    public void GetExp()
    {   
        if(!isLive) return;

        exp++;
        if (exp == nextExp[Mathf.Min(level, nextExp.Length-1)]) {
            exp -= nextExp[Mathf.Min(level, nextExp.Length - 1)];
            level++;
            uiLevelUp.Show();
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive= true;
        Time.timeScale = 1;
    }
}
