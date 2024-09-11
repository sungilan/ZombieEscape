using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeController : MonoBehaviour
{
    // 활성화 여부.
    public static bool isActivate = false;

    // 현재 장착된 Hand형 타입 무기.
    public GrenadeThrower currentGrenade;

    // 공격중??
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

        // 포물선을 그리며 투척
        Vector3 throwDirection = (transform.forward + transform.up).normalized; // 앞쪽과 위쪽 방향을 결합
        float throwForce = 15f; // 투척하는 힘
        float upwardForce = 2f; // 위쪽으로 주는 추가 힘

        // 앞쪽과 위쪽 방향으로 힘을 가함
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
        hud.gameObject.SetActive(true); // Hud를 띄움

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