using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour 
{
    // 필요한 컴포넌트
    [Header("Components")]
    [SerializeField] private WeaponManager weaponManager;

    // 필요하면 HUD 호출, 필요 없으면 HUD 비활성화.
    [SerializeField] private GameObject go_BulletHUD;

    // 총알 개수 텍스트에 반영
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI text_Name;
    [SerializeField] private TextMeshProUGUI text_carryBullet;
    [SerializeField] private TextMeshProUGUI text_Bullet;
    [SerializeField] private Image weaponIcon;
    
	private void Start() 
    {
        weaponManager = FindObjectOfType<WeaponManager>();
    }
	private void Update () 
    {
        UpdateHUDUI();
	}

    private void UpdateHUDUI()
    {
        if (weaponManager.currentWeapon != null)
        {
            weaponIcon.sprite = weaponManager.currentWeapon.GethudIcon();
            text_Name.text = weaponManager.currentWeapon.GetweaponName();

            if (weaponManager.currentWeapon is RangeWeapon currentGun)
            {
                text_carryBullet.text = "Total : " + currentGun.carryBulletCount.ToString();
                text_Bullet.text = currentGun.currentBulletCount.ToString() + "/" + currentGun.reloadBulletCount.ToString();
            }
            else if (weaponManager.currentWeapon is FlameThrower flameThrower)
            {
                text_carryBullet.text = "연료: " + ((int)flameThrower.GetFuel()).ToString();
                text_Bullet.text = " ";
            }
            else if (weaponManager.currentWeapon is CloseWeapon)
            {
                text_carryBullet.text = " ";
                text_Bullet.text = " ";
            }
        }
    }
}