using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private Collider spikeTrigger;
    [SerializeField] private int damageAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾ ���� Ʈ���ſ� ���Դ��� Ȯ��
        if (other.CompareTag("Player"))
        {
            // ���� �ִϸ��̼� ���� (���� �ھƿ�����)
            anim.SetTrigger("Active");

            // ������ Ʈ���� Ȱ��ȭ
            spikeTrigger.enabled = true;
        }

        // ���ÿ� �÷��̾ ��Ҵ��� Ȯ�� (���� Ȱ��ȭ ����)
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
