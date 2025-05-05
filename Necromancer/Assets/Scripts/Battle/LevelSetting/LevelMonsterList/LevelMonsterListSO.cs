using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ohter/LevelMonsterList")]

public class LevelMonsterListSO : ScriptableObject
{
    [SerializeField] public List<GameObject> levelMonsterList; 
}
