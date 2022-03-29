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
    /// 상점 창을 관리할 클래스
    /// </summary>
    public class UIStore : UIWindow, IPointerClickHandler
    {
        // public
        public TextMeshProUGUI storeName;       // 현재 상점의 이름
        public Transform ConfirmFrame;          // 아이템 구매 여부를 물어보는 창
        public Button buyBtn;                   // 산다 버튼
        public Button noBuyBtn;                 // 안산다 버튼
        public Button closeBtn;                 // 상점 닫기 버튼

        // private
        private BoNPC boNPC;                    // 현재 상점의 주인
        private Transform storeSlotHolder;      // 상점 슬롯이 담겨잇는 홀더
        private GraphicRaycaster gr;            // 컨버스안을 탐지하기 위한 레이캐스터
        private ItemSlot currentClickSlost;     // 현재 플레이어가 클릭한 상점 슬롯 

        /// <summary>
        /// 상점 창에 존재하는 아이템 슬롯
        /// </summary>
        public List<ItemSlot> storeSlots { get; private set; } = new List<ItemSlot>();

        public override void Start()
        {
            gr = GetComponentInParent<GraphicRaycaster>();

            // 산다/안산다/창 닫기 버튼에 이벤트 바인딩
            buyBtn.onClick.AddListener(() => { OnBuyButton(); });
            noBuyBtn.onClick.AddListener(() => { ConfirmFrame.gameObject.SetActive(false); });
            closeBtn.onClick.AddListener(() => { Close(); });

            // 아이템 슬롯을 지닌 홀더를 가져옴
            storeSlotHolder = transform.GetChild(0).GetChild(0);

            // 홀더에 존재하는 아이템 슬롯을 모두 List에 담아놓는 작업
            for (int i = 0; i < storeSlotHolder.childCount; i++)
            {
                storeSlots.Add(storeSlotHolder.GetChild(i).GetComponent<ItemSlot>());
            }

            // 담아둔 아이템 슬롯을 초기화
            InitStoreSlots();

            base.Start();
        }

        /// <summary>
        /// 활성화된 상점 창을 세팅하는 메서드
        /// </summary>
        /// <param name="boNPC"> 현재 상점 NPC 데이터 </param>
        public void Open(BoNPC boNPC)
        {
            var sdItems = GameManager.SD.sdItems;

            // 상점 NPC의 데이터
            this.boNPC = boNPC;
            storeName.text = boNPC.sdNPC.name + "의 상점!";

            // 담아둔 아이템 슬롯에 아이템을 세팅하는 작업
            for (int i = 0; i < storeSlots.Count; i++)
            {
                // 현재 NPC가 판매하는 아이템이 있다면
                if (boNPC.sdNPC.storeItem.Length - 1 >= i)
                {
                    // 판매하는 아이템의 기획 데이터를 가져와 Bo데이터로 만듬
                    var boItem = sdItems.Where(obj => obj.index == boNPC.sdNPC.storeItem[i])?.SingleOrDefault();

                    // Bo데이터를 아이템 슬롯에 세팅
                    storeSlots[i].SetSlot(boItem);
                }
                else
                {
                    storeSlots[i].SetSlot();
                }
            }
            base.Open();

            // 상점 창이 활성화 되면 인벤토리 창도 활성화
            UIWindowManager.Instance.GetWindow<UIInventory>().Open();
        }

        public void Close()
        {
            boNPC.actor.isPlayerAction = false;
            boNPC.actor = null;

            base.Close();
        }

        /// <summary>
        /// 아이템이 존재하는 슬롯 클릭 시 호출되는 메서드
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            // 레이캐스트 결과를 담을 공간
            var results = new List<RaycastResult>();

            // 그래픽 레이캐스트로 이벤트 지점의 UI들을 List에 담아둠
            gr.Raycast(eventData, results);

            // 담아둔 결과물에서 StoreSlot을 찾는 작업
            for (int i = 0; i < results.Count; i++)
            {
                // StoreSlot이 있다면
                if (results[i].gameObject.name.Contains("StoreSlot"))
                {
                    // 유저가 클릭한 슬롯의 정보를 담아둠
                    currentClickSlost = results[i].gameObject.GetComponent<ItemSlot>();
                    break;
                }
            }

            // 클릭한 슬롯이 존재한다면
            if (currentClickSlost.sdItem != null)
            {
                // 구매 여부를 물어보는 창을 띄움
                ConfirmFrame.gameObject.SetActive(true);
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 구매 버튼에 바인딩될 메서드
        /// </summary>
        private void OnBuyButton()
        {
            var dummyServer = DummyServer.Instance;
            var uiInventory = UIWindowManager.Instance.GetWindow<UIInventory>();

            var dtoAccount = dummyServer.userData.dtoAccount;

            // 소지 금액이 부족하다면
            if (dtoAccount.gold < currentClickSlost.sdItem.price)
            {
                ConfirmFrame.gameObject.SetActive(false);
                return;
            }

            // 소지한 금액에서 아이템 가격을 뺌
            dtoAccount.gold -= currentClickSlost.sdItem.price;

            // 구매 이후 소지금이 0이하라면
            if (dtoAccount.gold <= 0)
            {
                dtoAccount.gold = 0;
            }

            // 인벤토리에 존재하는 슬롯들
            var itemSlots = uiInventory.itemSlots;

            // 구매한 아이템이 이미 슬롯에 있다면
            // 슬롯을 담아놓을 공간
            ItemSlot sameItem = null;

            // 구매한 아이템을 지닌 슬롯이 있는지 찾는 작업
            for (int i = 0; i < itemSlots.Count; i++)
            {
                if (itemSlots[i].BoItem != null)
                {
                    // 슬롯이 존재한다면
                    if (itemSlots[i].BoItem.sdItem.index == currentClickSlost.sdItem.index)
                    {
                        // 그 슬롯을 담아둠
                        sameItem = itemSlots[i];
                        break;
                    }
                }
            }

            // 같은 아이템이 존재한다면
            if (sameItem != null)
            {
                // 아이템의 개수를 업데이트
                sameItem.BoItem.amount++;
                uiInventory.IncreaseItem(sameItem.BoItem);
            }
            else
            {
                // 인벤토리에 새로운 아이템을 추가하는 작업
                for (int i = 0; i < itemSlots.Count; i++)
                {
                    // 비어있는 슬롯을 찾았다면
                    if (itemSlots[i].BoItem == null)
                    {
                        var boItem = new BoItem(currentClickSlost.sdItem);

                        // 슬롯에 아이템을 저장
                        uiInventory.AddItem(boItem);

                        GameManager.User.boItems.Add(boItem);
                        break;
                    }
                }
            }

            // DB에 구매한 아이템의 정보를 업데이트 
            dummyServer.userData.dtoItem = new DtoItem(GameManager.User.boItems);
            GameManager.User.boAccount = new BoAccount(dummyServer.userData.dtoAccount);
            dummyServer.Save();

            // 소지한 금액을 인벤토리에 업데이트
            uiInventory.MyGoldUpdate();
            ConfirmFrame.gameObject.SetActive(false);
        }

        /// <summary>
        /// 상점에 존재하는 슬롯들을 초기화 하는 메서드
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