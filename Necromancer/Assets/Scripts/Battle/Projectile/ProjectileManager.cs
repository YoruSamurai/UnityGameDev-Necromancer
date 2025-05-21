using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance;

    [SerializeField] private GameObject projectileGeneratorPrefab;


    private void Awake()
    {
        // 确保实例唯一
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 防止多个实例
        }
    }

    public void GenerateProjectile(ProjectileSO _projectileSO,BaseEquipment _baseEquipment,bool facingRight ,Enemy _enemy,int combo)
    {

        GameObject obj =  ObjectPoolManager.SpawnObject(projectileGeneratorPrefab, transform.position, Quaternion.identity,ObjectPoolManager.PoolType.Projectiles);

        /*GameObject obj = Instantiate(
            projectileGeneratorPrefab,
            transform.position,
            Quaternion.identity,
            transform
            );*/
        Debug.Log(_projectileSO.ToString() + "_projectileSO");
        BaseProjectileGenerator generator = obj.GetComponent<BaseProjectileGenerator>();
        generator.Initialize(_projectileSO, _baseEquipment, facingRight, _enemy, combo);
    }
}
