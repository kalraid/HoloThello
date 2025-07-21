using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Pool
{
    public string tag;
    public GameObject prefab;
    public int size;
}

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public void AddPool(string tag, GameObject prefab, int size)
    {
        if (poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} already exists.");
            return;
        }

        Queue<GameObject> objectPool = new Queue<GameObject>();
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
        poolDictionary.Add(tag, objectPool);
    }
    
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }
        
        Queue<GameObject> pool = poolDictionary[tag];
        
        if (pool.Count == 0)
        {
            // 풀이 비어있으면 새로 생성
            GameObject prefab = pools.Find(p => p.tag == tag)?.prefab; // Find the prefab from the list
            if (prefab == null)
            {
                Debug.LogError($"Prefab for tag {tag} not found in pools list.");
                return null;
            }
            GameObject newObj = Instantiate(prefab);
            newObj.transform.position = position;
            newObj.transform.rotation = rotation;
            newObj.SetActive(true);
            return newObj;
        }
        
        GameObject objectToSpawn = pool.Dequeue();
        
        if (objectToSpawn == null)
        {
            // 파괴된 오브젝트면 새로 생성
            GameObject prefab = pools.Find(p => p.tag == tag)?.prefab; // Find the prefab from the list
            if (prefab == null)
            {
                Debug.LogError($"Prefab for tag {tag} not found in pools list.");
                return null;
            }
            objectToSpawn = Instantiate(prefab);
        }
        
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);
        
        return objectToSpawn;
    }
    
    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return;
        }
        
        objectToReturn.SetActive(false);
        poolDictionary[tag].Enqueue(objectToReturn);
    }
    
    // 자동으로 풀로 반환하는 컴포넌트
    public void ReturnToPoolAfterDelay(string tag, GameObject obj, float delay)
    {
        StartCoroutine(ReturnToPoolCoroutine(tag, obj, delay));
    }
    
    System.Collections.IEnumerator ReturnToPoolCoroutine(string tag, GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(tag, obj);
    }
    
    // 풀 상태 확인
    public void LogPoolStatus()
    {
        foreach (var kvp in poolDictionary)
        {
            Debug.Log($"Pool '{kvp.Key}': {kvp.Value.Count} objects available");
        }
    }
    
    // 풀 정리
    public void ClearPool(string tag)
    {
        if (poolDictionary.ContainsKey(tag))
        {
            Queue<GameObject> pool = poolDictionary[tag];
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }
    }
    
    // 모든 풀 정리
    public void ClearAllPools()
    {
        foreach (var kvp in poolDictionary)
        {
            ClearPool(kvp.Key);
        }
    }
} 