using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPos;  // 에너미가 스폰될 위치
    [SerializeField] private string enemyTag = "Zombie";  // ObjectPoolManager에서 사용할 에너미 태그
    [SerializeField] private float spawnInterval = 2f;  // 에너미 스폰 간격 (초)

    private float spawnTimer;

    void Update()
    {
        spawnTimer += Time.deltaTime;

        // 스폰 간격이 지나면 새로운 에너미를 생성
        if (spawnTimer >= spawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        // ObjectPoolManager를 통해 에너미를 가져오기
        GameObject enemy = ObjectPoolManager.Instance.GetObject(enemyTag);

        if (enemy != null)
        {
            // 에너미 위치와 회전을 설정
            enemy.transform.position = spawnPos.position;
            enemy.transform.rotation = spawnPos.rotation;
        }
    }
}
