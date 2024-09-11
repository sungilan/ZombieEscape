using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct Speaker
{
    public SpriteRenderer spriteRenderer; // 캐릭터 이미지
    public Image imageDialog; // 대화창 UI 이미지
    public Text textName; // 현재 대사중인 캐릭터 이름
    public Text textDialogue; // 현재 대사 출력
    public GameObject objectArrow; //대사가 완료되었을 때 출력되는 커서 오브젝트
}

[System.Serializable]
public struct DialogueData // 화자 이름과 대사 정보를 저장하는 구조체
{
    public int speakerIndex; // 이름과 대사를 출력할 현재 DialogueSystem의 speakers 배열 순번
    public string name; // 캐릭터 이름
    [TextArea(3,5)]
    public string dialogue; // 대사
}

public class DialogueSystem : MonoBehaviour
{
    [SerializeField]
    private Speaker[] speakers; // 대화에 참여하는 캐릭터들의 UI 배열
    [SerializeField]
    private DialogueData[] dialogues; // 현재 분기의 대사 목록 배열
    [SerializeField]
    private bool isAutoStart = true; // 자동 시작 여부
    private bool isFirst = true; // 최초 1회만 호출하기 위한 변수
    private int currentDialogueIndex = -1; // 현재 대사 순번
    private int currentSpeakerIndex = 0; // 현재 말을 하는 화자(Speaker)의 speakers 배열 순번
    private float typingSpeed = 0.1f; // 텍스트 타이핑 효과의 재생 속도
    private bool isTypingEffect = false; // 텍스트 타이핑 효과를 재생중인지
    public static bool isDialogue = false; // 대화가 실행중인지 여부
    
    private void Awake()
    {
        Setup();
    }
    private void Setup()
    {
        // 모든 대화 관련 게임오브젝트 비활성화
        for(int i =0; i<speakers.Length; ++i)
        {
            SetActiveObjects(speakers[i], false);
            // 캐릭터 이미지는 보이도록 설정
            speakers[i].spriteRenderer.gameObject.SetActive(true);
        }
    }

    public bool UpdateDialogue()
    {
        isDialogue = true;
        // 대사 분기가 시작될 때 1회만 호출
        if(isFirst == true)
        {
            Setup(); // 초기화. 캐릭터 이미지는 활성화하고, 대사 관련 UI는 모두 비활성화
            if(isAutoStart)SetNextDialogue(); // 자동 재생으로 설정되어 있으면 첫번째 대사 재생
            isFirst = false;
        }
        if(Input.GetMouseButtonDown(0))
        {
            //텍스트 타이핑 효과를 재생 중일 때 마우스 왼쪽 클릭하면 타이핑 효과 종료
            if(isTypingEffect == true)
            {
                isTypingEffect = false;
                //타이핑 효과를 중지하고, 현재 대사 전체를 출력한다
                StopCoroutine("OntypingText");
                speakers[currentSpeakerIndex].textDialogue.text = dialogues[currentDialogueIndex].dialogue;
                //대사가 완료되었을 때 출력되는 커서 활성화
                speakers[currentSpeakerIndex].objectArrow.SetActive(true);
                return false;
            }
            // 대사가 남아있을 경우 다음 대사 진행
            if(dialogues.Length>currentDialogueIndex +1)
            {
                SetNextDialogue();
            }
            // 대사가 더이상 없을 경우 모든 오브젝트를 비활성화하고 true 반환
            else
            {
                //현재 대화에 참여했던 모든 캐릭터, 대화 관련 UI를 보이지 않게 비활성화
                for(int i =0; i<speakers.Length; ++i)
                {
                    SetActiveObjects(speakers[i],false);
                    //SetActiveObjects()에 캐릭터 이미지를 보이지 않게 하는 부분이 없기 때문에 별도로 호출
                    speakers[i].spriteRenderer.gameObject.SetActive(false);
                    isDialogue = false;
                }
                
            }
        }
        return false;
    }
    private void SetNextDialogue()
    {
        //이전 화자의 대화 관련 오브젝트 비활성화
        SetActiveObjects(speakers[currentSpeakerIndex], false);
        //다음 대사를 진행
        currentDialogueIndex ++;
        //현재 화자 순번 설정
        currentSpeakerIndex = dialogues[currentDialogueIndex].speakerIndex;
        //현재 화자의 대화 관련 오브젝트 활성화
        SetActiveObjects(speakers[currentSpeakerIndex], true);
        //현재 화자 이름 텍스트 설정
        speakers[currentSpeakerIndex].textName.text = dialogues[currentDialogueIndex].name;
        //현재 화자의 대사 텍스트 설정
        //speakers[currentSpeakerIndex].textDialogue.text = dialogues[currentDialogueIndex].dialogue;
        StartCoroutine("OnTypingText");
    }
    private IEnumerator OnTypingText()
    {
        int index = 0;
        isTypingEffect = true;
        //텍스트를 한글자씩 타이핑치듯 재생
        while(index<=dialogues[currentDialogueIndex].dialogue.Length)
        {
            speakers[currentSpeakerIndex].textDialogue.text = dialogues[currentDialogueIndex].dialogue.Substring(0,index);
            index++;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTypingEffect = false;
        //대사가 완료되었을 떄 출력되는 커서 활성화
        speakers[currentSpeakerIndex].objectArrow.SetActive(true);
    }
    private void SetActiveObjects(Speaker speaker, bool visible)
    {
        speaker.imageDialog.gameObject.SetActive(visible);
        speaker.textName.gameObject.SetActive(visible);
        speaker.textDialogue.gameObject.SetActive(visible);
        //화살표는 대사가 종료되었을 때만 활성화하기 때문에 항상 false
        speaker.objectArrow.SetActive(false);
        //캐릭터 알파 값 변경
        Color color = speaker.spriteRenderer.color;
        color.a = visible == true ? 1 : 0;
        speaker.spriteRenderer.color = color;
    }
}
