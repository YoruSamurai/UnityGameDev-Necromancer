using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneController : MonoBehaviour
{
    private float stayTime;
    private float determineInterval;
    private bool determineOnlyOnce;
    private BaseZoneGenerator generator;

    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    public void Initialize(float stayTime, float determineInterval, bool determineOnlyOnce, BaseZoneGenerator generator)
    {
        this.stayTime = stayTime;
        this.determineInterval = determineInterval;
        this.determineOnlyOnce = determineOnlyOnce;
        this.generator = generator;

        StartCoroutine(DetermineRoutine());

        Destroy(gameObject, stayTime);
    }

    private IEnumerator DetermineRoutine()
    {
        int i = 100;
        do
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, col.bounds.size, 0f, LayerMask.GetMask("Enemy"));
            foreach (var hit in hits)
            {
                int id = hit.gameObject.GetInstanceID();
                if (!generator.HasDamaged(id))
                {
                    generator.RegisterDamagedEnemy(id);
                    Debug.Log("击中敌人：" + hit.name);
                    // TODO: 在这里调用敌人受伤逻辑
                }
            }
            i--;
            yield return new WaitForSeconds(determineInterval);

        } while (i > 0);
    }

    private void OnDrawGizmos()
    {
        if (col != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}
