using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForgeSystem : MonoBehaviour
{
    private IEquipableItem currentItem;

    private int itemLevel;

    public int levelUpGold;//升级需要的金币
    public int recastGold;//重铸需要的金币
    [SerializeField] private Text levelUpGoldText;
    [SerializeField] private Text recastGoldText;

    [SerializeField] private ForgeUI forgeUI;

    private ForgeSlot currentSlot;


    /// <summary>
    /// 基于当前装备进行一些面板的设置
    /// </summary>
    /// <param name="item"></param>
    public void Setup(IEquipableItem item , ForgeSlot slot)
    {
        currentItem = item;
        currentSlot = slot;
        SetUpGold();
    }

    /// <summary>
    /// 基于当前装备进行需要的金钱的设置 暂时50块
    /// 后续可能会根据多种参数进行？
    /// </summary>
    private void SetUpGold()
    {
        itemLevel = currentItem.GetEquipableItemLevel();
        levelUpGold = 50 * itemLevel;
        recastGold = 50;
        levelUpGoldText.text = $"-{levelUpGold}G";
        recastGoldText.text = $"-{recastGold}G";
    }


    /// <summary>
    /// 升级按钮，我们看看需要调用哪些接口。
    /// 1：检查等级是否已经到了当前地图的上限 检查钱够不够 有错误提示/按不了
    /// 2：按下去 我们升级咯！ 给他升级
    /// 3：刷新铸造台 然后获取这个item 类似于重新点击
    /// </summary>
    public void OnClickLevelUp()
    {
        Debug.Log("我要升级！");
        if (LevelManager.Instance.IsLevelLimit(itemLevel))
        {
            Debug.Log("等级达到上限！无法升级");
            return;
        }
        if (PlayerStats.Instance.IsGoldLimit())
        {
            Debug.Log("金币不够 无法升级");
            return;
        }
        //开始升级逻辑
        currentItem.EquipableItemLevelUp();

        //刷新铸造台
        forgeUI.RefreshPanel();


        currentSlot.OnclickSlot();
    }

    /// <summary>
    /// 重铸按钮，我们看看需要调用哪些接口。
    /// 1：检查钱够不够 有错误提示/按不了
    /// 2：按下去 我们调用IEquipableItem的接口 让它去BMT里面根据物品等级获取词条 给他丢进去
    /// 3：刷新铸造台 然后获取这个item 类似于重新点击
    /// </summary>
    public void OnClickRecast()
    {
        Debug.Log("我要重铸！");
        if (PlayerStats.Instance.IsGoldLimit())
        {
            Debug.Log("金币不够 无法重铸");
            return;
        }

        //开始升级逻辑
        currentItem.EquipableItemRecast();

        //刷新铸造台
        forgeUI.RefreshPanel();


        currentSlot.OnclickSlot();
    }


}
