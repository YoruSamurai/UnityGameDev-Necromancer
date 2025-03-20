using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 人物伤害碰撞检测 : BaseSkillComponent
{
    [Header("伤害区参数")]
    public int damage = 10;
    public float duration = 0.5f;
    public Vector2 boxSize = new Vector2(2f, 1f);
    public Vector2 offset = Vector2.zero;

    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();

    public override bool CanExecute(Player player, PlayerStats playerStats, SkillSO skillData)
    {
        // 这里可以加条件限制，例如必须在地面上才能生效
        return true;
    }

    public override void Execute(Player player, PlayerStats playerStats, SkillSO skillData)
    {
        StartCoroutine(ActivateDamageZone(player));
    }

    private IEnumerator ActivateDamageZone(Player player)
    {
       /* GameObject damageZoneObj = new GameObject("DamageZone");
        damageZoneObj.transform.position = player.transform.position;*/

        //BoxCollider2D collider = damageZoneObj.AddComponent<BoxCollider2D>();
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.size = boxSize;
        collider.offset = offset;

        yield return new WaitForSeconds(duration);
        damagedEnemies.Clear();
        Destroy(collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !damagedEnemies.Contains(other.gameObject))
        {
            damagedEnemies.Add(other.gameObject);

            if (other.TryGetComponent<Enemy>(out Enemy enemy))
            {
                Debug.Log($"🔥 {other.name} 受到 {damage} 点伤害");
            }
        }
    }

    public override void OnSkillEnd(Player player, SkillSO skillData)
    {
        Debug.Log("伤害区域结束");
    }
}
