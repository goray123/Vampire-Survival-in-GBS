using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    private float timer;
    private int spawnLevel;
    public float levelTime;

    private void Start()
    {
        spawnPoint = GetComponentsInChildren<Transform>(); //나를 포함하는 아이들
        levelTime = GameManager.instance.maxGameTime / spawnData.Length;
    }

    private void Update()
    {
        if (!GameManager.instance.isLive) return;

        timer += Time.deltaTime;
        spawnLevel = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length-1); //버림
        
        if (timer > spawnData[spawnLevel].spawnTime) {
            Spawn();
            timer -= spawnData[spawnLevel].spawnTime;
        }
    }

    private void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; //0은 Player이므로 제외
        enemy.GetComponent<Enemy>().Init(spawnData[spawnLevel]);
    }
}

[System.Serializable]
public class SpawnData //스폰레벨설정
{
    public int spriteType;
    public float spawnTime;
    public int health;
    public float speed;
}
