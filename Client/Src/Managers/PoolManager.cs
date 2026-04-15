using UnityEngine;
using System.Collections.Generic;

public interface IPool
{
    MonoBehaviour Pop();
    void Push(MonoBehaviour obj);
}
public enum PoolLayer { Monster, Skill }

public interface IPoolableObject
{
    void Activate();
}

public class Pool<T> : IPool where T : MonoBehaviour, IPoolableObject
{
    private HashSet<T> activeObjects = new HashSet<T>(); // Pop되어 사용 중인 오브젝트
    private Queue<T> availableObjects = new Queue<T>();  // 풀에서 대기 중인 오브젝트
    private T prefab;
    private Transform parent;

    public Pool(T prefab, int initialSize, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            T obj = Object.Instantiate(this.prefab, parent);
            obj.gameObject.SetActive(false);
            availableObjects.Enqueue(obj);
        }
    }

    public MonoBehaviour Pop()
    {
        T obj = availableObjects.Count > 0 ? availableObjects.Dequeue() : null;

        if (obj == null || obj.gameObject.activeInHierarchy)
            obj = Object.Instantiate(prefab, parent);

        obj.gameObject.SetActive(true);
        obj.Activate();
        activeObjects.Add(obj);
        return obj;
    }

    public void Push(MonoBehaviour obj)
    {
        if (obj is T typedObj)
        {
            if (!activeObjects.Remove(typedObj))
                return;

            typedObj.gameObject.SetActive(false);
            availableObjects.Enqueue(typedObj);
        }
    }
}


public class PoolManager : MonoBehaviour
{
    [SerializeField] private Transform monsterLayerRoot;
    [SerializeField] private Transform skillLayerRoot;

    private Dictionary<PoolType, IPool> pools = new Dictionary<PoolType, IPool>();

    public void Init() { }

    private Transform GetLayerRoot(PoolLayer layer)
        => layer == PoolLayer.Monster ? monsterLayerRoot : skillLayerRoot;

    private void CreatePool<T>(PoolType poolType, T prefab, int initialSize, PoolLayer layer) where T : MonoBehaviour, IPoolableObject
    {
        if (pools.ContainsKey(poolType)) return;

        Transform poolRoot = new GameObject(poolType.ToString()).transform;
        poolRoot.SetParent(GetLayerRoot(layer));
        pools.Add(poolType, new Pool<T>(prefab, initialSize, poolRoot));
    }

    public T Pop<T>(PoolType poolType) where T : MonoBehaviour, IPoolableObject
    {
        if (pools.TryGetValue(poolType, out IPool pool))
            return (pool as Pool<T>)?.Pop() as T;

        Debug.LogError($"{poolType} pool not found.");
        return null;
    }

    public void Push<T>(PoolType poolType, T obj) where T : MonoBehaviour, IPoolableObject
    {
        if (pools.TryGetValue(poolType, out IPool pool))
            pool.Push(obj);
    }

    public void Prewarm<T>(PoolType poolType, T prefab, int count, PoolLayer layer) where T : MonoBehaviour, IPoolableObject
    {
        if (!pools.ContainsKey(poolType))
            CreatePool(poolType, prefab, count, layer);
    }

    public T GetPoolObject<T>(PoolType poolType, T prefab, PoolLayer layer) where T : MonoBehaviour, IPoolableObject
    {
        if (!pools.ContainsKey(poolType))
            CreatePool(poolType, prefab, 1, layer);

        return Pop<T>(poolType);
    }
}
