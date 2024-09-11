using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GrenadeType {Timer, Collison}
public class Grenade : MonoBehaviour
{
    [SerializeField] private GrenadeType gradeType;
    [SerializeField] private GameObject meshObj;
    [SerializeField] private GameObject effectObj;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private int damage;
    [SerializeField] private int explosionTime;
    [SerializeField] private float explosionRadius;
    [SerializeField] private string explosionSound;
    // Start is called before the first frame update
    void Start()
    {
        if(gradeType == GrenadeType.Timer)
        {
            StartCoroutine(Explosion());
        }
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(explosionTime);
        SoundManager.Instance.PlaySE(explosionSound);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, explosionRadius, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        foreach (RaycastHit hitObj in raycastHits)
        {
            hitObj.transform.GetComponent<Enemy>().TakeDamage(damage);
        }
        Destroy(gameObject, explosionTime);

    }

    // 기즈모로 폭발 반경을 표시
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (gradeType == GrenadeType.Collison)
        {
            
                SoundManager.Instance.PlaySE(explosionSound);
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                meshObj.SetActive(false);
                effectObj.SetActive(true);

                RaycastHit[] raycastHits = Physics.SphereCastAll(transform.position, explosionRadius, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
                foreach (RaycastHit hitObj in raycastHits)
                {
                    hitObj.transform.GetComponent<Enemy>().TakeDamage(damage);
                }
                Destroy(gameObject, explosionTime);
            
        }
    }
}
