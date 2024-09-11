using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CloseWeaponController : MonoBehaviour 
{
    // 미완성 클래스 = 추상 클래스.


    // 현재 장착된 Hand형 타입 무기.
    [SerializeField]
    protected CloseWeapon currentCloseWeapon;

    // 공격중??
    protected bool isAttack = false;
    protected bool isSwing = false;

    protected RaycastHit hitInfo;
    public GameObject hud;


    //protected void TryAttack()
    //{
    //    if(!Inventory.inventoryActivated && CraftManual.CraftisActivated == false)
    //    {
    //        if (Input.GetButton("Fire1") && !isAttack)
    //        {
    //                StartCoroutine(AttackCoroutine());
    //        }
    //    }
    //}

    //protected IEnumerator AttackCoroutine()
    //{
    //    isAttack = true;
    //    currentCloseWeapon.anim.SetTrigger("Attack");

    //    yield return new WaitForSeconds(currentCloseWeapon.attackDelayA);
    //    isSwing = true;

    //    StartCoroutine(HitCoroutine());

    //    yield return new WaitForSeconds(currentCloseWeapon.attackDelayB);
    //    isSwing = false;


    //    yield return new WaitForSeconds(currentCloseWeapon.attackDelay - currentCloseWeapon.attackDelayA - currentCloseWeapon.attackDelayB);
    //    isAttack = false;
    //}


    // 미완성 = 추상 코루틴.
    protected abstract IEnumerator HitCoroutine();


    //protected bool CheckObject()
    //{
        //if (Physics.Raycast(transform.position, transform.forward, out hitInfo, currentCloseWeapon.range))
        //{
        //    // 충돌이 감지되었을 때 디버그 출력
        //    Debug.Log("Hit object: " + hitInfo.collider.gameObject.name);
        //    return true;
        //}
        //    // 충돌이 감지되지 않았을 때 디버그 출력
        //    Debug.Log("No collision detected");
        //return false;
    //}

    // 완성 함수이지만, 추가 편집한 함수.
    public virtual void CloseWeaponChange(CloseWeapon _closeWeapon)
    {
        //if (WeaponManager.currentWeapon != null)
        //    WeaponManager.currentWeapon.gameObject.SetActive(false);

        currentCloseWeapon = _closeWeapon;
        //WeaponManager.currentWeapon = currentCloseWeapon.GetComponent<Transform>();
        //WeaponManager.currentWeaponAnim = currentCloseWeapon.anim;
        hud.gameObject.SetActive(false); // Hud를 띄움

        currentCloseWeapon.transform.localPosition = Vector3.zero;
        currentCloseWeapon.gameObject.SetActive(true);
    }

    public CloseWeapon GetCloseWeapon()
    {
        return currentCloseWeapon;
    }
}