using UnityEngine;
using System.Collections;

public class FlameThrower : CloseWeapon
{
    [SerializeField] private float burnInterval = 0.5f;  // 지속 피해 주기
    [SerializeField] public float fuel = 100f;  // 연료 최대치
    [SerializeField] private float fuelConsumptionRate = 10f;  // 연료 소비 속도 (초당)
    private bool isFiring = false;

    private void Update()
    {
        // 연료가 남아있고, 공격 시간이 지났으면
        if (fuel > 0 )
        {
            if (Input.GetButton("Fire1"))  // 버튼을 누르고 있는 동안 공격
            {
                if (!isFiring)
                {
                    UseWeapon();
                    isFiring = true;
                    hitEffect.gameObject.SetActive(true);
                }
                else
                {
                    ConsumeFuel();  // 연료 소비
                }
            }
            else if (isFiring)
            {
                StopFiring();  // 버튼을 떼면 공격 중지
            }
        }
        else if (isFiring)
        {
            StopFiring();  // 연료가 없을 때도 공격 중지
        }
    }

    private void ConsumeFuel()
    {
        fuel -= fuelConsumptionRate * Time.deltaTime;  // 연료 소비
        if (fuel <= 0)
        {
            fuel = 0;
            StopFiring();  // 연료가 0이 되면 공격 중지
        }
    }

    private void StopFiring()
    {
        isFiring = false;
        col.enabled = false;
        hitEffect.gameObject.SetActive(false);
    }

    protected override void UseWeapon()
    {
        // 공격 애니메이션, 사운드 재생
        anim.SetTrigger("Attack");
        SoundManager.Instance.PlaySE(attackSound);
        nextAttackTime = Time.time + 1f / attackRate;

        // 콜라이더 활성화
        ActiveCollider();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                StartCoroutine(ApplyBurnDamage(enemy));
            }
        }
    }

    private IEnumerator ApplyBurnDamage(Enemy enemy)
    {
        while (isFiring && fuel > 0)  // 불꽃을 뿜고 있는 동안만 지속 피해
        {
            enemy.TakeDamage(damage);  // 지속적인 불꽃 피해
            yield return new WaitForSeconds(burnInterval);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            StopCoroutine(ApplyBurnDamage(other.GetComponent<Enemy>()));
        }
    }

    // 연료를 HUD에 표시하거나 UI로 관리할 수 있는 메서드
    public float GetFuel()
    {
        return fuel;
    }
}
