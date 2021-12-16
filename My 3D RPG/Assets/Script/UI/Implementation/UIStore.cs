using ProjectChan.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIStore : UIWindow
{

    /// <summary>
    /// => 상점 슬롯 홀더에 자식으로 존재하는 슬롯들을 담아놓을 공간
    /// </summary>
    public List<ItemSlot> storeSlots { get; private set; } = new List<ItemSlot>();

    public override void Start()
    {





        base.Start();
    }
}