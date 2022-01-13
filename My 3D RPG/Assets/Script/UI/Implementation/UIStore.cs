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
    /// => 상점 UI를 관리할 클래스
    /// </summary>
    public class UIStore : UIWindow, IPointerClickHandler
    {
        public TextMeshProUGUI storeName;       // -> 현재 상점의 이름
        public Transform ConfirmFrame;          // -> 아이템 구매 여부를 물어보는 창
        public Button closeBtn;                 // -> 상점 창 닫기 버튼

        private Transform storeSlotHolder;      // -> 상점 슬롯이 담겨잇는 홀더
        private GraphicRaycaster gr;            // -> 컨버스안을 탐지하기 위한 레이캐스터
        private ItemSlot currentClickSlost;     // -> 현재 플레이어가 클릭한 상점 슬롯 
        private Button itemYesBtn;              // -> 네 살겁니다 버튼
        private Button itemNoBtn;               // -> 아니요 안살겁니다 버튼
        private BoNPC boNPC;                    // -> 현재 상점의 주인

        /// <summary>
        /// => 상점 슬롯 홀더에 자식으로 존재하는 슬롯들을 담아놓을 공간
        /// </summary>
        public List<ItemSlot> storeSlots { get; private set; } = new List<ItemSlot>();

        public override void Start()
        {
            gr = GetComponentInParent<GraphicRaycaster>();

            // -> 구매 여부를 물어보는 창에 존재하는 Yes버튼과 No버튼을 가져 옵니다!
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

        /// <summary>
        /// => 상점을 오픈할 때 호출될 메서드
        /// </summary>
        /// <param name="boNPC"> 현재 상점 NPC 데이터 </param>
        public void Open(BoNPC boNPC)
        {
            this.boNPC = boNPC;

            storeName.text = boNPC.sdNPC.name + "의 상점!";

            // -> 상점을 세팅합니다!
            for (int i = 0; i < storeSlots.Count; i++)
            {
                // -> 상점 NPC가 판매할 아이템을 지니고 있다면!
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
        /// => 상점에 존재하는 슬롯들을 초기화 하는 메서드
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
            // -> 레이캐스트 결과를 담아놓을 공간입니다!
            var results = new List<RaycastResult>();

            gr.Raycast(eventData, results);

            for (int i = 0; i < results.Count; i++)
            {
                // -> 결과들의 이름중에 StroeSlot을 포함하고 있다면!
                if (results[i].gameObject.name.Contains("StoreSlot"))
                {
                    // -> 현재 클릭한 슬롯에 i번째 데이터를 담아둡니다!
                    currentClickSlost = results[i].gameObject.GetComponent<ItemSlot>();
                    break;
                }
            }

            // -> 현재 클릭한 슬롯이 빈 슬롯이 아니라면!
            if (currentClickSlost.sdItem != null)
            {
                // -> 구매 여부를 물어볼 창을 킵니다!
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

            // -> 만약 아이템 구매로 인해 소지금이 0보다 작거나 같다면!
            if (dtoAccount.gold <= 0)
            {
                // -> 0으로 초기화 해줍니다!
                dtoAccount.gold = 0;
            }

            // -> 현재 인벤토리를 확인 해야하므로 인벤토리에 담겨있는 슬롯을 가져옵니다!
            var itemSlots = uiInventory.itemSlots;

            // -> 구매한 아이템이 이미 인벤토리에 존재 한다면 
            //    아이템이 존재하는 슬롯의 데이터를 넣어놓을 공간입니다!
            ItemSlot selectItem = null;

            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (itemSlots[i].BoItem != null)
                {
                    // -> 구매한 아이템이 슬롯에 존재한다면!
                    if (itemSlots[i].BoItem.sdItem.index == currentClickSlost.sdItem.index)
                    {
                        // -> 그 슬롯을 담아놓습니다!
                        selectItem = itemSlots[i];
                        break;
                    }
                }
            }

            if (selectItem != null)
            {
                // -> 존재한다면 개수를 올려줍니다!
                selectItem.BoItem.amount++;
                uiInventory.IncreaseItem(selectItem.BoItem);
            }
            else
            {
                // -> 존재하지 않는 아이템이라면 인벤토리에 새롭게 추가해줍니다!
                for (int i = 0; i < itemSlots.Count; i++)
                {
                    if (itemSlots[i].BoItem == null)
                    {
                        // -> 비어있는 슬롯에 구매한 아이템을 추가해 줍니다!
                        var boItem = new BoItem(currentClickSlost.sdItem);
                        uiInventory.AddItem(boItem);
                        GameManager.User.boItems.Add(boItem);
                        break;
                    }
                }
            }

            // -> DB에 구매한 아이템에 대한 정보를 업데이트 해줍니다!
            dummyServer.userData.dtoItem = new DtoItem(GameManager.User.boItems);
            GameManager.User.boAccount = new BoAccount(dummyServer.userData.dtoAccount);
            dummyServer.Save();

            // -> 소지한 금액을 업데이트 해줍니다!
            uiInventory.MyGoldUpdate();
            ConfirmFrame.gameObject.SetActive(false);
        }
    }
}