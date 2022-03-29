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
    /// ���� â�� ������ Ŭ����
    /// </summary>
    public class UIStore : UIWindow, IPointerClickHandler
    {
        // public
        public TextMeshProUGUI storeName;       // ���� ������ �̸�
        public Transform ConfirmFrame;          // ������ ���� ���θ� ����� â
        public Button buyBtn;                   // ��� ��ư
        public Button noBuyBtn;                 // �Ȼ�� ��ư
        public Button closeBtn;                 // ���� �ݱ� ��ư

        // private
        private BoNPC boNPC;                    // ���� ������ ����
        private Transform storeSlotHolder;      // ���� ������ ����մ� Ȧ��
        private GraphicRaycaster gr;            // ���������� Ž���ϱ� ���� ����ĳ����
        private ItemSlot currentClickSlost;     // ���� �÷��̾ Ŭ���� ���� ���� 

        /// <summary>
        /// ���� â�� �����ϴ� ������ ����
        /// </summary>
        public List<ItemSlot> storeSlots { get; private set; } = new List<ItemSlot>();

        public override void Start()
        {
            gr = GetComponentInParent<GraphicRaycaster>();

            // ���/�Ȼ��/â �ݱ� ��ư�� �̺�Ʈ ���ε�
            buyBtn.onClick.AddListener(() => { OnBuyButton(); });
            noBuyBtn.onClick.AddListener(() => { ConfirmFrame.gameObject.SetActive(false); });
            closeBtn.onClick.AddListener(() => { Close(); });

            // ������ ������ ���� Ȧ���� ������
            storeSlotHolder = transform.GetChild(0).GetChild(0);

            // Ȧ���� �����ϴ� ������ ������ ��� List�� ��Ƴ��� �۾�
            for (int i = 0; i < storeSlotHolder.childCount; i++)
            {
                storeSlots.Add(storeSlotHolder.GetChild(i).GetComponent<ItemSlot>());
            }

            // ��Ƶ� ������ ������ �ʱ�ȭ
            InitStoreSlots();

            base.Start();
        }

        /// <summary>
        /// Ȱ��ȭ�� ���� â�� �����ϴ� �޼���
        /// </summary>
        /// <param name="boNPC"> ���� ���� NPC ������ </param>
        public void Open(BoNPC boNPC)
        {
            var sdItems = GameManager.SD.sdItems;

            // ���� NPC�� ������
            this.boNPC = boNPC;
            storeName.text = boNPC.sdNPC.name + "�� ����!";

            // ��Ƶ� ������ ���Կ� �������� �����ϴ� �۾�
            for (int i = 0; i < storeSlots.Count; i++)
            {
                // ���� NPC�� �Ǹ��ϴ� �������� �ִٸ�
                if (boNPC.sdNPC.storeItem.Length - 1 >= i)
                {
                    // �Ǹ��ϴ� �������� ��ȹ �����͸� ������ Bo�����ͷ� ����
                    var boItem = sdItems.Where(obj => obj.index == boNPC.sdNPC.storeItem[i])?.SingleOrDefault();

                    // Bo�����͸� ������ ���Կ� ����
                    storeSlots[i].SetSlot(boItem);
                }
                else
                {
                    storeSlots[i].SetSlot();
                }
            }
            base.Open();

            // ���� â�� Ȱ��ȭ �Ǹ� �κ��丮 â�� Ȱ��ȭ
            UIWindowManager.Instance.GetWindow<UIInventory>().Open();
        }

        public void Close()
        {
            boNPC.actor.isPlayerAction = false;
            boNPC.actor = null;

            base.Close();
        }

        /// <summary>
        /// �������� �����ϴ� ���� Ŭ�� �� ȣ��Ǵ� �޼���
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            // ����ĳ��Ʈ ����� ���� ����
            var results = new List<RaycastResult>();

            // �׷��� ����ĳ��Ʈ�� �̺�Ʈ ������ UI���� List�� ��Ƶ�
            gr.Raycast(eventData, results);

            // ��Ƶ� ��������� StoreSlot�� ã�� �۾�
            for (int i = 0; i < results.Count; i++)
            {
                // StoreSlot�� �ִٸ�
                if (results[i].gameObject.name.Contains("StoreSlot"))
                {
                    // ������ Ŭ���� ������ ������ ��Ƶ�
                    currentClickSlost = results[i].gameObject.GetComponent<ItemSlot>();
                    break;
                }
            }

            // Ŭ���� ������ �����Ѵٸ�
            if (currentClickSlost.sdItem != null)
            {
                // ���� ���θ� ����� â�� ���
                ConfirmFrame.gameObject.SetActive(true);
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// ���� ��ư�� ���ε��� �޼���
        /// </summary>
        private void OnBuyButton()
        {
            var dummyServer = DummyServer.Instance;
            var uiInventory = UIWindowManager.Instance.GetWindow<UIInventory>();

            var dtoAccount = dummyServer.userData.dtoAccount;

            // ���� �ݾ��� �����ϴٸ�
            if (dtoAccount.gold < currentClickSlost.sdItem.price)
            {
                ConfirmFrame.gameObject.SetActive(false);
                return;
            }

            // ������ �ݾ׿��� ������ ������ ��
            dtoAccount.gold -= currentClickSlost.sdItem.price;

            // ���� ���� �������� 0���϶��
            if (dtoAccount.gold <= 0)
            {
                dtoAccount.gold = 0;
            }

            // �κ��丮�� �����ϴ� ���Ե�
            var itemSlots = uiInventory.itemSlots;

            // ������ �������� �̹� ���Կ� �ִٸ�
            // ������ ��Ƴ��� ����
            ItemSlot sameItem = null;

            // ������ �������� ���� ������ �ִ��� ã�� �۾�
            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (itemSlots[i].BoItem != null)
                {
                    // ������ �����Ѵٸ�
                    if (itemSlots[i].BoItem.sdItem.index == currentClickSlost.sdItem.index)
                    {
                        // �� ������ ��Ƶ�
                        sameItem = itemSlots[i];
                        break;
                    }
                }
            }

            // ���� �������� �����Ѵٸ�
            if (sameItem != null)
            {
                // �������� ������ ������Ʈ
                sameItem.BoItem.amount++;
                uiInventory.IncreaseItem(sameItem.BoItem);
            }
            else
            {
                // �κ��丮�� ���ο� �������� �߰��ϴ� �۾�
                for (int i = 0; i < itemSlots.Count; i++)
                {
                    // ����ִ� ������ ã�Ҵٸ�
                    if (itemSlots[i].BoItem == null)
                    {
                        var boItem = new BoItem(currentClickSlost.sdItem);

                        // ���Կ� �������� ����
                        uiInventory.AddItem(boItem);

                        GameManager.User.boItems.Add(boItem);
                        break;
                    }
                }
            }

            // DB�� ������ �������� ������ ������Ʈ 
            dummyServer.userData.dtoItem = new DtoItem(GameManager.User.boItems);
            GameManager.User.boAccount = new BoAccount(dummyServer.userData.dtoAccount);
            dummyServer.Save();

            // ������ �ݾ��� �κ��丮�� ������Ʈ
            uiInventory.MyGoldUpdate();
            ConfirmFrame.gameObject.SetActive(false);
        }

        /// <summary>
        /// ������ �����ϴ� ���Ե��� �ʱ�ȭ �ϴ� �޼���
        /// </summary>
        public void InitStoreSlots()
        {
            for (int i = 0; i < storeSlots.Count; i++)
            {
                storeSlots[i].Initialize();
            }
        }
    }
}