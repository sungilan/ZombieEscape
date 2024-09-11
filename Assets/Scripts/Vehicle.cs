using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum VehicleType{Tank, Helicopter }
public class Vehicle : MonoBehaviour
{
    public VehicleType vehicleType;
    public static Vehicle currentVehicle; // ���� ž�� ���� ������ �����ϱ� ���� ���� ����

    public bool isPlayerInRange;
    public bool playerOnBoard;
    public Transform dropOffPoint;
    public GameObject vehicleCamera;
    public GameObject player;

    public float moveSpeed = 10f;
    public float turnSpeed = 100f;
    public float hoverSpeed = 5f; // �︮������ ���/�ϰ� �ӵ�

    private Rigidbody rb;
    public Rigidbody turretRb;
    public GameObject bulletPrefab;
    public Transform createBulletPos;

   // �ΰ���
   [SerializeField]
    private float lookSensitivity;


    // ī�޶� �Ѱ�
    [SerializeField] private float cameraRotationLimitY;
    [SerializeField] private float cameraRotationLimitX;
    private float currentCameraRotationY = 0;
    private float currentCameraRotationX = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OnInteraction();
        }

        // ���� ������ �� ������ ��쿡�� ������ ó��
        if (playerOnBoard && Vehicle.currentVehicle == this)
        {
            HandleVehicleMovement();
            Fire();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    public void OnInteraction()
    {
        if (!playerOnBoard)
        {
            EnterVehicle();
        }
        else
        {
            ExitVehicle();
        }
    }

    public void EnterVehicle()
    {
        playerOnBoard = true;
        Vehicle.currentVehicle = this; // �÷��̾ �� ������ ž�� ������ ���
        player.transform.parent = dropOffPoint;
        player.SetActive(false);
        player.transform.localPosition = Vector3.zero;

        vehicleCamera.SetActive(true);
    }

    public void ExitVehicle()
    {
        playerOnBoard = false;
        Vehicle.currentVehicle = null; // �÷��̾ �������� �������� ���
        player.transform.parent = null;
        player.SetActive(true);
        vehicleCamera.SetActive(false);
    }

    private void HandleVehicleMovement()
    {
        //�Ϲ����� : �⺻���� ������
        float move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float turn = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;

        Vector3 movement = transform.forward * move;
        rb.MovePosition(rb.position + movement);
        turretRb.MovePosition(rb.position + movement);

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        if (vehicleType == VehicleType.Tank) // ���� : ������ ��ž ���� ������, ��ź �߻�
        {
            HandleTankTurret();
        }
        
        if (vehicleType == VehicleType.Helicopter) //���: ��� ����, ��ź �߻�
        {
            // ���콺 Y�� �Է¿� ���� ��� �Ǵ� �ϰ�
            float verticalMove = Input.GetAxis("Jump") * hoverSpeed * Time.deltaTime;
            Vector3 verticalMovement = transform.up * verticalMove;
            rb.MovePosition(rb.position + verticalMovement);
        }
    }

    private void HandleTankTurret() // ���� ��ž ������ ����
    {
        // ��ž �¿� ȸ��
        float _yRotation = Input.GetAxisRaw("Mouse X");
        float _cameraRotationY = _yRotation * lookSensitivity;
        currentCameraRotationY -= _cameraRotationY;
        currentCameraRotationY = Mathf.Clamp(currentCameraRotationY, -cameraRotationLimitY, cameraRotationLimitY);

        //��ž ���Ʒ� ȸ��
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimitX, cameraRotationLimitX);

        //��ž ȸ���� ������ٵ� ����
        turretRb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(currentCameraRotationX, currentCameraRotationY, 0f)));
    }

    private void Fire()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            GameObject bullet = Instantiate(bulletPrefab, createBulletPos.transform.position, createBulletPos.transform.rotation); // ��ź ������ ����
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            float bulletForce = 500f;
            bulletRb.AddForce(createBulletPos.transform.forward * bulletForce, ForceMode.Impulse);
        }
    }
}
