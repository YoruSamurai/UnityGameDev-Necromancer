using UnityEngine;

[CreateAssetMenu(fileName = "MeleeMonsterConfig", menuName = "ScriptableObjects/MeleeMonsterConfig")]
public class MeleeMonsterConfig : ScriptableObject
{
    public int[] thresholds = new int[] { 7, 10, 13, 17 };
    public int[] possibilities = new int[] { 10, 30, 50, 70, 90 };
}
