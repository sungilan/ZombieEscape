using UnityEngine;

public class RifleGun : RangeWeapon
{
    private void Start()
    {
        // ���� Ưȭ ����
        accuracy = 0.75f; // ���÷� ��Ȯ�� ����
        fireRate = 0.1f; // ���� ���� �ӵ�
        reloadTime = 2f; // ��� ������ �ð�
        // �ٸ� ���� Ưȭ ����
    }

    protected override void Shoot()
    {
        base.Shoot(); // �⺻ �߻� ��� ȣ��
        // ���� Ưȭ �߻� ��� �߰�
    }

    // �߰����� ���� Ưȭ �޼��峪 �Ӽ�
}
