using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Enemy/Components/Chase/不动追逐")]
public class Component_不动追逐 : EnemyBehaviorComponent
{

    public override void OnEnter()
    {

    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        
    }

    public override void OnUpdate()
    {
        enemy.anim.SetBool("Chase", true);
        enemy.anim.SetBool("Idle", false);

    }
}
