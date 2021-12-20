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
    private BoNPC boNPC;                    // -> 현재 상점의 주인

    public TextMeshProUGUI storeName;       // -> 현재 상점의 이름
    public Transform ConfirmFrame;          // -> 아이템을 살건지 물어보는 창
    public Button closeBtn;                 // -> 상점 창 닫기 버튼

    /// <summary>
    /// => 상점 슬롯 홀더에 자식으로 존재하는 슬롯들을 담아놓을 공간
    /// </summary>
    public List<ItemSlot> storeSlots { get; private set; } = new List<ItemSlot>();

    public override void Start()
    {
        gr = GetComponentInParent<GraphicRaycaster>();

        var btnHolder = ConfirmFrame.GetChild(0);

        // -> 네 버튼 이벤트 바인딩!
        itemYesBtn = btnHolder.Find("YesButton").GetComponent<Button>();
        itemYesBtn.onClick.AddListener(() => { OnItemYesButton(); });

        // -> 아니요 버튼 이벤트 바인딩!
        itemNoBtn = btnHolder.Find("NoButton").GetComponent<Button>();
        itemNoBtn.onClick.AddListener(() => { ConfirmFrame.gameObject.SetActive(false); });

        closeBtn.onClick.AddListener(() => { Close(); });

        // -> 상점에 존재하는 상점 슬롯들을 가져오는 작업!
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
        this.boNPC = boNPC;

        storeName.text = boNPC.sdNPC.name + "의 상점!";

        // -> 상점을 세팅합니다!
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
        // -> 상점 창이 켜지면 인벤토리 창도 같이 켜줍니다
        UIWindowManager.Instance.GetWindow<UIInventory>().Open();
    }

    public void Close()
    {
        // -> 상점 창을 닫으면서 NPC와의 대화가 끝났습니다!
        boNPC.actor.isPlayerAction = false;
        boNPC.actor = null;

        base.Close();
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

    /// <summary>
    /// => 아이템이 존재하는 상점 슬롯을 클릭 했을 때 호출되는 메서드
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData)
    {
        var results = new List<RaycastResult>();

        gr.Raycast(eventData, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject.name.Contains("StoreSlot"))
            {
                currentClickSlost = results[i].gameObject.GetComponent<ItemSlot>();
                break;
            }
        }

        // -> 아이템을 살건지 확인하는 창!
        if (currentClickSlost.sdItem != null)
        {
            ConfirmFrame.gameObject.SetActive(true);
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// => 확인 창에서 네 버튼에 바인딩 될 메서드
    /// </summary>
    private void OnItemYesButton()
    {
        var dummyServer = DummyServer.Instance;
        var uiInventory = UIWindowManager.Instance.GetWindow<UIInventory>();

        // -> 아이템의 가격을 플레이어의 골드에서 빼줍니다!
        var dtoAccount = dummyServer.userData.dtoAccount;

        // -> 아이템 가격보다 소지 금액이 부족하다면!
        if (dtoAccount.gold < currentClickSlost.sdItem.price)
        {
            ConfirmFrame.gameObject.SetActive(false);
            return;
        }

        // -> 소지한 금액에서 아이템 가격을 빼줍니다!
        dtoAccount.gold -= currentClickSlost.sdItem.price;

        // -> 소지한 금액이 마이너스가 되지 않도록
        if (dtoAccount.gold <= 0)
        {
            dtoAccount.gold = 0;
        }

        // -> 현재 인벤토리를 확인 해야하므로 인벤토리에 담겨있는 슬롯을 가져옵니다!
        var itemSlots = uiInventory.itemSlots;

        // -> 플레이어가 선택한 아이템이 인벤토리에 존재 한다면 인벤토리에 존재하는 아이템이 담길 변수!
        ItemSlot selectItem = null;

        for (int i = 0; i < itemSlots.Count; i++)
        {
            // -> 존재한다면 아이템이 들어있는 슬롯!
            if (itemSlots[i].BoItem != null)
            {
                // -> 슬롯에 살려는 아이템이 존재한다면!
                if (itemSlots[i].BoItem.sdItem.index == currentClickSlost.sdItem.index)
                {
                    selectItem = itemSlots[i];
                    break;
                }
            }
        }

        // -> 같은 아이템을 지닌 슬롯이 존재하는지!
        if (selectItem != null)
        {
            // -> 존재하면 개수를 올려줍니다!
            selectItem.BoItem.amount++;
            uiInventory.IncreaseItem(selectItem.BoItem);
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

        // -> 소지한 금액 업데이트!
        uiInventory.MyGoldUpdate();
        ConfirmFrame.gameObject.SetActive(false);
    }
}