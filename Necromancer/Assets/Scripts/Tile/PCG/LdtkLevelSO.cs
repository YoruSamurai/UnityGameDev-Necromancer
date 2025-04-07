using LDtkUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelSO", menuName = "Level")]
public class LdtkLevelSO : ScriptableObject
{

    public LDtkComponentLevel levelData;
    public GameRoomType gameRoomType;
    public float levelHeight;
    public float levelWidth;
    public List<DoorInfo> doorInfos;

}



[Serializable]
public struct DoorInfo
{
    public Vector3Int startCell; // 门起始单元格
    public int length;           // 门的长度（连续的格子数）
    public DoorDir direction;    // 门的方向
    public bool isLocked;        //门已经被上锁了
    public int node;             //表示对应门的Node

    
}

public enum DoorDir 
{ 
    Left, 
    Right, 
    Top, 
    Bottom,
}