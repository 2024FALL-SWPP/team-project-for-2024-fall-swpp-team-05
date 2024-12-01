using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private List<GameObject> pool = new List<GameObject>();
    private GameObject prefab;

    public ObjectPool(GameObject prefab)
    {
        this.prefab = prefab;
    }

    // Instantiate a new GameObject or reuse an inactive one from the pool.
    
    public GameObject Instantiate(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                obj.transform.SetParent(parent);
                return obj;
            }
        }
        if(prefab == null)
        {
            Debug.LogWarning("null을 Instantiate 할 수 없음");
            return null;
        }

        GameObject newObj;
        if (parent == null)
        {
            newObj = Object.Instantiate(prefab, position, rotation);
        }
        else
        {
            newObj = Object.Instantiate(prefab, position, rotation, parent);
        }

        pool.Add(newObj);
        return newObj;
    }

    // Disable the GameObject and return it to the pool.
    public void Destroy(GameObject obj)
    {
        if (pool.Contains(obj))
        {
            obj.SetActive(false);
        }
        else
        {
            Debug.LogWarning("The object is not part of the pool!");
        }
    }
}