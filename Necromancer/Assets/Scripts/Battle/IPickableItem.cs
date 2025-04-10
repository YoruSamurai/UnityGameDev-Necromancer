using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickableItem
{
    public string GetItemName();
    public string GetItemMessage();
    public Sprite GetSprite();
    public void OnPickup(); // 可选：如果你想让每种物体在拾取时做自定义逻辑
}
