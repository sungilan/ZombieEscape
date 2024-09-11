using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;

    Vector3 lookVec;
    Vector3 tauntVec;
    public bool isLook;
    private Rigidbody rb;
    private BoxCollider boxCollider;
    private NavMeshAgent nav;
    private Animator anim;
    private bool isDead;
    [SerializeField] private Transform target;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true;
        StartCoroutine(Think());
    }

    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);
        }
        else
            nav.SetDestination(tauntVec);  
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);
        int ranAction = Random.Range(0, 4);
        switch (ranAction)
        {
            case 0:
                StartCoroutine(Magnet());
                break;
            case 1:
                StartCoroutine(Slash());
                break;
            case 2:
                StartCoroutine(Meteo());
                break;
            case 3:
                StartCoroutine(Hurricane());
                break;
        }
    }
    IEnumerator Magnet()
    {
        anim.SetTrigger("Magnet");
        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());
    }
    IEnumerator Slash()
    {
        isLook = false;
        anim.SetTrigger("Slash");
        yield return new WaitForSeconds(3f);
        isLook = true;

        StartCoroutine(Think());
    }
    IEnumerator Meteo()
    {
        tauntVec = target.position + lookVec;

        anim.SetTrigger("Meteo");
        yield return new WaitForSeconds(1f);
   
        StartCoroutine(Think());
    }

    IEnumerator Hurricane()
    {
        anim.SetTrigger("Hurricane");
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(Think());
    }
}