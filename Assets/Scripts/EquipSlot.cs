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
        if (eventData.button == PointerEventData.InputButton.Right) //�� ��ũ��Ʈ�� ����� ��ü�� ���콺 ��Ŭ����
        {
            if (item != null && item.itemType == Item.ItemType.Equipment)
            {
                // ��� ����
                theInventory.AcquireItem(item);
                equipmentManager.Unequip(item.equipmentType);
                ClearSlot();
            }
        }
    }

    public override void OnBeginDrag(PointerEventData eventData) // �巡�� ����
    {
        if (item != null && item.itemType == Item.ItemType.Equipment)
        {
            DragSlot.instance.dragSlot = this;
            DragSlot.instance.DragSetImage(itemImage);

            DragSlot.instance.transform.position = eventData.position;
        }
    }

    public override void OnEndDrag(PointerEventData eventData) // �巡�� ����
    {
            if (DragSlot.instance.dragSlot != null)
            {
                // ���� ���� (���� ������ �ʰ� ������)
                DragSlot.instance.SetColor(0);
                DragSlot.instance.dragSlot = null;

                theInventory.AcquireItem(item);
                equipmentManager.Unequip(item.equipmentType);
                ClearSlot();
            }
            else
            {
                Debug.Log(item + "��� ���� ����");
                DragSlot.instance.SetColor(0);
                DragSlot.instance.dragSlot = null;
            }
    }

    public override void OnDrop(PointerEventData eventData) // �������� ����� �� ȣ��
    {
        Slot dragItem = DragSlot.instance.dragSlot;
        if (dragItem != null && dragItem.item.itemType == Item.ItemType.Equipment)
        {
            if (dragItem.item.equipmentType == equipmentType)
            {
                ChangeSlot();
                equipmentManager.Equip(item); // ��� ����
            }
            else
            {
                Debug.Log("��� Ÿ���� ���� �ʽ��ϴ�.");
            }
        }
        else
        {
            Debug.Log("��� Ÿ���� �������� �ƴմϴ�.");
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