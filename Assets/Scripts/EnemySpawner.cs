using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnPos;  // ���ʹ̰� ������ ��ġ
    [SerializeField] private string enemyTag = "Zombie";  // ObjectPoolManager���� ����� ���ʹ� �±�
    [SerializeField] private float spawnInterval = 2f;  // ���ʹ� ���� ���� (��)

    private float spawnTimer;

    void Update()
    {
        spawnTimer += Time.deltaTime;

        // ���� ������ ������ ���ο� ���ʹ̸� ����
        if (spawnTimer >= spawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        // ObjectPoolManager�� ���� ���ʹ̸� ��������
        GameObject enemy = ObjectPoolManager.Instance.GetObject(enemyTag);

        if (enemy != null)
        {
            // ���ʹ� ��ġ�� ȸ���� ����
            enemy.transform.position = spawnPos.position;
            enemy.transform.rotation = spawnPos.rotation;
        }
    }
}
