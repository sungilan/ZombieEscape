using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject {

    public string itemName; // 아이템의 이름.
    [TextArea]
    public string itemDesc; // 아이템 설명.
    public ItemType itemType; // 아이템의 유형.
    public Sprite itemImage; // 아이템의 이미지.
    public GameObject itemPrefab; // 아이템의 프리팹.
    public EquipmentType equipmentType;
    public Weapon weapon; // 무기 

    public enum ItemType
    {
        Equipment,
        Used,
        Ingredient,
        Box
    }
    public enum EquipmentType
    {
        Helmet,
        Armor,
        Weapon
    }
}