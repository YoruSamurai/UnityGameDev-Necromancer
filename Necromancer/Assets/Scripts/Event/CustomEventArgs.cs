
using System;

public class CustomEventArgs
{
    
}


public class PlayerDeadEventArgs : EventArgs
{
    public string playerName;
}

public class OnPlayerHitEventArgs : EventArgs
{
    public BaseEquipment baseEquipment;
    public MonsterStats monsterStats;
}

public class OnPlayerHittedEventArgs : EventArgs
{
    public MonsterStats monsterStats;
}


public class OnEnemyDeadEventArgs : EventArgs
{
    public MonsterStats monsterStats;
}

public class OnPlayerCritEventArgs : EventArgs
{
    public BaseEquipment baseEquipment;
    public MonsterStats monsterStats;
}
public class OnPlayerParryEventArgs : EventArgs
{
    public BaseEquipment baseEquipment;
    public MonsterStats monsterStats;
}
public class OnPlayerComboEventArgs : EventArgs
{
    public BaseEquipment baseEquipment;
    public MonsterStats monsterStats;
}