using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nicknameInput;

    public void OnClickLoginBtn()
    {
        //로그인 버튼을 눌렀을 때 로그인 처리
        //닉네임 정보 넘기기
        DBManager.Instance.playerName = nicknameInput.text;
        //클라이언트 시작
        SceneManager.LoadScene("GameStage");
    }

}