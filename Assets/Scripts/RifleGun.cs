using UnityEngine;

public class RifleGun : RangeWeapon
{
    private void Start()
    {
        // 소총 특화 설정
        accuracy = 0.75f; // 예시로 정확도 설정
        fireRate = 0.1f; // 빠른 연사 속도
        reloadTime = 2f; // 평균 재장전 시간
        // 다른 소총 특화 설정
    }

    protected override void Shoot()
    {
        base.Shoot(); // 기본 발사 기능 호출
        // 소총 특화 발사 기능 추가
    }

    // 추가적인 소총 특화 메서드나 속성
}
