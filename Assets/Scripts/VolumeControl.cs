using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public static VolumeControl instance;

    [Header("음악 설정")]
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] Slider musicVolumeSlider;

    [Header("효과음 설정")]
    [SerializeField] AudioSource sfxPlayer;
    [SerializeField] Slider sfxVolumeSlider;

    [Header("마스터 설정")]
    [SerializeField] Slider masterVolumeSlider;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // 슬라이더의 값을 변경할 때마다 볼륨 조절 함수 호출
        musicVolumeSlider.onValueChanged.AddListener(delegate { OnMusicVolumeChanged(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { OnSFXVolumeChanged(); });
        masterVolumeSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChanged(); });

        // 초기에 각각의 볼륨을 슬라이더의 초기값으로 설정
        musicPlayer.volume = musicVolumeSlider.value;
        sfxPlayer.volume = sfxVolumeSlider.value;
    }

    // 음악 볼륨 조절 함수
    void OnMusicVolumeChanged()
    {
        musicPlayer.volume = musicVolumeSlider.value * masterVolumeSlider.value;
    }

    // 효과음 볼륨 조절 함수
    void OnSFXVolumeChanged()
    {
        sfxPlayer.volume = sfxVolumeSlider.value * masterVolumeSlider.value;
    }

    // 마스터 볼륨 조절 함수
    void OnMasterVolumeChanged()
    {
        // 마스터 볼륨이 변경될 때마다 음악과 효과음의 볼륨을 업데이트
        musicPlayer.volume = musicVolumeSlider.value * masterVolumeSlider.value;
        sfxPlayer.volume = sfxVolumeSlider.value * masterVolumeSlider.value;
    }
}