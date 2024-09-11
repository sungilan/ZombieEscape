using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [System.Serializable] 
    public class ObjectPool
    {
        public string tag;                  // 오브젝트 풀 이름
        public GameObject prefab;           // 풀링할 오브젝트 프리팹
        public int size;                    // 오브젝트 풀의 크기(풀에 들어갈 프리팹 수)
    }

    [SerializeField] private List<ObjectPool> pools; // 여러 개의 풀 리스트
    private Dictionary<string, Queue<GameObject>> poolDictionary; // 풀을 관리할 딕셔너리

    private void Awake()
    {
        Init();
    }

    // 오브젝트 풀 초기화 함수
    private void Init()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>(); //poolDictionary 초기화

        foreach (var pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.transform.SetParent(transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    //사용할 오브젝트를 오브젝트 풀에서 가져오는 함수
    public GameObject GetObject(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"풀에 {tag} 태그가 존재하지 않습니다.");
            return null;
        }

        if (poolDictionary[tag].Count > 0) //오브젝트 풀에 오브젝트가 존재할 경우
        {
            var obj = poolDictionary[tag].Dequeue(); //오브젝트 풀에서 오브젝트를 꺼냄
            obj.transform.SetParent(null); //오브젝트의 부모설정 해제
            obj.gameObject.SetActive(true); //오브젝트 활성화
            return obj;
        }
        else //오브젝트 풀에 오브젝트가 존재하지 않을 경우
        {
            var newObj = Instantiate(GetPoolPrefab(tag)); //새로운 오브젝트 생성
            newObj.transform.SetParent(null); //오브젝트의 부모설정 해제
            newObj.gameObject.SetActive(true); //오브젝트 활성화
            return newObj;
        }
    }
    
    //사용한 오브젝트를 오브젝트 풀로 다시 돌려받는 함수
    public void ReturnObject(string tag, GameObject returnObject)
    {
        if (!poolDictionary.ContainsKey(tag)) //poolDictionary에 해당하는 tag를 가진 objectPool이 없다면 리턴
        {
            Debug.LogWarning($"풀에 {tag} 태그가 존재하지 않습니다.");
            return;
        }

        Debug.Log($"Returning object {returnObject.name} to pool with tag: {tag}");
        returnObject.SetActive(false); //오브젝트 비활성화
        returnObject.transform.SetParent(transform); //오브젝트 풀을 오브젝트의 부모로 설정
        poolDictionary[tag].Enqueue(returnObject); // 오브젝트 풀에 오브젝트 반환
    }

    // 오브젝트 풀의 프리팹을 얻는 함수
    private GameObject GetPoolPrefab(string tag)
    {
        foreach (var pool in pools)
        {
            if (pool.tag == tag)
            {
                return pool.prefab;
            }
        }

        Debug.LogWarning($"태그 {tag}에 해당하는 프리팹이 없습니다.");
        return null;
    }
}
