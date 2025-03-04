using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Components/Attack/AttackSingle")]
public class Component_AttackSingle : EnemyBehaviorComponent
{
    private float _timer;
    [SerializeField] private float _timeBetweenShots = 1f;
    public override void OnUpdate()
    {
        enemy.SetVelocity(0, 0);
        if (_timer > _timeBetweenShots)
        {
            _timer = 0f;
            Debug.Log("我射");
            enemy.isAttacking = false;
        }
        _timer += Time.deltaTime;
    }
}
