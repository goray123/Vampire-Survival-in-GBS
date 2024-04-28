using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;
    private float timer;
    private int spawnLevel;

    private void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>(); //나를 포함하는 아이들
    }

    private void Update()
    {
        timer += Time.deltaTime;
        spawnLevel = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / 10), spawnData.Length-1); //버림
        
        if (timer > spawnData[spawnLevel].spawnTime) {
            Spawn();
            timer -= spawnData[spawnLevel].spawnTime;
        }
    }

    private void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
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
