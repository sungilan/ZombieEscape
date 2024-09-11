using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum State { Idle, Move, Attack }
public class Enemy : MonoBehaviour
{
    public State _state;
    private Transform playerTransform;

    [Header("EnemyInfo")]
    [SerializeField] private string enemyName;
    [SerializeField] private int currentHp;
    [SerializeField] private int maxHp;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    private float applySpeed;
    private Vector3 direction;

    // 상태 변수
    private bool isWalking;
    private bool isRunning;
    private bool isDead;
    private bool isStun;

    [Header("Times")]
    [SerializeField] private float walkTime;
    [SerializeField] private float runTime;
    [SerializeField] private float waitTime;
    private float currentTime;

    // 필요한 컴포넌트
    [Header("Components")]
    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rigid;
    [SerializeField] private AudioClip[] sound_Nomal;
    [SerializeField] private string[] sound_Hurt;
    [SerializeField] private string[] sound_Death;
    [SerializeField] private string sound_Attack;
    public int damage;
    private NavMeshAgent agent;
    private FieldOfViewAngle fieldOfViewAngle;

    [Header("HpImages")]
    [SerializeField] private Canvas hpBarCanvas;
    [SerializeField] private Image hpBarIMG;
    [SerializeField] private Image backHpBarIMG;

    private void Start()
    {
        _state = State.Idle;
        agent = GetComponent<NavMeshAgent>();
        currentTime = waitTime;
        anim = GetComponent<Animator>();
        fieldOfViewAngle = GetComponent<FieldOfViewAngle>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        if (playerTransform == null)
        {
            Debug.LogError("Player not found!");
        }
    }

    private void Update()
    {
        switch (_state)
        {
            case State.Idle:
                Idle();
                break;

            case State.Move:
                Move();
                break;

            case State.Attack:
                Attack();
                break;
        }

        GaugeUpdate();
    }

    private void Idle()
    {
        ElapseTime();
        if (fieldOfViewAngle.isLook && !isStun)
        {
            _state = State.Move; // 플레이어를 발견하면 이동 상태로 전환
        }
    }

    private void Move()
    {
        if (!isStun)
        {
            agent.isStopped = false;
            anim.SetBool("Walking", true);
            agent.destination = playerTransform.position;

            if (Vector3.Distance(transform.position, playerTransform.position) <= agent.stoppingDistance)
            {
                _state = State.Attack; // 공격 거리 내에 들어오면 공격 상태로 전환
            }
        }
        if(!fieldOfViewAngle.isLook)
        {
            _state = State.Idle; // 시야에 플레이어가 없으면 대기 상태로 전환
        }
    }

    private void Attack()
    {
        if (!isStun && !isDead)
        {
            StopMoving();
            PlayerStatus playerStatus = playerTransform.GetComponent<PlayerStatus>();
            if (playerStatus != null && playerStatus.currentHp > 0)
            {
                LookAtPlayer();
                StartCoroutine(PerformAttack(playerStatus));
            }    
        }

        if (Vector3.Distance(transform.position, playerTransform.position) > agent.stoppingDistance)
        {
            _state = State.Move; // 플레이어가 멀어지면 이동 상태로 전환
        }
    }
    private void LookAtPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized; // 플레이어 방향 계산
        direction.y = 0; // 수평 방향으로만 회전 (y축 회전 방지)

        Quaternion lookRotation = Quaternion.LookRotation(direction); // 회전 각도 계산
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 20f); // 부드럽게 회전
    }

    private void StopMoving()
    {
        agent.isStopped = true;
        anim.SetBool("Walking", false);
    }

    private IEnumerator PerformAttack(PlayerStatus playerStatus)
    {
        isStun = true;
        anim.SetTrigger("Attacking");
        SoundManager.Instance.PlaySE(sound_Attack);
        yield return new WaitForSeconds(1f); // 공격 딜레이
        if (Vector3.Distance(transform.position, playerTransform.position) < agent.stoppingDistance)
        {
            playerStatus.DecreaseHP(damage); // 플레이어에게 데미지
        }
        isStun = false;
        _state = State.Idle; // 공격 후 다시 대기 상태로 전환
    }

    private void ElapseTime()
    {
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            ReSetAction();
        }
    }

    private void ReSetAction()
    {
        isWalking = false;
        isRunning = false;
        applySpeed = walkSpeed;
        anim.SetBool("Walking", isWalking);
        anim.SetBool("Running", isRunning);
        direction.Set(0f, Random.Range(0f, 360f), 0f);
        RandomAction();
    }

    private void RandomAction()
    {
        int _random = Random.Range(0, 4);
        if (_random == 0) Wait();
        else if (_random == 1) TryWalk();
        else if (_random == 2) Taunt();
        else if (_random == 3) Battlecry();
    }

    private void Wait()
    {
        currentTime = waitTime;
    }

    private void TryWalk()
    {
        isWalking = true;
        anim.SetBool("Walking", isWalking);
        currentTime = walkTime;
        applySpeed = walkSpeed;
    }

    private void Taunt()
    {
        currentTime = waitTime;
        anim.SetTrigger("Taunt");
    }

    private void Battlecry()
    {
        currentTime = waitTime;
        anim.SetTrigger("Battlecry");
    }

    public void TakeDamage(int _dmg)
    {
        if (isDead) return;
        StartCoroutine(TakeDamageCorutine(_dmg));
    }

    public IEnumerator TakeDamageCorutine(int _dmg)
    {
        isStun = true;
        agent.isStopped = true;
        if (currentHp > 0)
        {
            hpBarCanvas.gameObject.SetActive(true);
            currentHp -= _dmg;
            int _random = Random.Range(0, sound_Hurt.Length);
            SoundManager.Instance.PlaySE(sound_Hurt[_random]);
            anim.SetTrigger("Hurt");
            if(currentHp <= 0) StartCoroutine(Death());
            yield return new WaitForSeconds(1f);
            isStun = false;
            agent.isStopped = false;
            hpBarCanvas.gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(Death());
        }  
    }

    private IEnumerator Death()
    {
        agent.enabled = false;
        isWalking = false;
        isRunning = false;
        isDead = true;
        int _random = Random.Range(0, sound_Death.Length);
        SoundManager.Instance.PlaySE(sound_Death[_random]);
        anim.SetTrigger("Death");
        hpBarCanvas.gameObject.SetActive(false);
        yield return new WaitForSeconds(4f);
        ObjectPoolManager.Instance.ReturnObject(enemyName, this.gameObject);
    }

    private void GaugeUpdate()
    {
        hpBarIMG.fillAmount = Mathf.Lerp(hpBarIMG.fillAmount, (float)currentHp / (float)maxHp, Time.deltaTime * 5f);
        StartCoroutine(UpdateBackHp());
    }

    IEnumerator UpdateBackHp()
    {
        yield return new WaitForSeconds(0.1f);
        float targetBackFillAmount = hpBarIMG.fillAmount;
        float initialBackFillAmount = backHpBarIMG.fillAmount;
        float elapsedTime = 0f;
        float duration = 1f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newBackFillAmount = Mathf.LerpUnclamped(initialBackFillAmount, targetBackFillAmount, elapsedTime / duration);
            backHpBarIMG.fillAmount = newBackFillAmount;
            yield return null;
        }
        backHpBarIMG.fillAmount = targetBackFillAmount;
    }
}
