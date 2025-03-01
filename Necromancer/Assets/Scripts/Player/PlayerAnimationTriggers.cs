using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();

    //不知道会不会出问题啊。。
    private void AnimationTrigger()
    {
        Debug.Log("我在哪？" + this.gameObject.name);   
        player.AnimationTrigger();
    }
}
