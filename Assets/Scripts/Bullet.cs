using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int damage;

    private void Start()
    {
        //Invoke(nameof(DestroyBullet), 5f);
    }

    private void DestroyBullet()
    {
        ObjectPoolManager.Instance.ReturnObject("Bullet", this.gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            DestroyBullet();
        }
        else
        {
            DestroyBullet();
        }
    }
}

