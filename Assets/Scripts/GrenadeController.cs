using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    // Ȱ��ȭ ����.
    public static bool isActivate = false;

    // ���� ������ Hand�� Ÿ�� ����.
    public GrenadeThrower currentGrenade;

    // ������??
    private bool isAttack = false;

    protected RaycastHit hitInfo;
    public GameObject hud;


    private void TryAttack()
    {
        if (!Inventory.inventoryActivated && CraftManual.CraftisActivated == false)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!isAttack)
                {
                    StartCoroutine(ThrowGrenade());
                }
            }
        }
    }
    IEnumerator ThrowGrenade()
    {
        isAttack = true;
        currentGrenade.anim.SetTrigger("Attack");
        GameObject instantGrenade = Instantiate(currentGrenade.grenadePrefab, transform.position, transform.rotation);
        Rigidbody rbInstantGrenade = instantGrenade.GetComponent<Rigidbody>();

        // �������� �׸��� ��ô
        Vector3 throwDirection = (transform.forward + transform.up).normalized; // ���ʰ� ���� ������ ����
        float throwForce = 15f; // ��ô�ϴ� ��
        float upwardForce = 2f; // �������� �ִ� �߰� ��

        // ���ʰ� ���� �������� ���� ����
        rbInstantGrenade.AddForce(throwDirection * throwForce + Vector3.up * upwardForce, ForceMode.Impulse);
        rbInstantGrenade.AddTorque(Vector3.forward, ForceMode.Impulse);
        yield return new WaitForSeconds(3f);
        isAttack = false;
    }
    
    public virtual void GrenadeChange(GrenadeThrower _grenade)
    {
        //if (WeaponManager.currentWeapon != null)
        //    WeaponManager.currentWeapon.gameObject.SetActive(false);

        currentGrenade = _grenade;
        //WeaponManager.currentWeapon = currentGrenade.GetComponent<Transform>();
        //WeaponManager.currentWeaponAnim = currentGrenade.anim;
        hud.gameObject.SetActive(true); // Hud�� ���

        currentGrenade.transform.localPosition = Vector3.zero;
        currentGrenade.gameObject.SetActive(true);
    }

    public GrenadeThrower GetGrenade()
    {
        return currentGrenade;
    }

    private void Update()
    {
        TryAttack();
    }
}