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
    private Transform storeSlotHolder;      // -> ���� ������ ����մ� Ȧ��
    private GraphicRaycaster gr;            // -> ���������� Ž���ϱ� ���� ����ĳ����
    private ItemSlot currentClickSlost;     // -> ���� �÷��̾ Ŭ���� ���� ���� 
    private Button itemYesBtn;              // -> �� ��̴ϴ� ��ư
    private Button itemNoBtn;               // -> �ƴϿ� �Ȼ�̴ϴ� ��ư

    public TextMeshProUGUI storeName;       // -> ���� ������ �̸�
    public Transform ConfirmFrame;         // -> �������� ����� ����� â

    /// <summary>
    /// => ���� ���� Ȧ���� �ڽ����� �����ϴ� ���Ե��� ��Ƴ��� ����
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
        storeName.text = boNPC.sdNPC.name + "�� ����!";

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
    /// => ���� ���Ե��� �ʱ�ȭ ���ִ� �޼���
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

        // -> �������� ����� Ȯ���ϴ� â!
        ConfirmFrame.gameObject.SetActive(true);
    }

    private void OnItemYesButton()
    {
        var dummyServer = DummyServer.Instance;
        var uiInventory = UIWindowManager.Instance.GetWindow<UIInventory>();

        // -> �������� ������ �÷��̾��� ��忡�� ���ݴϴ�!
        var dtoAccount = dummyServer.userData.dtoAccount;
        dtoAccount.gold -= currentClickSlost.sdItem.price;

        // -> ���� �κ��丮�� Ȯ�� �ؾ��ϹǷ� �κ��丮�� ����ִ� ������ �����ɴϴ�!
        var itemSlots = uiInventory.itemSlots;

        ItemSlot qwe = null;

        for (int i = 0; i < itemSlots.Count; i++)
        {
            // -> �����Ѵٸ� �������� ����ִ� ����!
            if (itemSlots[i].BoItem != null)
            {
                // -> ���Կ� ����� �������� �����Ѵٸ�!
                if (itemSlots[i].BoItem.sdItem.index == currentClickSlost.sdItem.index)
                {
                    qwe = itemSlots[i];
                    break;
                }
            }
        }

        // -> ���� �������� ���� ������ �����ϴ���!
        if (qwe != null)
        {
            // -> �����ϸ� ������ �÷��ݴϴ�!
            qwe.BoItem.amount++;
            uiInventory.IncreaseItem(qwe.BoItem);
        }
        else
        {
            for (int i = 0; i < itemSlots.Count; i++)
            {
                // -> ����ִ� �����̶��!
                if (itemSlots[i].BoItem == null)
                {
                    // -> ����ִ� ���Կ� ����� �������� �߰��� �ݴϴ�!
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