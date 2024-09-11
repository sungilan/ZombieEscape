using System.Collections;
using UnityEngine;


public class WeaponManager : MonoBehaviour // 무기 교체 관리
{
    // 무기 중복 교체 실행 방지.
    public static bool isChangeWeapon = false;

    // 현재 무기
    public Weapon currentWeapon;

    // 무기 교체 딜레이, 무기 교체가 완전히 끝난 시점.
    [SerializeField] private float changeWeaponDelayTime;
    [SerializeField] private float changeWeaponEndDelayTime;

    // 무기 슬롯들 관리
    [SerializeField] private Slot[] weaponSlots;

    // 컴포넌트 초기화
    void Start()
    {
        // 무기 초기 설정 등 추가 설정 가능
    }

    // 매 프레임마다 무기 교체 키 입력 확인
    void Update()
    {
        if (!isChangeWeapon)
        {
            // 각 무기 슬롯에 있는 무기가 있는지 확인하고 교체
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    if (weaponSlots[i].item != null)
                    {
                        string weaponName = weaponSlots[i].item.itemName;
                        if (!string.IsNullOrEmpty(weaponName))
                        {
                            StartCoroutine(ChangeWeaponCoroutine(weaponName));
                        }
                    }
                }
            }
        }
    }

    // 무기 교체 함수 호출
    public void ChangeWeapon(string weaponName)
    {
        StartCoroutine(ChangeWeaponCoroutine(weaponName));
    }

    // 무기 교체 코루틴
    private IEnumerator ChangeWeaponCoroutine(string weaponName)
    {
        isChangeWeapon = true;

        if (currentWeapon != null && currentWeapon.anim != null)
        {
            currentWeapon.anim.SetTrigger("Weapon_Out"); // 무기 내리는 애니메이션 실행
        }

        SoundManager.Instance.PlaySE("재장전"); // 무기 교체 사운드 재생

        yield return new WaitForSeconds(changeWeaponDelayTime);

        CancelPreWeaponAction(); // 이전 무기의 행동 취소
        WeaponChange(weaponName); // 무기 변경

        yield return new WaitForSeconds(changeWeaponEndDelayTime);

        isChangeWeapon = false; // 무기 교체 완료
    }

    // 이전 무기의 액션 취소 함수
    private void CancelPreWeaponAction()
    {
        if (currentWeapon is RangeWeapon rangeWeapon)
        {
            rangeWeapon.CancelFineSight(); // 정조준 취소
            //rangeWeapon.CancelReload();    // 재장전 취소
        }
        else if (currentWeapon is CloseWeapon closeWeapon)
        {
            // 근접 무기 관련 액션 취소 가능
        }
    }

    // 무기 교체 함수
    private void WeaponChange(string weaponName)
    {
        Debug.Log("교체");
        foreach (Slot slot in weaponSlots)
        {
            if (slot.item.itemName == weaponName)
            {
                // 현재 활성화된 무기가 있다면 비활성화
                if (currentWeapon != null)
                {
                    currentWeapon.gameObject.SetActive(false);
                }
                Transform weaponTransform = transform.Find(weaponName);

                Debug.Log("트랜스폼 :" + weaponTransform);
                if (weaponTransform != null)
                {
                    currentWeapon = weaponTransform.GetComponent<Weapon>();

                    if (currentWeapon != null)
                    {
                        // 무기 활성화
                        currentWeapon.gameObject.SetActive(true);

                        // 필요한 무기 타입에 따라 처리
                        if (currentWeapon is RangeWeapon rangeWeapon)
                        {
                            // 원거리 무기 관련 처리
                        }
                        else if (currentWeapon is CloseWeapon closeWeapon)
                        {
                            // 근접 무기 관련 처리
                        }

                        Debug.Log(weaponName + " 무기 활성화 완료.");
                    }
                    else
                    {
                        Debug.LogWarning("Weapon 컴포넌트를 찾을 수 없습니다.");
                    }
                }
                else
                {
                    Debug.LogWarning(weaponName + "을(를) Holder에서 찾을 수 없습니다.");
                }

                break;
            }
        }
    }

    // 무기 애니메이션 트리거 설정
    //public static void SetWeaponTrigger(string triggerName)
    //{
    //    if (currentWeapon != null && currentWeapon.anim != null)
    //    {
    //        currentWeapon.anim.SetTrigger(triggerName);
    //    }
    //}
}
