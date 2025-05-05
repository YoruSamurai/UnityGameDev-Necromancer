using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class 骷髅 : Enemy
{

    private void OnDisable()
    {
        Debug.Log("为什么我不见了");
    }
    protected override void Awake()
    {
        base.Awake();
        
    }

    protected override void Start()
    {
        base.Start();
        
    }

    protected override void Update()
    {
        base.Update();
    }
}
