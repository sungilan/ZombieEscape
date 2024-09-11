using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionController : MonoBehaviour 
{

    [SerializeField]
    private float range; // 습득 가능한 최대 거리.

    private bool pickupActivated = false; // 습득 가능할 시 true.
    private bool dialogueActivated = false; // 대화 가능할 시 true.

    private RaycastHit hitInfo; // 충돌체 정보 저장.


    // 아이템 레이어에만 반응하도록 레이어 마스크를 설정.
    [SerializeField]
    private LayerMask layerMask;

    // 필요한 컴포넌트.
    [SerializeField]
    private Text actionText;
    [SerializeField]
    private Inventory theInventory;
    [SerializeField]
    private string sound_player_Pickup;
    [SerializeField]
    private DialogueSystem dialogueSystem01;

	// Update is called once per frame
	void Update () 
    {
        CheckItem();
        TryAction();
	}

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckItem();
            CanPickUp();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            CheckItem();
            CanDialogue();
        }
    }

    private void CanPickUp()
    {
        if (pickupActivated)
        {
            if(hitInfo.transform != null)
            {
                SoundManager.Instance.PlaySE(sound_player_Pickup);
                Debug.Log(hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " 획득했습니다");
                theInventory.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                //아이템을 인벤토리에 넣어준다.
                Destroy(hitInfo.transform.gameObject);
                InfoDisappear();
            }
        }
    }

    private void CheckItem()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, range, layerMask))
        {
            if (hitInfo.transform.tag == "Item")
            {
                ItemInfoAppear();
            }
            if (hitInfo.transform.tag == "NPC")
            {
                DialogueInfoAppear();
            }
        }
        else
            InfoDisappear();
    }

    private void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName + " 획득 " + "<color=yellow>" + "(E)" + "</color>";
    }
    private void InfoDisappear()
    {
        pickupActivated = false;
        dialogueActivated = false;
        actionText.gameObject.SetActive(false);
    }
    ///
    private void DialogueInfoAppear()
    {
        dialogueActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = " 와 대화한다 " + "<color=yellow>" + "(J)" + "</color>";
    }
    private void CanDialogue()
    {
        if (dialogueActivated)
        {
            if(hitInfo.transform != null)
            {
                // 대화 가능한 NPC일 경우 대화를 시작합니다.
                StartCoroutine(StartDialogue());
                InfoDisappear();
            }
        }
    }
    public IEnumerator StartDialogue()
    {
        //첫번째 대사 분기 시작
        yield return new WaitUntil(()=>dialogueSystem01.UpdateDialogue());
    }
}