using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    
    [SerializeField] private bool _addToDontDestroyOnLoad = false;

    private GameObject _emptyHolder;

    private static GameObject _projectileEmpty;
    private static GameObject _shadowCasterEmpty;

    private static Dictionary<GameObject, ObjectPool<GameObject>> _objectPools;
    private static Dictionary<GameObject, GameObject> _cloneToPrefabMap;

    public enum PoolType
    {
        Projectiles,
        GameObjects
    }
    public static PoolType PoolingType;

    private void Awake()
    {
        _objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
        _cloneToPrefabMap = new Dictionary<GameObject, GameObject>();

        SetupEmpties();
    }

    private void SetupEmpties()
    {
        _emptyHolder = this.gameObject;
        _projectileEmpty = new GameObject("Projectiles");
        _projectileEmpty.transform.SetParent(_emptyHolder.transform, false);

        _shadowCasterEmpty = new GameObject("ShadowCaster");
        _shadowCasterEmpty.transform.SetParent(_emptyHolder.transform, false);

        if (_addToDontDestroyOnLoad)
            DontDestroyOnLoad(_projectileEmpty.transform.root);

    }

    private static void CreatePool(GameObject prefab,Vector3 pos,Quaternion rot,PoolType poolType = PoolType.GameObjects)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc:() => CreateObject(prefab,pos,rot,poolType),
            actionOnGet:OnGetObject,
            actionOnRelease:OnReleaseObject,
            actionOnDestroy:OnDestroyObject
            );

        _objectPools.Add(prefab, pool);
    }

    private static GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        prefab.SetActive(false);

        GameObject obj = Instantiate(prefab,pos,rot);

        prefab.SetActive(true);

        GameObject parentObject = SetParentObject(poolType);
        obj.transform.SetParent(parentObject.transform);

        return obj;
    }

    private static void OnGetObject(GameObject obj)
    {

    }

    private static void OnReleaseObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    private static void OnDestroyObject(GameObject obj)
    {
        if (_cloneToPrefabMap.ContainsKey(obj))
        {
            _cloneToPrefabMap.Remove(obj);
        }
    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        switch(poolType)
        {
            case PoolType.Projectiles:
                return _projectileEmpty;
            default:
                return null;
        }
    }

    private static T SpawnObject<T>(GameObject objectToSpawn,Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Object
    {
        if (!_objectPools.ContainsKey(objectToSpawn))
        {
            CreatePool(objectToSpawn, spawnPos, spawnRotation, poolType);
        }
        GameObject obj = _objectPools[objectToSpawn].Get();

        if (obj != null)
        {
            if (!_cloneToPrefabMap.ContainsKey(obj))
            {
                _cloneToPrefabMap.Add(obj,objectToSpawn);
            }

            obj.transform.position = spawnPos;
            obj.transform.rotation = spawnRotation;
            obj.SetActive(true);

            if(typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }

            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"Object{objectToSpawn.name} doesnot have component{typeof(T)}");
                return null;
            }

            return component;
        }
        return null;
    }

    public static T SpawnObject<T>(T typePrefab,Vector3 spawnPos,Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Component
    {
        return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRotation, poolType);
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects)
    {
        return SpawnObject<GameObject>(objectToSpawn, spawnPos, spawnRotation, poolType);
    }

    public static void ReturnObjectToPool(GameObject obj,PoolType poolType)
    {
        if (_cloneToPrefabMap.TryGetValue(obj,out GameObject prefab))
        {
            GameObject parentObject = SetParentObject(poolType);

            if(obj.transform.parent != parentObject.transform)
            {
                obj.transform.SetParent(parentObject.transform);
            }

            if(_objectPools.TryGetValue(prefab,out ObjectPool<GameObject> pool))
            {
                pool.Release(obj);
            }
        }
        else
        {
            Debug.LogWarning("Trying to return an obj that is not pooled" + obj.name);
        }
    }

}
