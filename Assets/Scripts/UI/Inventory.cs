using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private string sound_Inventory_Open;
    public static bool inventoryActivated = false; // 인벤토리 활성화 상태변수
    // inventoryActivated가 true일 시에는 카메라 움직임과 공격을 막음

    // 필요한 컴포넌트
    [SerializeField] private GameObject go_InventoryBase; // 인벤토리 베이스 이미지
    [SerializeField] private GameObject go_SlotsParent; 
    // Slot들을 일괄적으로 관리하는 Grid_Setting 오브젝트(Grid Layout Group 컴포넌트를 추가, 슬롯이미지들을 일괄정렬시킴)
    //Constraint - Fixed Column Count : 10 -> 10개 이후 다음 줄로 내려감 

    private Slot[] slots; // 슬롯들
    private SlotToolTip slotToolTip;

    // Use this for initialization
    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<Slot>(); //go_SlotsParent 안의 자식개체들(Slot)을 slots 안에
        slotToolTip = GetComponentInChildren<SlotToolTip>(true);
    }

    // Update is called once per frame
    void Update()
    {
        TryOpenInventory();
    }

    private void TryOpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I)) // I 버튼을 눌렀을 경우
        {
            //inventoryActivated = !inventoryActivated; 
            //inventoryActivated가 true이면 false로, false이면 true로 바꿈

            if (inventoryActivated == false)
            {
            // inventoryActivated가 true이면
                OpenInventory();
                inventoryActivated = true;
            } 
            else // inventoryActivated가 false이면
            {
                CloseInventory();
                inventoryActivated = false;
            }
               
        }
    }

    private void OpenInventory()
    {
        SoundManager.Instance.PlaySE(sound_Inventory_Open);
        go_InventoryBase.SetActive(true); // go_InventoryBase를 활성화
    }

    private void CloseInventory()
    {
        SoundManager.Instance.PlaySE(sound_Inventory_Open);
        go_InventoryBase.SetActive(false); // go_InventoryBase를 비활성화
        slotToolTip.HideToolTip();
    }

    public void AcquireItem(Item _item, int _count = 1) // 획득 아이템, 획득 갯수(초기값 : 1)
    {
        if(Item.ItemType.Equipment != _item.itemType) // _item의 타입이 장비아이템이 아닐 경우
        {
            for (int i = 0; i < slots.Length; i++) // slots의 길이만큼 for문 반복
            {
                if(slots[i].item != null) // slots[i].item이 null이 아닐 경우
                {
                    if (slots[i].item.itemName == _item.itemName) 
                    // slots 배열 안에 있는 item 중 획득한 _item의 이름과 같은 이름이 존재하는 경우
                    {
                        slots[i].SetSlotCount(_count); // SetSlotCount의 _count만큼 카운트 증가시켜줌
                        return; // 만족하는 것이 없으면 for문 빠져나옴
                    }
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null) // 슬롯에 빈자리가 있을 경우
            {
                slots[i].AddItem(_item, _count); // 빈자리를 찾아서 빈자리에 아이템을 넣어준다.
                return;
            }
        }
    }
}