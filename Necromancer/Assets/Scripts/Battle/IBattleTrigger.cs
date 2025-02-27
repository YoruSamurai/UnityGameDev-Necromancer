
public interface IBattleTrigger
{
    void InvokeAffixPre(PlayerStats playerStats,MonsterStats monsterStats);
    void InvokeAffixPost(PlayerStats playerStats,MonsterStats monsterStats,float cMag);
}
