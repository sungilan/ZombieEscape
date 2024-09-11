using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotToolTip : MonoBehaviour 
{

    // 필요한 컴포넌트
    [Header("Components")]
    [SerializeField] private GameObject go_Base; // 툴팁 베이스
    [SerializeField] private TextMeshProUGUI txt_ItemName; // 아이템 이름
    [SerializeField] private TextMeshProUGUI txt_ItemDesc; // 아이템 설명
    [SerializeField] private TextMeshProUGUI txt_ItemHowtoUsed; // 아이템 사용방법

    public void ShowToolTip(Item _item, Vector3 _pos)
    {
        go_Base.SetActive(true);
        _pos += new Vector3(go_Base.GetComponent<RectTransform>().rect.width * 0.5f, -go_Base.GetComponent<RectTransform>().rect.height, 0f);
        go_Base.transform.position = _pos;

        txt_ItemName.text = _item.itemName;
        txt_ItemDesc.text = _item.itemDesc;

        if (_item.itemType == Item.ItemType.Equipment)
            txt_ItemHowtoUsed.text = "우클릭 - 장착";
        else if (_item.itemType == Item.ItemType.Used)
            txt_ItemHowtoUsed.text = "우클릭 - 사용";
        else
            txt_ItemHowtoUsed.text = "";
    }

    public void HideToolTip()
    {
        go_Base.SetActive(false);
    }
}