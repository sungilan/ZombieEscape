using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatusPanel : MonoBehaviour
{
    [Header("TextGroup")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI mpText;
    [SerializeField] private TextMeshProUGUI dpText;
    [SerializeField] private TextMeshProUGUI spText;
  
    [Header("Component")]
    [SerializeField] private PlayerStatus playerStatus;

    private void Update()
    {
        hpText.text = playerStatus.hp.ToString() + "( + )";
        mpText.text = playerStatus.mp.ToString() + "( + )";
        dpText.text = playerStatus.dp.ToString() + "( + )";
        spText.text = playerStatus.sp.ToString() + "( + )";
    }
}
