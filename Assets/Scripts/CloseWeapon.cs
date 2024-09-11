using System.Collections;
using UnityEngine;

public class CloseWeapon : Weapon
{
    [SerializeField] protected float attackRate = 1f;
    [SerializeField] protected BoxCollider col;
    [SerializeField] protected ParticleSystem hitEffect;
    [SerializeField] protected float nextAttackTime = 0f;

    private void Start()
    {
        col.enabled = false;
    }
    private void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                UseWeapon();
            }
        }
    }

    protected override void UseWeapon()
    {
        anim.SetTrigger("Attack");
        SoundManager.Instance.PlaySE(attackSound);
        nextAttackTime = Time.time + 1f / attackRate;
    }

    public void ActiveCollider()
    {
        col.enabled = true;
    }
    public void DeactiveCollider()
    {
        col.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    // 공격 범위 시각화 (디버깅용)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
