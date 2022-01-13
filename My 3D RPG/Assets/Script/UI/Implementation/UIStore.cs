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

namespace ProjectChan.UI
{
    /// <summary>
    /// => ���� UI�� ������ Ŭ����
    /// </summary>
    public class UIStore : UIWindow, IPointerClickHandler
    {
        public TextMeshProUGUI storeName;       // -> ���� ������ �̸�
        public Transform ConfirmFrame;          // -> ������ ���� ���θ� ����� â
        public Button closeBtn;                 // -> ���� â �ݱ� ��ư

        private Transform storeSlotHolder;      // -> ���� ������ ����մ� Ȧ��
        private GraphicRaycaster gr;            // -> ���������� Ž���ϱ� ���� ����ĳ����
        private ItemSlot currentClickSlost;     // -> ���� �÷��̾ Ŭ���� ���� ���� 
        private Button itemYesBtn;              // -> �� ��̴ϴ� ��ư
        private Button itemNoBtn;               // -> �ƴϿ� �Ȼ�̴ϴ� ��ư
        private BoNPC boNPC;                    // -> ���� ������ ����

        /// <summary>
        /// => ���� ���� Ȧ���� �ڽ����� �����ϴ� ���Ե��� ��Ƴ��� ����
        /// </summary>
        public List<ItemSlot> storeSlots { get; private set; } = new List<ItemSlot>();

        public override void Start()
        {
            gr = GetComponentInParent<GraphicRaycaster>();

            // -> ���� ���θ� ����� â�� �����ϴ� Yes��ư�� No��ư�� ���� �ɴϴ�!
            var btnHolder = ConfirmFrame.GetChild(0);

            // -> �� ��ư �̺�Ʈ ���ε�!
            itemYesBtn = btnHolder.Find("YesButton").GetComponent<Button>();
            itemYesBtn.onClick.AddListener(() => { OnItemYesButton(); });

            // -> �ƴϿ� ��ư �̺�Ʈ ���ε�!
            itemNoBtn = btnHolder.Find("NoButton").GetComponent<Button>();
            itemNoBtn.onClick.AddListener(() => { ConfirmFrame.gameObject.SetActive(false); });

            closeBtn.onClick.AddListener(() => { Close(); });

            // -> ������ �����ϴ� ���� ���Ե��� �������� �۾�!
            storeSlotHolder = transform.GetChild(0).GetChild(0);

            for (int i = 0; i < storeSlotHolder.childCount; i++)
            {
                storeSlots.Add(storeSlotHolder.GetChild(i).GetComponent<ItemSlot>());
            }

            InitStoreSlots();
            base.Start();
        }

        /// <summary>
        /// => ������ ������ �� ȣ��� �޼���
        /// </summary>
        /// <param name="boNPC"> ���� ���� NPC ������ </param>
        public void Open(BoNPC boNPC)
        {
            this.boNPC = boNPC;

            storeName.text = boNPC.sdNPC.name + "�� ����!";

            // -> ������ �����մϴ�!
            for (int i = 0; i < storeSlots.Count; i++)
            {
                // -> ���� NPC�� �Ǹ��� �������� ���ϰ� �ִٸ�!
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

            // -> ���� â�� ������ �κ��丮 â�� ���� ���ݴϴ�
            UIWindowManager.Instance.GetWindow<UIInventory>().Open();
        }

        public void Close()
        {
            // -> ���� â�� �����鼭 NPC���� ��ȭ�� �������ϴ�!
            boNPC.actor.isPlayerAction = false;
            boNPC.actor = null;

            base.Close();
        }

        /// <summary>
        /// => ������ �����ϴ� ���Ե��� �ʱ�ȭ �ϴ� �޼���
        /// </summary>
        public void InitStoreSlots()
        {
            for (int i = 0; i < storeSlots.Count; i++)
            {
                storeSlots[i].Initialize();
            }
        }

        /// <summary>
        /// => �������� �����ϴ� ���� ������ Ŭ�� ���� �� ȣ��Ǵ� �޼���
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            // -> ����ĳ��Ʈ ����� ��Ƴ��� �����Դϴ�!
            var results = new List<RaycastResult>();

            gr.Raycast(eventData, results);

            for (int i = 0; i < results.Count; i++)
            {
                // -> ������� �̸��߿� StroeSlot�� �����ϰ� �ִٸ�!
                if (results[i].gameObject.name.Contains("StoreSlot"))
                {
                    // -> ���� Ŭ���� ���Կ� i��° �����͸� ��ƵӴϴ�!
                    currentClickSlost = results[i].gameObject.GetComponent<ItemSlot>();
                    break;
                }
            }

            // -> ���� Ŭ���� ������ �� ������ �ƴ϶��!
            if (currentClickSlost.sdItem != null)
            {
                // -> ���� ���θ� ��� â�� ŵ�ϴ�!
                ConfirmFrame.gameObject.SetActive(true);
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// => Ȯ�� â���� �� ��ư�� ���ε� �� �޼���
        /// </summary>
        private void OnItemYesButton()
        {
            var dummyServer = DummyServer.Instance;
            var uiInventory = UIWindowManager.Instance.GetWindow<UIInventory>();

            // -> �������� ������ �÷��̾��� ��忡�� ���ݴϴ�!
            var dtoAccount = dummyServer.userData.dtoAccount;

            // -> ������ ���ݺ��� ���� �ݾ��� �����ϴٸ�!
            if (dtoAccount.gold < currentClickSlost.sdItem.price)
            {
                ConfirmFrame.gameObject.SetActive(false);
                return;
            }

            // -> ������ �ݾ׿��� ������ ������ ���ݴϴ�!
            dtoAccount.gold -= currentClickSlost.sdItem.price;

            // -> ���� ������ ���ŷ� ���� �������� 0���� �۰ų� ���ٸ�!
            if (dtoAccount.gold <= 0)
            {
                // -> 0���� �ʱ�ȭ ���ݴϴ�!
                dtoAccount.gold = 0;
            }

            // -> ���� �κ��丮�� Ȯ�� �ؾ��ϹǷ� �κ��丮�� ����ִ� ������ �����ɴϴ�!
            var itemSlots = uiInventory.itemSlots;

            // -> ������ �������� �̹� �κ��丮�� ���� �Ѵٸ� 
            //    �������� �����ϴ� ������ �����͸� �־���� �����Դϴ�!
            ItemSlot selectItem = null;

            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (itemSlots[i].BoItem != null)
                {
                    // -> ������ �������� ���Կ� �����Ѵٸ�!
                    if (itemSlots[i].BoItem.sdItem.index == currentClickSlost.sdItem.index)
                    {
                        // -> �� ������ ��Ƴ����ϴ�!
                        selectItem = itemSlots[i];
                        break;
                    }
                }
            }

            if (selectItem != null)
            {
                // -> �����Ѵٸ� ������ �÷��ݴϴ�!
                selectItem.BoItem.amount++;
                uiInventory.IncreaseItem(selectItem.BoItem);
            }
            else
            {
                // -> �������� �ʴ� �������̶�� �κ��丮�� ���Ӱ� �߰����ݴϴ�!
                for (int i = 0; i < itemSlots.Count; i++)
                {
                    if (itemSlots[i].BoItem == null)
                    {
                        // -> ����ִ� ���Կ� ������ �������� �߰��� �ݴϴ�!
                        var boItem = new BoItem(currentClickSlost.sdItem);
                        uiInventory.AddItem(boItem);
                        GameManager.User.boItems.Add(boItem);
                        break;
                    }
                }
            }

            // -> DB�� ������ �����ۿ� ���� ������ ������Ʈ ���ݴϴ�!
            dummyServer.userData.dtoItem = new DtoItem(GameManager.User.boItems);
            GameManager.User.boAccount = new BoAccount(dummyServer.userData.dtoAccount);
            dummyServer.Save();

            // -> ������ �ݾ��� ������Ʈ ���ݴϴ�!
            uiInventory.MyGoldUpdate();
            ConfirmFrame.gameObject.SetActive(false);
        }
    }
}