using UnityEngine;
using System.Collections;

public class SniperGun : RangeWeapon
{
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private GameObject scope;

    public float normalFOV = 60f;
    public float aimFOV = 30f;

    // �ݵ��� ���� ������
    [SerializeField] private float recoilAmount = 2f; // �ݵ� ����
    [SerializeField] private float recoilDuration = 0.1f; // �ݵ� �ð�

    private void Start()
    {
        // �������� �ѱ� Ưȭ ����
        accuracy = 0.95f; // ���÷� ��Ȯ���� ���� ����
        fireRate = 1f; // ���� ���� �ӵ�
        reloadTime = 3f; // �� ������ �ð�
        // �ٸ� �������� Ưȭ ����
    }

    protected override void Shoot()
    {
        base.Shoot(); // �⺻ �߻� ��� ȣ��
        StartCoroutine(CameraRecoil()); // ī�޶� �ݵ� ����
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
            scope.SetActive(true); // ������ �ѱ�
        }
        else
        {
            scope.SetActive(false); // ������ ����
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
        // ���� ī�޶� ȸ�� �� ����
        Vector3 originalRotation = theCam.transform.localEulerAngles;

        // �ݵ� ���� ī�޶� ���� �̵�
        float elapsedTime = 0f;
        while (elapsedTime < recoilDuration)
        {
            // X���� �������� �������� ī�޶� ȸ�� (�ݵ�)
            float recoilOffset = Mathf.Lerp(0, recoilAmount, elapsedTime / recoilDuration);
            theCam.transform.localEulerAngles = new Vector3(-recoilOffset, originalRotation.y, originalRotation.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // �ݵ��� ������ ���� ������ ���ƿ�
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

        // ������ ���� ��ġ�� ����
        theCam.transform.localEulerAngles = originalRotation;
    }

    // �߰����� �������� Ưȭ �޼��峪 �Ӽ�
}

