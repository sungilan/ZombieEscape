using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour, IObserver
{
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerLevelText;

    // 체력
    [SerializeField] private int hp; //최대체력
    private int currentHp; //현재체력

    // 스태미나
    [SerializeField] private int sp; //최대스태미나
    private int currentSp; //현재스태미나

    // 스태미나 증가량
    [SerializeField] private int spIncreaseSpeed;

    // 스태미나 재회복 딜레이.
    [SerializeField] private int spRechargeTime;
    private int currentSpRechargeTime;

    // 스태미나 감소 여부.
    private bool spUsed;
    public bool isDead; // 플레이어 사망 여부

    // 방어력
    [SerializeField]
    private int dp; //최대방어력
    private int currentDp; //현재방어력

    // 배고픔
    [SerializeField]
    private int hungry; //최대배고픔
    private int currentHungry; //현재배고픔

    // 배고픔이 줄어드는 속도.
    [SerializeField]
    private int hungryDecreaseTime;
    private int currentHungryDecreaseTime;

    // 목마름.
    [SerializeField]
    private int thirsty; //최대목마름
    private int currentThirsty; //현재목마름

    // 목마름이 줄어드는 속도.
    [SerializeField]
    private int thirstyDecreaseTime;
    private int currentThirstyDecreaseTime;

    // 만족도
    [SerializeField] private int satisfy; //최대만족도
    private int currentSatisfy; //현재만족도

    [SerializeField] private string[] sound_player_Hurt;
    [SerializeField] private string[] sound_player_Death;
    [SerializeField] private string sound_player_Drink;
    [SerializeField] private string sound_player_Read;

    // 필요한 이미지
    [SerializeField] private Image hpBarIMG;
    [SerializeField] private Image backHpBarIMG;
    [SerializeField] private GameObject PlayerHurtProfile;
    [SerializeField] private GameObject DamageScreen;
    [SerializeField] private FadeEffect damageScreenFadeEffect;
    [SerializeField] private FadeEffect gameOverFadeEffect;

    private const int HP = 0, DP = 1, SP = 2, HUNGRY = 3, THIRSTY = 4, SATISFY = 5;

    public GameObject gameOverPanel; //게임오버창

    // Use this for initialization
    void Start () 
    {
        // 옵저버 패턴 구독
        playerStatus.RegisterObserver(this);
        //playerNameText.text = DBManager.instance.playerName;
        //playerLevelText.text = DBManager.instance.playerLevel.ToString();
        currentHp = hp;
        currentDp = dp;
        currentSp = sp;
        currentHungry = hungry;
        currentThirsty = thirsty;
        currentSatisfy = satisfy;
        damageScreenFadeEffect = DamageScreen.GetComponent<FadeEffect>();
        gameOverFadeEffect = gameOverPanel.GetComponent<FadeEffect>();
    }

    // 옵저버 인터페이스 구현
    public void OnNotify()
    {
        GaugeUpdate(); // 상태 변경 시 UI 업데이트
        TakeDamage();
    }

    // Update is called once per frame
    void Update () 
    {
        /*Hungry();
        Thirsty();*/
        SPRechargeTime();
        SPRecover();
        GaugeUpdate();
	}

    private void SPRechargeTime()
    {
        if (spUsed)
        {
            if (currentSpRechargeTime < spRechargeTime)
                currentSpRechargeTime++;
            else
                spUsed = false;
        }
    }

    private void SPRecover()
    {
        if(!spUsed && currentSp < sp)
        {
            currentSp += spIncreaseSpeed;
        }
    }

    /*private void Hungry()
    {
        if (currentHungry > 0)
        {
            if (currentHungryDecreaseTime <= hungryDecreaseTime)
                currentHungryDecreaseTime++;
            else
            {
                currentHungry--;
                currentHungryDecreaseTime = 0;
            }
        }
        else
            Debug.Log("배고픔 수치가 0이 되었습니다");
    }*/

    /*private void Thirsty()
    {
        if (currentThirsty > 0)
        {
            if (currentThirstyDecreaseTime <= thirstyDecreaseTime)
                currentThirstyDecreaseTime++;
            else
            {
                currentThirsty--;
                currentThirstyDecreaseTime = 0;
            }
        }
        else
            Debug.Log("목마름 수치가 0이 되었습니다");
    }*/

    private void GaugeUpdate()
    {
        hpBarIMG.fillAmount = Mathf.Lerp(hpBarIMG.fillAmount, (float)playerStatus.currentHp / (float)playerStatus.hp, Time.deltaTime * 5f);
        StartCoroutine(UpdateBackHp());
    }
    IEnumerator UpdateBackHp()
    {
        yield return new WaitForSeconds(0.1f); // 적절한 시간 간격 설정

        float targetBackFillAmount = hpBarIMG.fillAmount;
        float initialBackFillAmount = backHpBarIMG.fillAmount;

        float elapsedTime = 0f;
        float duration = 1f; // 애니메이션 지속 시간 설정

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newBackFillAmount = Mathf.LerpUnclamped(initialBackFillAmount, targetBackFillAmount, elapsedTime / duration);
            backHpBarIMG.fillAmount = newBackFillAmount;
            yield return null;
        }

        backHpBarIMG.fillAmount = targetBackFillAmount;
    }

    private float GetFillAmount(int index)
    {
        switch (index)
        {
            case HP:
                return (float)currentHp / hp;
            case SP:
                return (float)currentSp / sp;
            case DP:
                return (float)currentDp / dp;
            case HUNGRY:
                return (float)currentHungry / hungry;
            case THIRSTY:
                return (float)currentThirsty / thirsty;
            case SATISFY:
                return (float)currentSatisfy / satisfy;
            default:
                return 0f;
        }
    }

    public void IncreaseHP(int _count)
    {
        SoundManager.Instance.PlaySE(sound_player_Drink);
        if (playerStatus.currentHp + _count < hp)
            playerStatus.currentHp += _count;
        else
            playerStatus.currentHp = hp;
    }
    public void TakeDamage()
    {
        if (!playerStatus.isDead)
        {
            int _random = Random.Range(0, sound_player_Hurt.Length);

            SoundManager.Instance.PlaySE(sound_player_Hurt[_random]);
            StartCoroutine(CountHurtimage());
        }
        else
        {
            int _random = Random.Range(0, sound_player_Death.Length);
            SoundManager.Instance.PlaySE(sound_player_Death[_random]);
            StartCoroutine(GameOver());
        }
    }
    IEnumerator CountHurtimage()
    {
    PlayerHurtProfile.gameObject.SetActive(true);

    // 페이드 인 효과를 시작합니다.
    damageScreenFadeEffect.StartFadeIn();
    yield return new WaitForSeconds(damageScreenFadeEffect.fadeTime); // 페이드 아웃 시간만큼 대기합니다.

    // 페이드 아웃 효과를 시작합니다.
    damageScreenFadeEffect.StartFadeOut();
    yield return new WaitForSeconds(damageScreenFadeEffect.fadeTime);

    PlayerHurtProfile.gameObject.SetActive(false);
    }

     public void IncreaseSP(int _count)
    {
        SoundManager.Instance.PlaySE(sound_player_Read);
        if (currentSp + _count < sp)
            currentSp += _count;
        else
            currentSp = sp;
    }

    public void IncreaseDP(int _count)
    {
        if (currentDp + _count < hp)
            currentDp += _count;
        else
            currentDp = dp;
    }

    public void DecreaseDP(int _count)
    {
        currentDp -= _count;

        if (currentDp <= 0)
            Debug.Log("방어력이 0이 되었습니다!!");
    }

    public void IncreaseHungry(int _count)
    {
        if (currentHungry + _count < hungry)
            currentHungry += _count;
        else
            currentHungry = hungry;
    }

    public void DecreaseHungry(int _count)
    {
        if (currentHungry - _count < 0)
            currentHungry = 0;
        else
            currentHungry -= _count;
    }

    public void IncreaseThirsty(int _count)
    {
        if (currentThirsty + _count < thirsty)
            currentThirsty += _count;
        else
            currentThirsty = thirsty;
    }

    public void DecreaseThirsty(int _count)
    {
        if (currentThirsty - _count < 0)
            currentThirsty = 0;
        else
            currentThirsty -= _count;
    }

    public void DecreaseStamina(int _count)
    {
        spUsed = true;
        currentSpRechargeTime = 0;

        if (currentSp - _count > 0)
            currentSp -= _count;
        else
            currentSp = 0;
    }

    public int GetCurrentSP()
    {
        return currentSp;
    }
    IEnumerator GameOver()
    {
        //게임오버창을 띄운다.
        gameOverPanel.gameObject.SetActive(true);
        // 페이드 인 효과를 시작합니다.
        gameOverFadeEffect.StartFadeIn();
        yield return new WaitForSeconds(gameOverFadeEffect.fadeTime);
    }
}