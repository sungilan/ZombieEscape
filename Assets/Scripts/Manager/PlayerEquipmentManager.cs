using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;
    private Dictionary<Item.EquipmentType, Item> equippedItems = new Dictionary<Item.EquipmentType, Item>();

    public void Equip(Item item)
    {
        Debug.Log("장착");
        equippedItems[item.equipmentType] = item;
        // 추가적인 착용 효과 (예: 스탯 증가, 모델 변경 등) 구현
        playerStatus.hp += 30;
    }

    public void Unequip(Item.EquipmentType equipmentType)
    {
        if (equippedItems.ContainsKey(equipmentType))
        {
            Debug.Log("해제!!");
            equippedItems.Remove(equipmentType);
            // 추가적인 해제 효과 (예: 스탯 감소, 모델 변경 등) 구현
            playerStatus.hp -= 30;
        }
    }

    public Item GetEquippedItem(Item.EquipmentType equipmentType)
    {
        return equippedItems.ContainsKey(equipmentType) ? equippedItems[equipmentType] : null;
    }
}