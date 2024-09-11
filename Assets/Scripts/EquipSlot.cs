using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;


public class EquipmentSlot : Slot
{
    public Item.EquipmentType equipmentType;
    private PlayerEquipmentManager equipmentManager;
    [SerializeField] Inventory theInventory;

    void Start()
    {
        baseRect = transform.parent.parent.GetComponent<RectTransform>().rect;
        equipmentManager = FindObjectOfType<PlayerEquipmentManager>();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) //이 스크립트가 적용된 개체에 마우스 우클릭시
        {
            if (item != null && item.itemType == Item.ItemType.Equipment)
            {
                // 장비 해제
                theInventory.AcquireItem(item);
                equipmentManager.Unequip(item.equipmentType);
                ClearSlot();
            }
        }
    }

    public override void OnBeginDrag(PointerEventData eventData) // 드래그 시작
    {
        if (item != null && item.itemType == Item.ItemType.Equipment)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);

            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public override void OnEndDrag(PointerEventData eventData) // 드래그 종료
    {
            if (DragSlot.instance.dragSlot != null)
            {
                // 슬롯 해제 (장비는 버리지 않고 해제만)
                DragSlot.instance.SetColor(0);
                DragSlot.instance.dragSlot = null;

                theInventory.AcquireItem(item);
                equipmentManager.Unequip(item.equipmentType);
                ClearSlot();
            }
            else
            {
                Debug.Log(item + "장비 착용 해제");
                DragSlot.instance.SetColor(0);
                DragSlot.instance.dragSlot = null;
            }
    }

    public override void OnDrop(PointerEventData eventData) // 아이템을 드롭할 때 호출
    {
        Slot dragItem = DragSlot.instance.dragSlot;
        if (dragItem != null && dragItem.item.itemType == Item.ItemType.Equipment)
        {
            if (dragItem.item.equipmentType == equipmentType)
            {
                ChangeSlot();
                equipmentManager.Equip(item); // 장비 착용
            }
            else
            {
                Debug.Log("장비 타입이 맞지 않습니다.");
            }
        }
        else
        {
            Debug.Log("장비 타입의 아이템이 아닙니다.");
        }
    }

    public void EquipItem(Item _item)
    {
        AddItem(_item);
        equipmentManager.Equip(_item);
    }

    public void UnequipItem()
    {
        if (item != null)
        {
            equipmentManager.Unequip(item.equipmentType);
            ClearSlot();
        }
    }
}