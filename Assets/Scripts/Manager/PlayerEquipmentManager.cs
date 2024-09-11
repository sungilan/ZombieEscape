using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentManager : MonoBehaviour
{
    [SerializeField] private PlayerStatus playerStatus;
    private Dictionary<Item.EquipmentType, Item> equippedItems = new Dictionary<Item.EquipmentType, Item>();

    public void Equip(Item item)
    {
        Debug.Log("����");
        equippedItems[item.equipmentType] = item;
        // �߰����� ���� ȿ�� (��: ���� ����, �� ���� ��) ����
        playerStatus.hp += 30;
    }

    public void Unequip(Item.EquipmentType equipmentType)
    {
        if (equippedItems.ContainsKey(equipmentType))
        {
            Debug.Log("����!!");
            equippedItems.Remove(equipmentType);
            // �߰����� ���� ȿ�� (��: ���� ����, �� ���� ��) ����
            playerStatus.hp -= 30;
        }
    }

    public Item GetEquippedItem(Item.EquipmentType equipmentType)
    {
        return equippedItems.ContainsKey(equipmentType) ? equippedItems[equipmentType] : null;
    }
}