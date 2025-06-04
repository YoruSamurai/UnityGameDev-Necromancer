using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArchiveManager : SingletonManagerBase<ArchiveManager>
{
    protected override void Awake()
    {
        base.Awake(); // 必须保留：处理单例与DDOL
    }

    public GameData CreateNewGameData()
    {
        GameData gameData = new GameData();
        gameData.playerData = new PlayerData();

        return gameData;
    }

}
