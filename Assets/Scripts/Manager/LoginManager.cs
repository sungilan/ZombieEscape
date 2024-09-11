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
        //�α��� ��ư�� ������ �� �α��� ó��
        //�г��� ���� �ѱ��
        DBManager.Instance.playerName = nicknameInput.text;
        //Ŭ���̾�Ʈ ����
        SceneManager.LoadScene("GameStage");
    }

}