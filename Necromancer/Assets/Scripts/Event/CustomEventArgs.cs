
using System;

public class CustomEventArgs
{
    
}


public class PlayerDeadEventArgs : EventArgs
{
    public string playerName;
}


public class NormalEnemyDeadEventArgs : EventArgs
{
    public string enemyName;//后续可能需要修改为获取enemy的csv的对应id以获取对应解锁。
}