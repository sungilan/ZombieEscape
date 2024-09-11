using UnityEngine;

public class FieldOfViewAngle : MonoBehaviour 
{

    [SerializeField] private float viewAngle = 120; // 시야각 (120도);
    [SerializeField] private float viewDistance = 10; // 시야거리 (10미터);
    [SerializeField] private LayerMask targetMask; // 타겟 마스크 (플레이어)

    private Enemy theEnemy;

    public bool isLook = false;

    void Start()
    {
        theEnemy = GetComponent<Enemy>();
    }

	// Update is called once per frame
	void Update () 
    {
        View();
	}

    private Vector3 BoundaryAngle(float _angle)
    {
        _angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(_angle * Mathf.Deg2Rad), 0f, Mathf.Cos(_angle * Mathf.Deg2Rad));
    }

    //시각적 가이드라인 그려주는 이벤트 함수
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
    private void View()
    {
        isLook = false;
        Vector3 _leftBoundary = BoundaryAngle(-viewAngle * 0.5f);
        Vector3 _rightBoundary = BoundaryAngle(viewAngle * 0.5f);

        Collider[] _target = Physics.OverlapSphere(transform.position, viewDistance); //시야 안에 있는 모든 개체 저장(targetMask만)
        //Debug.Log("target count : " + _target.Length);

        for (int i = 0; i < _target.Length; i++)
        {
            Transform _targetTf = _target[i].transform;
            if(_target[i].tag == "Player") // 타겟의 이름이 Player라면
            {
                isLook = true;
            }
        }
    }
}