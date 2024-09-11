using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Collider spikeTrigger;
    [SerializeField] private int damageAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 함정 트리거에 들어왔는지 확인
        if (other.CompareTag("Player"))
        {
            // 함정 애니메이션 실행 (가시 솟아오르기)
            anim.SetTrigger("Active");

            // 가시의 트리거 활성화
            spikeTrigger.enabled = true;
        }

        // 가시에 플레이어가 닿았는지 확인 (함정 활성화 이후)
        if (spikeTrigger.enabled == true)
        {
            PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
            if(playerStatus != null)
            {
                playerStatus.DecreaseHP(damageAmount);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        spikeTrigger.enabled = false;
    }
}
