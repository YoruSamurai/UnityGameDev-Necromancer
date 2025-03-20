using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Battle/Skill")]
public class SkillSO : ScriptableObject
{
    public string skillName;
    [TextArea] public string description;
    public Sprite icon;
    public float cooldown;    // 冷却时间

    [Header("投射物参数")]
    public GameObject projectilePrefab; // 投射物预制体
    public float projectileSpeed = 10f;
    public float projectileMaxDistance = 5f;
    public float projectileMaxTimer = 3f;
    public float projectileGravity = 0f;
    public float projectileAngle = 0f;
}
