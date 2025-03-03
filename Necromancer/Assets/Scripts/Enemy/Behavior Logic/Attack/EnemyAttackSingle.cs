using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack-Single", menuName = "Enemy Logic/Attack/Attack-Single")]
public class EnemyAttackSingle : EnemyAttackSOBase
{

    private Transform _playerTransform;
    private float _timer;
    private float _timeBetweenShots = 2f;


    public override void DoAnimationTriggerEventLogic(AnimationTriggerType triggerType)
    {
        base.DoAnimationTriggerEventLogic(triggerType);
    }

    public override void DoEnterLogic()
    {
        base.DoEnterLogic();
    }

    public override void DoExitLogic()
    {
        base.DoExitLogic();
    }

    public override void DoUpdateLogic()
    {
        base.DoUpdateLogic();
        enemy.MoveEnemy(Vector2.zero);
        if (_timer > _timeBetweenShots)
        {
            _timer = 0f;
            Debug.Log("我射");
        }
        _timer += Time.deltaTime;

        
    }

    public override void Initialize(GameObject gameObject, Enemy enemy)
    {
        base.Initialize(gameObject, enemy);
    }

    public override void ResetValues()
    {
        base.ResetValues();
    }
}
