using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum VehicleType{Tank, Helicopter }
public class Vehicle : MonoBehaviour
{
    public VehicleType vehicleType;
    public static Vehicle currentVehicle; // 현재 탑승 중인 차량을 추적하기 위한 전역 변수

    public bool isPlayerInRange;
    public bool playerOnBoard;
    public Transform dropOffPoint;
    public GameObject vehicleCamera;
    public GameObject player;

    public float moveSpeed = 10f;
    public float turnSpeed = 100f;
    public float hoverSpeed = 5f; // 헬리콥터의 상승/하강 속도

    private Rigidbody rb;
    public Rigidbody turretRb;
    public GameObject bulletPrefab;
    public Transform createBulletPos;

   // 민감도
   [SerializeField]
    private float lookSensitivity;


    // 카메라 한계
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

        // 현재 차량이 이 차량인 경우에만 움직임 처리
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
        Vehicle.currentVehicle = this; // 플레이어가 이 차량에 탑승 중임을 기록
        player.transform.parent = dropOffPoint;
        player.SetActive(false);
        player.transform.localPosition = Vector3.zero;

        vehicleCamera.SetActive(true);
    }

    public void ExitVehicle()
    {
        playerOnBoard = false;
        Vehicle.currentVehicle = null; // 플레이어가 차량에서 내렸음을 기록
        player.transform.parent = null;
        player.SetActive(true);
        vehicleCamera.SetActive(false);
    }

    private void HandleVehicleMovement()
    {
        //일반차량 : 기본적인 움직임
        float move = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float turn = Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime;

        Vector3 movement = transform.forward * move;
        rb.MovePosition(rb.position + movement);
        turretRb.MovePosition(rb.position + movement);

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        if (vehicleType == VehicleType.Tank) // 전차 : 차량과 포탑 별개 움직임, 포탄 발사
        {
            HandleTankTurret();
        }
        
        if (vehicleType == VehicleType.Helicopter) //헬기: 상승 가능, 포탄 발사
        {
            // 마우스 Y축 입력에 따라 상승 또는 하강
            float verticalMove = Input.GetAxis("Jump") * hoverSpeed * Time.deltaTime;
            Vector3 verticalMovement = transform.up * verticalMove;
            rb.MovePosition(rb.position + verticalMovement);
        }
    }

    private void HandleTankTurret() // 전차 포탑 움직임 구현
    {
        // 포탑 좌우 회전
        float _yRotation = Input.GetAxisRaw("Mouse X");
        float _cameraRotationY = _yRotation * lookSensitivity;
        currentCameraRotationY -= _cameraRotationY;
        currentCameraRotationY = Mathf.Clamp(currentCameraRotationY, -cameraRotationLimitY, cameraRotationLimitY);

        //포탑 위아래 회전
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimitX, cameraRotationLimitX);

        //포탑 회전값 리지드바디에 적용
        turretRb.MoveRotation(rb.rotation * Quaternion.Euler(new Vector3(currentCameraRotationX, currentCameraRotationY, 0f)));
    }

    private void Fire()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            GameObject bullet = Instantiate(bulletPrefab, createBulletPos.transform.position, createBulletPos.transform.rotation); // 포탄 프리팹 생성
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            float bulletForce = 500f;
            bulletRb.AddForce(createBulletPos.transform.forward * bulletForce, ForceMode.Impulse);
        }
    }
}
