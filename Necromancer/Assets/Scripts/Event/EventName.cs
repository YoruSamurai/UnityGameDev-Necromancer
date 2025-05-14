public static class EventName
{
    public const string PlayerAttack = nameof(PlayerAttack);

    /// <summary>
    /// 玩家命中敌人
    /// </summary>
    public const string OnPlayerHit = nameof(OnPlayerHit);
    /// <summary>
    /// 玩家被命中
    /// </summary>
    public const string OnPlayerHitted = nameof(OnPlayerHitted);
    /// <summary>
    /// 玩家击杀敌人
    /// </summary>
    public const string OnEnemyDead = nameof(OnEnemyDead);
    /// <summary>
    /// 玩家暴击
    /// </summary>
    public const string OnPlayerCrit = nameof(OnPlayerCrit);
    /// <summary>
    /// 玩家招架
    /// </summary>
    public const string OnPlayerParry = nameof(OnPlayerParry);
    /// <summary>
    /// 玩家连段中
    /// </summary>
    public const string OnPlayerCombo = nameof(OnPlayerCombo);



}