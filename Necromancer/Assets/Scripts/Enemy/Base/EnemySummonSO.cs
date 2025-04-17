using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//对一个召唤物进行配置 
[CreateAssetMenu(menuName = "Enemy/Summon")]
public class EnemySummonSO : ScriptableObject
{
    [SerializeField] public GameObject summon;
    [SerializeField] public GameObject summoningCircle;

    [SerializeField] public Vector2 startPosition;
    [SerializeField] public int summonNum;
    [SerializeField] public float summonInterval;



}
