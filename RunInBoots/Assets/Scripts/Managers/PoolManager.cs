using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoolManager : MonoSingleton<PoolManager>
{

    private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

    // awake listener
    protected override void Awake()
    {
        base.Awake();
        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) => { pools.Clear(); };
    }

    // Create a new pool for a specific prefab.
    private void CreatePool(string key, GameObject prefab)
    {
        if (!pools.ContainsKey(key))
        {
            ObjectPool newPool = new ObjectPool(prefab);
            pools.Add(key, newPool);
        }
        else
        {
            Debug.LogWarning($"Pool for key {key} already exists!");
        }
    }

    public GameObject Pool(GameObject prefab, Vector3 position)
    {
        return Pool(prefab, position, Quaternion.identity, null);
    }

    public GameObject Pool(GameObject prefab, Vector3 position, Transform parent)
    {
        return Pool(prefab, position, Quaternion.identity, parent);
    }

    // Get an object from the pool by key.
    public GameObject Pool(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        string key = prefab.name;
        if (pools.TryGetValue(key, out ObjectPool pool))
        {
            return pool.Instantiate(position, rotation);
        }
        else
        {
            // create a pool for the prefab
            if (prefab != null)
            {
                CreatePool(key, prefab);
                if (parent != null)
                {
                    return pools[key].Instantiate(position, rotation, parent);
                }
                else
                {
                    return pools[key].Instantiate(position, rotation);
                }
            }
            else
            {
                Debug.LogError("Prefab is null!");
                return null;
            }
        }
    }

    //public GameObject Pool(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    //{
    //    string key = prefab.name;

    //    if (!pools.ContainsKey(key))
    //    {
    //        // Pool이 없으면 생성
    //        CreatePool(key, prefab);
    //    }

    //    // Pool에서 오브젝트 가져오기
    //    return pools[key].Instantiate(position, rotation, parent);
    //}

    // Return an object to the pool.
    // public void Destroy(GameObject obj)
    // {
    //     string key = obj.name;
    //     if (pools.TryGetValue(key, out ObjectPool pool))
    //     {
    //         pool.Destroy(obj);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("No pool for object!");
    //         Destroy(obj);
    //     }
    // }
}