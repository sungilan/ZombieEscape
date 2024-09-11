using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour , ISubject
{

    private List<IObserver> observers = new List<IObserver>(); // 옵저버 목록

    // 체력
    public int hp; //최대체력
    public int currentHp; //현재체력

    // 마나
    public int mp; //최대체력
    public int currentMp; //현재체력

    // 스태미나
    public int sp; //최대스태미나
    public int currentSp; //현재스태미나

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
    public int dp; //최대방어력
    public int currentDp; //현재방어력

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

    private const int HP = 0, DP = 1, SP = 2, HUNGRY = 3, THIRSTY = 4, SATISFY = 5;

    public void RegisterObserver(IObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }

    public void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.OnNotify();
        }
    }

    // Use this for initialization
    void Start () 
    {
        currentHp = hp;
        currentDp = dp;
        currentSp = sp;
        currentHungry = hungry;
        currentThirsty = thirsty;
        currentSatisfy = satisfy;
    }
	
	// Update is called once per frame
	void Update () 
    {
        /*Hungry();
        Thirsty();*/
        SPRechargeTime();
        SPRecover();
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


    public void IncreaseHP(int _count)
    {
        if (currentHp + _count < hp)
            currentHp += _count;
        else
            currentHp = hp;
    }
    public void DecreaseHP(int _count)
    {
        if (!isDead)
        {
            if (currentDp > 0)
            {
                DecreaseDP(_count);
                return;
            }
            currentHp -= _count;

            if (currentHp <= 0)
            {
                currentHp = 0;
                isDead = true;
            }
        }

        NotifyObservers(); // HP가 변경될 때마다 옵저버에게 알림
    }

     public void IncreaseSP(int _count)
    {
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
}