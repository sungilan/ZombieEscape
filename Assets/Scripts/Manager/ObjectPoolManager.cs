using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    [System.Serializable] 
    public class ObjectPool
    {
        public string tag;                  // ������Ʈ Ǯ �̸�
        public GameObject prefab;           // Ǯ���� ������Ʈ ������
        public int size;                    // ������Ʈ Ǯ�� ũ��(Ǯ�� �� ������ ��)
    }

    [SerializeField] private List<ObjectPool> pools; // ���� ���� Ǯ ����Ʈ
    private Dictionary<string, Queue<GameObject>> poolDictionary; // Ǯ�� ������ ��ųʸ�

    private void Awake()
    {
        Init();
    }

    // ������Ʈ Ǯ �ʱ�ȭ �Լ�
    private void Init()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>(); //poolDictionary �ʱ�ȭ

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

    //����� ������Ʈ�� ������Ʈ Ǯ���� �������� �Լ�
    public GameObject GetObject(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Ǯ�� {tag} �±װ� �������� �ʽ��ϴ�.");
            return null;
        }

        if (poolDictionary[tag].Count > 0) //������Ʈ Ǯ�� ������Ʈ�� ������ ���
        {
            var obj = poolDictionary[tag].Dequeue(); //������Ʈ Ǯ���� ������Ʈ�� ����
            obj.transform.SetParent(null); //������Ʈ�� �θ��� ����
            obj.gameObject.SetActive(true); //������Ʈ Ȱ��ȭ
            return obj;
        }
        else //������Ʈ Ǯ�� ������Ʈ�� �������� ���� ���
        {
            var newObj = Instantiate(GetPoolPrefab(tag)); //���ο� ������Ʈ ����
            newObj.transform.SetParent(null); //������Ʈ�� �θ��� ����
            newObj.gameObject.SetActive(true); //������Ʈ Ȱ��ȭ
            return newObj;
        }
    }
    
    //����� ������Ʈ�� ������Ʈ Ǯ�� �ٽ� �����޴� �Լ�
    public void ReturnObject(string tag, GameObject returnObject)
    {
        if (!poolDictionary.ContainsKey(tag)) //poolDictionary�� �ش��ϴ� tag�� ���� objectPool�� ���ٸ� ����
        {
            Debug.LogWarning($"Ǯ�� {tag} �±װ� �������� �ʽ��ϴ�.");
            return;
        }

        Debug.Log($"Returning object {returnObject.name} to pool with tag: {tag}");
        returnObject.SetActive(false); //������Ʈ ��Ȱ��ȭ
        returnObject.transform.SetParent(transform); //������Ʈ Ǯ�� ������Ʈ�� �θ�� ����
        poolDictionary[tag].Enqueue(returnObject); // ������Ʈ Ǯ�� ������Ʈ ��ȯ
    }

    // ������Ʈ Ǯ�� �������� ��� �Լ�
    private GameObject GetPoolPrefab(string tag)
    {
        foreach (var pool in pools)
        {
            if (pool.tag == tag)
            {
                return pool.prefab;
            }
        }

        Debug.LogWarning($"�±� {tag}�� �ش��ϴ� �������� �����ϴ�.");
        return null;
    }
}
