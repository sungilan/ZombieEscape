using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;//인터페이스 기능(여러개 상속 가능)


public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{

    public Item item; // 획득한 아이템
    public int itemCount; // 획득한 아이템의 개수
    public Image itemImage; // 아이템의 이미지

    // 필요한 컴포넌트
    [SerializeField] private Text text_Count; //아이템 개수 텍스트
    public GameObject go_CountImage; // 아이템 개수 카운트 이미지

    private ItemEffectDatabase theItemEffectDatabase;
    public Rect baseRect;

    void Start()
    {
        theItemEffectDatabase = FindObjectOfType<ItemEffectDatabase>();
        baseRect = transform.parent.parent.GetComponent<RectTransform>().rect;
    }

    // 이미지의 투명도 조절
    //(itemImage는 아무것도 없는 평상시엔 알파값을 0으로, 아이템이 있을시에는 알파값을 255로 초기화시켜 보이게 만듬)
    private void SetColor(float _alpha) // 매개변수 : 알파값
    {
        Color color = itemImage.color; // itemImage의 color를 color 변수에 대입
        color.a = _alpha; // color 변수에 대입된 itemImage의 알파값(color.a)를 _alpha에 대입
        itemImage.color = color; //_alpha에 0이 오면 이미지가 안보이고, 1이 오면 이미지가 보이게됨.
    }

    // 아이템 획득
    public void AddItem(Item _item, int _count = 1) // 매개변수 : 얻는 아이템, 얻는 갯수(기본값 = 1)
    {
        item = _item; //item에 획득 아이템(_item) 넣고
        itemCount = _count; // itemCount에 획득 아이템 갯수(_count) 넣고
        itemImage.sprite = item.itemImage; // itemImage의 sprite를 item.itemImage(획득 아이템의 이미지)로 교체

        if(item.itemType != Item.ItemType.Equipment) // 아이템의 타입이 장비아이템이 아닐 경우
        {
            go_CountImage.SetActive(true); // 갯수 카운트 이미지 활성화
            text_Count.text = itemCount.ToString(); 
            // itemCount의 int 값을 string으로 변환 후 text_Count의 텍스트에 대입
        }
        else // 아이템이 장비아이템일 경우 카운트이미지를 띄우지 않음
        {
            text_Count.text = "0"; // 카운트 텍스트 0으로 초기화
            go_CountImage.SetActive(false); // 카운트 이미지를 비활성화
        }
        SetColor(1); // itemImage의 알파값을 1로 해 아이템이미지가 보이게 함
    }
    // 아이템 개수 조정.
    public void SetSlotCount(int _count) // 아이템을 사용하거나 얻을 때 카운트 조정(_count : 사용하거나 얻은 갯수)
    {
        itemCount += _count; // 사용하면 -, 얻으면 +
        text_Count.text = itemCount.ToString(); //itemCount를 string으로 변환 후 text_Count로 나타냄

        if (itemCount <= 0) //아이템 카운트가 0이거나 0보다 작을 경우(아이템을 다 썼을 경우)
            ClearSlot(); //슬롯 초기화 함수 실행
    }

    // 슬롯 초기화.
    public void ClearSlot() //아이템 카운트가 0이거나 0보다 작을 경우(아이템을 다 썼을 경우) 실행
    {
        item = null; // item을 null로
        itemCount = 0; // itemCount를 0으로 초기화
        itemImage.sprite = null; // itemImage를 null로
        SetColor(0); // itemImage의 알파값을 0으로 해 아이템이미지가 안보이게 함

        text_Count.text = "0"; // 카운트 텍스트 0으로 초기화
        go_CountImage.SetActive(false); //카운트 이미지 비활성화
    }

    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right) //이 스크립트가 적용된 개체에 마우스 우클릭시
        {
            if(item != null) //null이 아닐 경우
            {
                theItemEffectDatabase.UseItem(item);
                if(item.itemType == Item.ItemType.Used || item.itemType == Item.ItemType.Box) // item의 타입이 Used일 경우
                    SetSlotCount(-1); // 카운트를 -1시킨다(소모)
            }
        }
    }

    public virtual void OnBeginDrag(PointerEventData eventData) // 드래그 시작
    {
        if(item != null)
        {
            DragSlot.instance.dragSlot = this; // 현재 슬롯을 드래그 슬롯으로 설정
            DragSlot.instance.DragSetImage(itemImage); // 드래그 슬롯의 이미지 설정

            DragSlot.instance.transform.position = eventData.position; // 드래그 슬롯의 위치를 마우스 위치로 설정
        }
    }

    public virtual void OnDrag(PointerEventData eventData) // 드래그 중
    {
        if (item != null)
        {
            DragSlot.instance.transform.position = eventData.position; // 드래그 슬롯의 위치를 마우스 위치로 설정
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData) // 드래그 종료
    {
        if(DragSlot.instance.transform.localPosition.x < baseRect.xMin || DragSlot.instance.transform.localPosition.x > baseRect.xMax
           || DragSlot.instance.transform.localPosition.y < baseRect.yMin || DragSlot.instance.transform.localPosition.y > baseRect.yMax
           )
        {
            if(DragSlot.instance.dragSlot != null) // 드래그 영역이 baseRect 안쪽일 때
            {
                DragSlot.instance.SetColor(0); // 드래그 슬롯의 투명도 설정 (보이지 않게)
                DragSlot.instance.dragSlot = null; // 드래그 슬롯 해제
            }
        }
        else // 드래그 영역이 baseRect 바깥쪽일 때
        {
            //Debug.Log(item + "버리기");
            DragSlot.instance.SetColor(0);
            DragSlot.instance.dragSlot = null;

        //    // 아이템 프리팹 생성
        //    Vector3 mousePosition = Input.mousePosition; // 마우스 위치 가져오기
        //    mousePosition.z = Camera.main.nearClipPlane; // 카메라와의 거리 설정 (2D 게임에서는 필요 없음)

        //    // 마우스 위치에 약간의 오프셋을 추가
        //    float yOffset = -0.5f; // 원하는 오프셋 값으로 설정
        //    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition) + new Vector3(0, yOffset, 0); // 오프셋 적용
        //    Instantiate(item.itemPrefab, worldPosition, Quaternion.identity); // 월드 좌표에 아이템 프리팹 생성
        //    SetSlotCount(-1); // 카운트를 -1시킨다(소모)
        //    SoundManager.instance.PlaySE("아이템 픽업");
        }
    }

    public virtual void OnDrop(PointerEventData eventData) // 아이템을 드롭할 때 호출
    {
        if (DragSlot.instance.dragSlot != null)
            ChangeSlot();
    }

    public void ChangeSlot() // 슬롯 교체 처리
    {
        Item _tempItem = item; // 임시로 현재 슬롯의 아이템 저장
        int _tempItemCount = itemCount; // 임시로 현재 슬롯의 아이템 개수 저장

        AddItem(DragSlot.instance.dragSlot.item, DragSlot.instance.dragSlot.itemCount); // 드래그 슬롯의 아이템 추가

        if (_tempItem != null)
            DragSlot.instance.dragSlot.AddItem(_tempItem, _tempItemCount); // 이전 슬롯의 아이템을 드래그 슬롯으로 복사
        else
            DragSlot.instance.dragSlot.ClearSlot(); // 이전 슬롯에 아이템이 없었다면 드래그 슬롯 초기화
    }


    // 마우스가 슬롯에 들어갈 때 발동.
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(item != null)
            theItemEffectDatabase.ShowToolTop(item, transform.position);
    }

    // 슬롯에서 빠져나갈 때 발동.
    public void OnPointerExit(PointerEventData eventData)
    {
        theItemEffectDatabase.HideToolTip();
    }
}