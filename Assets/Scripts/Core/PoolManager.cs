
using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    Dictionary<GameObject, Queue<GameObject>> pools = new();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        if (pools[prefab].Count == 0)
            return Instantiate(prefab, pos, rot);

        GameObject go = pools[prefab].Dequeue();
        go.transform.SetPositionAndRotation(pos, rot);
        go.SetActive(true);
        return go;
    }

    public void Despawn(GameObject prefab, GameObject obj)
    {
        obj.SetActive(false);
        pools[prefab].Enqueue(obj);
    }
}
