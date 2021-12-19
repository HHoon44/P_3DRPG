using ProjectChan;
using ProjectChan.DB;
using ProjectChan.Dummy;
using ProjectChan.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIStore : UIWindow, IPointerClickHandler
{
    private Transform storeSlotHolder;      // -> 상점 슬롯이 담겨잇는 홀더
    private GraphicRaycaster gr;            // -> 컨버스안을 탐지하기 위한 레이캐스터
    private ItemSlot currentClickSlost;     // -> 현재 플레이어가 클릭한 상점 슬롯 
    private Button itemYesBtn;              // -> 네 살겁니다 버튼
    private Button itemNoBtn;               // -> 아니요 안살겁니다 버튼

    public TextMeshProUGUI storeName;       // -> 현재 상점의 이름
    public Transform ConfirmFrame;         // -> 아이템을 살건지 물어보는 창

    /// <summary>
    /// => 상점 슬롯 홀더에 자식으로 존재하는 슬롯들을 담아놓을 공간
    /// </summary>
    public List<ItemSlot> storeSlots { get; private set; } = new List<ItemSlot>();

    public override void Start()
    {
        gr = GetComponentInParent<GraphicRaycaster>();

        var btnHolder = ConfirmFrame.GetChild(0);

        itemYesBtn = btnHolder.Find("YesButton").GetComponent<Button>();
        itemYesBtn.onClick.AddListener(() => { OnItemYesButton(); });

        itemNoBtn = btnHolder.Find("NoButton").GetComponent<Button>();
        itemNoBtn.onClick.AddListener(() => { ConfirmFrame.gameObject.SetActive(false); });

        storeSlotHolder = transform.GetChild(0).GetChild(0);

        for (int i = 0; i < storeSlotHolder.childCount; i++)
        {
            storeSlots.Add(storeSlotHolder.GetChild(i).GetComponent<ItemSlot>());
        }

        InitStoreSlots();

        base.Start();
    }

    public void Open(BoNPC boNPC)
    {
        storeName.text = boNPC.sdNPC.name + "의 상점!";

        for (int i = 0; i < storeSlots.Count; i++)
        {
            if (boNPC.sdNPC.storeItem.Length - 1 >= i)
            {
                var boItem = GameManager.SD.sdItems.Where(obj => obj.index == boNPC.sdNPC.storeItem[i])?.SingleOrDefault();
                storeSlots[i].SetSlot(boItem);
            }
            else
            {
                storeSlots[i].SetSlot();
            }
        }

        base.Open();
    }

    /// <summary>
    /// => 상점 슬롯들을 초기화 해주는 메서드
    /// </summary>
    public void InitStoreSlots()
    {
        for (int i = 0; i < storeSlots.Count; i++)
        {
            storeSlots[i].Initialize();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();

        gr.Raycast(eventData, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.name.Contains("ItemSlot"))
            {
                currentClickSlost = results[i].gameObject.GetComponent<ItemSlot>();
                break;
            }
        }

        // -> 아이템을 살건지 확인하는 창!
        ConfirmFrame.gameObject.SetActive(true);
    }

    private void OnItemYesButton()
    {
        var dummyServer = DummyServer.Instance;
        var uiInventory = UIWindowManager.Instance.GetWindow<UIInventory>();

        // -> 아이템의 가격을 플레이어의 골드에서 빼줍니다!
        var dtoAccount = dummyServer.userData.dtoAccount;
        dtoAccount.gold -= currentClickSlost.sdItem.price;

        // -> 현재 인벤토리를 확인 해야하므로 인벤토리에 담겨있는 슬롯을 가져옵니다!
        var itemSlots = uiInventory.itemSlots;

        ItemSlot qwe = null;

        for (int i = 0; i < itemSlots.Count; i++)
        {
            // -> 존재한다면 아이템이 들어있는 슬롯!
            if (itemSlots[i].BoItem != null)
            {
                // -> 슬롯에 살려는 아이템이 존재한다면!
                if (itemSlots[i].BoItem.sdItem.index == currentClickSlost.sdItem.index)
                {
                    qwe = itemSlots[i];
                    break;
                }
            }
        }

        // -> 같은 아이템을 지닌 슬롯이 존재하는지!
        if (qwe != null)
        {
            // -> 존재하면 개수를 올려줍니다!
            qwe.BoItem.amount++;
            uiInventory.IncreaseItem(qwe.BoItem);
        }
        else
        {
            for (int i = 0; i < itemSlots.Count; i++)
            {
                // -> 비어있는 슬롯이라면!
                if (itemSlots[i].BoItem == null)
                {
                    // -> 비어있는 슬롯에 살려는 아이템을 추가해 줍니다!
                    var boItem = new BoItem(currentClickSlost.sdItem);
                    uiInventory.AddItem(boItem);
                    GameManager.User.boItems.Add(boItem);
                    break;
                }
            }
        }

        dummyServer.userData.dtoItem = new DtoItem(GameManager.User.boItems);
        GameManager.User.boAccount = new BoAccount(dummyServer.userData.dtoAccount);
        dummyServer.Save();

        ConfirmFrame.gameObject.SetActive(false);
    }
}