using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySummonGenerator : MonoBehaviour
{
    private Enemy enemy;
    private Player player;
    [SerializeField] private EnemySummonSO summonSO;

    [SerializeField] public GameObject summon;
    [SerializeField] public GameObject summoningCircle;

    public void Initialize(Enemy _enemy, Player _player, EnemySummonSO _summonSO)
    {
        enemy = _enemy;
        player = _player;
        summonSO = _summonSO;
        summon = summonSO.summon;
        summoningCircle = summonSO.summoningCircle;



        StartCoroutine("SummonEnemy");
    }


    public IEnumerator SummonEnemy()
    {
        int projectileNum = summonSO.summonNum;
        for (int i = 0; i < projectileNum; i++)
        {
            GameObject obj = Instantiate(
            summon,
            enemy.shootPosition.position + 
                new Vector3(enemy.facingRight? summonSO.startPosition.x : -summonSO.startPosition.x, summonSO.startPosition.y, 0),
            Quaternion.identity
            );
            Enemy summonEnemy = obj.GetComponent<Enemy>();
            if (summonEnemy.transform.position.x > player.transform.position.x)
            {
                summonEnemy.Flip();
            }
            yield return new WaitForSeconds(summonSO.summonInterval);
        }
        Destroy(gameObject);
    }

}
