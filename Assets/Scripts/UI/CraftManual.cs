using System.Collections;
using UnityEngine;

[System.Serializable]
public class Craft
{
    public string craftName; // 건축물 이름
    public GameObject go_Prefab; // 건축물 프리팹
    public GameObject go_PreviewPrefab; // 건축물 미리보기 프리팹
    public int buildTime;
}
public class CraftManual : MonoBehaviour
{
    [SerializeField] private string sound_Build;
    [SerializeField] private string sound_Craft_Open;

    //상태변수
    public static bool CraftisActivated = false;
    private bool isPreviewActivated = false;

    [Header("Components")]
    [SerializeField] private GameObject go_BaseUI; // 기본 베이스 UI
    [SerializeField] private Craft[] craft_fire; // 모닥불용 탭
    private GameObject go_Preview; // 미리보기 프리팹을 담을 변수
    private GameObject go_Prefab; // 실제 프리팹을 담을 변수
    private int currentBuildTime;
    [SerializeField] private Transform tf_Player; // 플레이어 위치
    [SerializeField] private Camera playerCamera; // 플레이어 카메라

    //Raycast 필요 변수 선언
    private RaycastHit hitInfo;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float range;
    private WeaponManager weaponManager;

void Start () 
    {
        weaponManager = FindObjectOfType<WeaponManager>();
	}
void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated)
        {
            Window();
        }

        if(isPreviewActivated)
        {
            PreviewPositionUpdate();
        }
      
        if(Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(Build(currentBuildTime));
        }
            
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
        }
        
    }

    public void SlotClick(int _slotNumber)
    {
        go_Preview = Instantiate(craft_fire[_slotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);
        go_Prefab = craft_fire[_slotNumber].go_Prefab;
        currentBuildTime = craft_fire[_slotNumber].buildTime;
        isPreviewActivated = true;
        go_BaseUI.SetActive(false);
    }
    
    private void PreviewPositionUpdate()
    {
        Vector3 crosshairPosition = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
        go_Preview.transform.position = new Vector3(crosshairPosition.x, tf_Player.position.y, crosshairPosition.z);
    }

    private Vector3 GetPreviewPosition()
    {
        Vector3 crosshairPosition = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
        return new Vector3(crosshairPosition.x, tf_Player.position.y, crosshairPosition.z);
    }
    
    private IEnumerator Build(int buildTime)
    {
        if (isPreviewActivated)
        {
            //WeaponManager.SetWeaponTrigger("Attack");
            SoundManager.Instance.PlaySE(sound_Build);
            isPreviewActivated = false;
            yield return new WaitForSeconds(buildTime);
            Destroy(go_Preview);
            Instantiate(go_Prefab, GetPreviewPosition(), Quaternion.identity);
            weaponManager.ChangeWeapon("주먹");
            go_Preview = null;
            go_Prefab = null;
            CraftisActivated = false;
        }
    }

    
    private void Window() 
    {
        if(!CraftisActivated)
        OpenWindow();
        else
        CloseWindow();
    }
    private void OpenWindow()
    {
        SoundManager.Instance.PlaySE(sound_Craft_Open);
        weaponManager.ChangeWeapon("건설");
        CraftisActivated = true;
        go_BaseUI.SetActive(true);
    }
    private void CloseWindow()
    {
        SoundManager.Instance.PlaySE(sound_Craft_Open);
        weaponManager.ChangeWeapon("주먹");
        CraftisActivated = false;
        go_BaseUI.SetActive(false);
    }
     private void Cancel()
    {
        CloseWindow();
        if (isPreviewActivated) // isPreviewActivated가 true면
        Destroy(go_Preview);
        isPreviewActivated = false;
        go_Preview = null;
        go_Prefab = null;
    }
}