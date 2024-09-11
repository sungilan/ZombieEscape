using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

[System.Serializable]
public class ItemEffect
{
    public string itemName; // 아이템의 이름. (키값)
    [Tooltip("HP, SP, DP, HUNGRY, THIRSTY , SATISFY만 가능합니다")]
    public string[] part; // 부위.
    public int[] num; // 수치.
    
}

public class ItemEffectDatabase : MonoBehaviour 
{
    public Item boxItem;
    public int boxItemCount;
    [SerializeField] private ItemEffect[] itemEffects;

    //필요한 컴포넌트
    [Header("Components")]
    [SerializeField] private PlayerStatus thePlayerStatus;
    [SerializeField] private WeaponManager theWeaponManager;
    //[SerializeField] private GunController theGunController;
    [SerializeField] private Inventory theInventory;
    [SerializeField] private SlotToolTip theSlotToolTip;

    private const string HP = "HP", SP = "SP", DP = "DP", HUNGRY = "HUNGRY", THIRSTY = "THIRSTY", Ammo = "Ammo";

    public void ShowToolTop(Item _item, Vector3 _pos)
    {
        theSlotToolTip.ShowToolTip(_item, _pos);
    }

    public void HideToolTip()
    {
        theSlotToolTip.HideToolTip();
    }

    public void UseItem(Item _item)
    {
        if (_item.itemType == Item.ItemType.Equipment)
        {
            //StartCoroutine(theWeaponManager.ChangeWeaponCoroutine(_item.itemName));
        }
        else if (_item.itemType == Item.ItemType.Box)
        {
            SoundManager.Instance.PlaySE("재장전");
            theInventory.AcquireItem(boxItem, boxItemCount);
        }
        else if (_item.itemType == Item.ItemType.Used)
        {
            for (int x = 0; x < itemEffects.Length; x++)
            {
                if(itemEffects[x].itemName == _item.itemName)
                {
                    for (int y = 0; y < itemEffects[x].part.Length; y++)
                    {
                        SoundManager.Instance.PlaySE("포션 섭취");
                        switch (itemEffects[x].part[y])
                        {
                            case HP:
                                thePlayerStatus.IncreaseHP(itemEffects[x].num[y]);
                                break;
                            case SP:
                                thePlayerStatus.IncreaseSP(itemEffects[x].num[y]);
                                break;
                            case DP:
                                thePlayerStatus.IncreaseDP(itemEffects[x].num[y]);
                                break;
                            case THIRSTY:
                                thePlayerStatus.IncreaseThirsty(itemEffects[x].num[y]);
                                break;
                            case HUNGRY:
                                thePlayerStatus.IncreaseHungry(itemEffects[x].num[y]);
                                break;
                            case Ammo:
                                SoundManager.Instance.PlaySE("재장전");
                                //if (theGunController.currentGun !=  null)
                                //theGunController.PlusAmmo();
                                break;
                            default:
                                Debug.Log("잘못된 Status 부위. HP, SP, DP, HUNGRY, THIRSTY , SATISFY만 가능합니다");
                                break;
                        }
                        Debug.Log(_item.itemName + " 을 사용했습니다");
                    }
                    return;
                }       
            }
            Debug.Log("ItemEffectDatabase에 일치하는 itemName 없습니다");
        }
    }
}