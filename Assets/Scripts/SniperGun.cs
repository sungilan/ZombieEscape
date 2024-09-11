using UnityEngine;
using System.Collections;

public class SniperGun : RangeWeapon
{
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private GameObject scope;

    public float normalFOV = 60f;
    public float aimFOV = 30f;

    // 반동을 위한 변수들
    [SerializeField] private float recoilAmount = 2f; // 반동 강도
    [SerializeField] private float recoilDuration = 0.1f; // 반동 시간

    private void Start()
    {
        // 스나이퍼 총기 특화 설정
        accuracy = 0.95f; // 예시로 정확도를 높게 설정
        fireRate = 1f; // 느린 연사 속도
        reloadTime = 3f; // 긴 재장전 시간
        // 다른 스나이퍼 특화 설정
    }

    protected override void Shoot()
    {
        base.Shoot(); // 기본 발사 기능 호출
        StartCoroutine(CameraRecoil()); // 카메라 반동 실행
        CancelFineSight();
    }

    protected override void FineSight()
    {
        base.FineSight();
        StartCoroutine(AimCoroutine());
    }

    IEnumerator AimCoroutine()
    {
        if (isFineSightMode)
        {
            scope.SetActive(true); // 스코프 켜기
        }
        else
        {
            scope.SetActive(false); // 스코프 끄기
        }

        float currentFOV = theCam.fieldOfView;
        float targetFOV = isFineSightMode ? aimFOV : normalFOV;

        while (Mathf.Abs(currentFOV - targetFOV) > 0.1f)
        {
            currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * zoomSpeed);
            theCam.fieldOfView = currentFOV;
            yield return null;
        }
    }

    IEnumerator CameraRecoil()
    {
        // 원래 카메라 회전 값 저장
        Vector3 originalRotation = theCam.transform.localEulerAngles;

        // 반동 동안 카메라를 위로 이동
        float elapsedTime = 0f;
        while (elapsedTime < recoilDuration)
        {
            // X축을 기준으로 위쪽으로 카메라를 회전 (반동)
            float recoilOffset = Mathf.Lerp(0, recoilAmount, elapsedTime / recoilDuration);
            theCam.transform.localEulerAngles = new Vector3(-recoilOffset, originalRotation.y, originalRotation.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 반동이 끝나면 원래 각도로 돌아옴
        elapsedTime = 0f;
        while (elapsedTime < recoilDuration)
        {
            theCam.transform.localEulerAngles = Vector3.Lerp(
                theCam.transform.localEulerAngles,
                originalRotation,
                elapsedTime / recoilDuration
            );

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 완전히 원래 위치로 복귀
        theCam.transform.localEulerAngles = originalRotation;
    }

    // 추가적인 스나이퍼 특화 메서드나 속성
}

