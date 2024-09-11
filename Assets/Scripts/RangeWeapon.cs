using System.Collections;
using UnityEngine;

public class RangeWeapon : Weapon
{
    public float accuracy; // 정확도
    public float fireRate; // 연사속도
    public float reloadTime; // 재장전 속도
    public int reloadBulletCount; // 총알 재장전 개수
    public int currentBulletCount; // 현재 탄알집에 남아있는 총알의 개수
    public int maxBulletCount; // 최대 소유 가능 총알 개수
    public int carryBulletCount; // 현재 소유하고 있는 총알 개수
    public float retroActionForce; // 반동 세기
    public float retroActionFineSightForce; // 정조준시 반동 세기
    public Vector3 fineSightOriginPos; // 정조준 시 위치
    public ParticleSystem muzzleFlash; // 총구섬광

    public Transform createCartridgePos;
    public Transform muzzlePos;


    // 활성화 여부
    public static bool isActivate = true;

    // 연사 속도 계산
    private float currentFireRate;

    // 상태 변수
    private bool isReload = false;
    [HideInInspector] public bool isFineSightMode = false;

    // 본래 포지션 값
    private Vector3 originPos;

    // 효과음 재생
    private AudioSource audioSource;
    //public GameObject hud; // Hud 불러옴

    // 레이저 충돌 정보 받아옴
    private RaycastHit hitInfo;

    // 필요한 컴포넌트
    [SerializeField] protected Camera theCam;
    [SerializeField] private Crosshair theCrosshair;

    // 피격 이펙트
    [SerializeField] private GameObject hitEffectPrefab;


    void Start()
    {
        originPos = Vector3.zero; // 초기화할 때 사용할 원래 포지션 값
        audioSource = GetComponent<AudioSource>(); // AudioSource 컴포넌트 참조
        theCrosshair = FindObjectOfType<Crosshair>(); // Crosshair 스크립트 참조

        if (theCrosshair == null)
        {
            Debug.LogError("Crosshair not found in the scene!");
        }

        // WeaponManager에서 현재 무기에 대한 정보 설정
        //WeaponManager.currentWeapon = GetComponent<Weapon>();
    }

    void Update()
    {
        if (!Inventory.inventoryActivated && !CraftManual.CraftisActivated && !DialogueSystem.isDialogue)
        {
            if (isActivate)
            {
                UseWeapon();
                GunFireRateCalc();
                TryReload();
                TryFineSight();
            }
        }
    }

    // 연사속도 재계산
    private void GunFireRateCalc()
    {
        if (currentFireRate > 0)
            currentFireRate -= Time.deltaTime;
    }

    protected override void UseWeapon()
    {
        if (Input.GetButton("Fire1") && currentFireRate <= 0 && !isReload)
        {
            Fire();
        }
    }

    // 발사 전 계산
    private void Fire()
    {
        if (!isReload)
        {
            if (currentBulletCount > 0)
                Shoot();
            else
            {
                CancelFineSight();
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    // 발사 후 계산
    protected virtual void Shoot()
    {
        anim.SetTrigger("Attack"); // 공격 애니메이션 트리거 실행
        theCrosshair.FireAnimation(); // 크로스헤어의 발사 애니메이션 실행
        currentBulletCount--; // 남은 총알 감소
        currentFireRate = fireRate; // 연사 속도 재계산
        SoundManager.Instance.PlaySE(attackSound); // 발사 사운드 재생
        muzzleFlash.Play(); // 총구 화염 이펙트 재생
        //SpawnBullet();
        SpawnCartridgeCase();
        Hit(); // 총알이 명중한 경우 처리를 위해 Hit 함수 호출
        StopAllCoroutines(); // 모든 코루틴 중지
        StartCoroutine(RetroActionCoroutine()); // 반동 코루틴 실행
    }

    private void SpawnBullet()
    {
        GameObject bullet = ObjectPoolManager.Instance.GetObject("Bullet");

        if (bullet != null)
        {
            bullet.transform.position = muzzlePos.transform.position;

            if (Physics.Raycast(theCam.transform.position, theCam.transform.forward, out hitInfo))
            {
                Vector3 direction = (hitInfo.point - muzzlePos.transform.position).normalized;
                bullet.transform.rotation = Quaternion.LookRotation(direction);
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                bulletRb.velocity = Vector3.zero;
                bulletRb.angularVelocity = Vector3.zero;
                bulletRb.AddForce(direction * 50f, ForceMode.Impulse);
            }
            else
            {
                Vector3 direction = muzzlePos.transform.forward;
                bullet.transform.rotation = Quaternion.LookRotation(direction);
                Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
                bulletRb.velocity = Vector3.zero;
                bulletRb.angularVelocity = Vector3.zero;
                bulletRb.AddForce(direction * 50f, ForceMode.Impulse);
            }
        }
    }

    private void SpawnCartridgeCase()
    {
        GameObject cartridgeCase = ObjectPoolManager.Instance.GetObject("Cartridge Case");
        cartridgeCase.transform.position = createCartridgePos.transform.position;
        Rigidbody cartridgeRb = cartridgeCase.GetComponent<Rigidbody>();
        Vector3 ejectDirection = transform.right + transform.up * 0.5f;
        float ejectForce = 1.5f;
        float ejectTorque = 5f;
        cartridgeRb.AddForce(ejectDirection * ejectForce, ForceMode.Impulse);
        cartridgeRb.AddTorque(Random.insideUnitSphere * ejectTorque, ForceMode.Impulse);
    }

    public void Hit()
    {
        if (Physics.Raycast(theCam.transform.position, theCam.transform.forward, out hitInfo, range))
        {
            //GameObject hitEffect = Instantiate(hitEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            //Destroy(hitEffect, 2f);

            if (hitInfo.transform.CompareTag("Rock"))
            {
                hitInfo.transform.GetComponent<Rock>().Mining();
            }
            else if (hitInfo.transform.CompareTag("Enemy"))
            {
                hitInfo.transform.GetComponent<Enemy>().TakeDamage(damage);
            }
        }
    }

    // 재장전 시도
    private void TryReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload && currentBulletCount < reloadBulletCount)
        {
            CancelFineSight();
            StartCoroutine(ReloadCoroutine());
        }
    }

    // 재장전
    IEnumerator ReloadCoroutine()
    {
        if (carryBulletCount > 0)
        {
            isReload = true;

            anim.SetTrigger("Reload");
            SoundManager.Instance.PlaySE("재장전");

            yield return new WaitForSeconds(reloadTime);

            if (carryBulletCount >= reloadBulletCount)
            {
                int requireBullet = reloadBulletCount - currentBulletCount;
                currentBulletCount += requireBullet;
                carryBulletCount -= requireBullet;
            }
            else
            {
                currentBulletCount = carryBulletCount;
                carryBulletCount = 0;
            }

            isReload = false;
        }
        else
        {
            Debug.Log("소유한 총알이 없습니다.");
        }
    }

    // 정조준 시도
    private void TryFineSight()
    {
        if (Input.GetButtonDown("Fire2") && !isReload)
        {
            FineSight();
        }
    }

    public void CancelFineSight()
    {
        if (isFineSightMode)
            FineSight();
    }

    protected virtual void FineSight()
    {
        isFineSightMode = !isFineSightMode;
        anim.SetBool("FineSightMode", isFineSightMode);
        //theCrosshair.FineSightAnimation(isFineSightMode);

        if (isFineSightMode)
        {
            StopAllCoroutines();
            StartCoroutine(FineSightActivateCoroutine());
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(FineSightDeactivateCoroutine());
        }
    }

    IEnumerator FineSightActivateCoroutine()
    {
        while (transform.localPosition != fineSightOriginPos)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, fineSightOriginPos, 0.2f);
            yield return null;
        }
    }

    IEnumerator FineSightDeactivateCoroutine()
    {
        while (transform.localPosition != originPos)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originPos, 0.2f);
            yield return null;
        }
    }

    IEnumerator RetroActionCoroutine()
    {
        // 반동 발생
        Vector3 recoilBack = new Vector3(0, 0, -retroActionForce);
        Vector3 recoilRotation = new Vector3(-retroActionForce, 0, 0);

        Vector3 currentPos = transform.localPosition;
        Quaternion currentRot = transform.localRotation;

        // 반동 처리
        transform.localPosition = Vector3.Lerp(transform.localPosition, recoilBack, 0.1f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(recoilRotation), 0.1f);

        yield return new WaitForSeconds(0.1f);

        transform.localPosition = currentPos;
        transform.localRotation = currentRot;
        //// 원래 위치로 되돌림
        //while (transform.localPosition != currentPos)
        //{
        //    transform.localPosition = Vector3.Lerp(transform.localPosition, currentPos, 0.1f);
        //    transform.localRotation = Quaternion.Lerp(transform.localRotation, currentRot, 0.1f);
        //    yield return null;
        //}
        //transform.localPosition = currentPos;
        //transform.localRotation = currentRot;
    }

}
