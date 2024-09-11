using UnityEngine;
using System.Collections;

public class FlameThrower : CloseWeapon
{
    [SerializeField] private float burnInterval = 0.5f;  // ���� ���� �ֱ�
    [SerializeField] public float fuel = 100f;  // ���� �ִ�ġ
    [SerializeField] private float fuelConsumptionRate = 10f;  // ���� �Һ� �ӵ� (�ʴ�)
    private bool isFiring = false;

    private void Update()
    {
        // ���ᰡ �����ְ�, ���� �ð��� ��������
        if (fuel > 0 )
        {
            if (Input.GetButton("Fire1"))  // ��ư�� ������ �ִ� ���� ����
            {
                if (!isFiring)
                {
                    UseWeapon();
                    isFiring = true;
                    hitEffect.gameObject.SetActive(true);
                }
                else
                {
                    ConsumeFuel();  // ���� �Һ�
                }
            }
            else if (isFiring)
            {
                StopFiring();  // ��ư�� ���� ���� ����
            }
        }
        else if (isFiring)
        {
            StopFiring();  // ���ᰡ ���� ���� ���� ����
        }
    }

    private void ConsumeFuel()
    {
        fuel -= fuelConsumptionRate * Time.deltaTime;  // ���� �Һ�
        if (fuel <= 0)
        {
            fuel = 0;
            StopFiring();  // ���ᰡ 0�� �Ǹ� ���� ����
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
        // ���� �ִϸ��̼�, ���� ���
        anim.SetTrigger("Attack");
        SoundManager.Instance.PlaySE(attackSound);
        nextAttackTime = Time.time + 1f / attackRate;

        // �ݶ��̴� Ȱ��ȭ
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
        while (isFiring && fuel > 0)  // �Ҳ��� �հ� �ִ� ���ȸ� ���� ����
        {
            enemy.TakeDamage(damage);  // �������� �Ҳ� ����
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

    // ���Ḧ HUD�� ǥ���ϰų� UI�� ������ �� �ִ� �޼���
    public float GetFuel()
    {
        return fuel;
    }
}
