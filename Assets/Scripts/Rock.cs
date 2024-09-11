using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour 
{
    [SerializeField] private int hp; // 바위의 체력.
    [SerializeField] private float destroyTime; // 파편 제거 시간
    private BoxCollider col; // 컬라이더.


    // 필요한 게임 오브젝트.
    [SerializeField] private GameObject go_rock; // 일반 바위.
    [SerializeField] private GameObject go_debris; // 깨진 바위.
    [SerializeField] private GameObject go_effect_prefabs; // 채굴 이펙트.
    [SerializeField] private GameObject go_rock_item_prefab; // 스폰 아이템 프리팹.
    [SerializeField] private int spawnCount; // 아이템 스폰 갯수

    //필요한 사운드 이름
    [SerializeField] private string strike_Sound;
    [SerializeField] private string destroy_Sound;

    private void Start()
    {
        col = GetComponent<BoxCollider>();
    }
    public void Mining()
    {
        SoundManager.Instance.PlaySE(strike_Sound);
        var clone = Instantiate(go_effect_prefabs, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        hp--;

        if (hp <= 0)
            Destruction();
    }

    private void Destruction()
    {
        SoundManager.Instance.PlaySE(destroy_Sound);
        col.enabled = false;
        for(int i = 0; i <= spawnCount -1; i++)
        {
            Instantiate(go_rock_item_prefab,go_rock.transform.position,Quaternion.identity);
        }
        Destroy(go_rock);

        go_debris.SetActive(true);
        Destroy(go_debris, destroyTime);
    }

}