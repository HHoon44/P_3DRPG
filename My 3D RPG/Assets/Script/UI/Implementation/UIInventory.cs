using ProjectChan.DB;
using ProjectChan.Dummy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectChan.UI
{
    /// <summary>
    /// => 유저 인벤토리가 지닐 클래스
    /// </summary>
    public class UIInventory : UIWindow, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public Button sortButton;               // -> 정렬 버튼S
        public TextMeshProUGUI playerGold;      // -> 플레이어가 지닌 금액

        private Transform itemSlotHolder;       // -> 아이템 슬롯 홀더를 담을 필드
        private GraphicRaycaster gr;            // -> 컨버스안을 탐지하기 위한 레이캐스트
        private ItemSlot dragSlot;              // -> 옮기려는 아이템 슬롯
        private Vector3 dragSlotOriginVec;      // -> 옮기려는 아이템 슬롯의 원 위치

        /// <summary>
        /// => 아이템 슬롯 홀더에 자식으로 존재하는 슬롯들을 담아놓을 공간
        /// </summary>
        public List<ItemSlot> itemSlots { get; private set; } = new List<ItemSlot>();

        public override void Start()
        {
            base.Start();

            gr = GetComponentInParent<GraphicRaycaster>();
            itemSlotHolder = transform.GetChild(0).GetChild(0);

            // -> 홀더에 존재하는 슬롯들을 리스트에 저장 해놓는 작업
            for (int i = 0; i < itemSlotHolder.childCount; i++)
            {
                itemSlots.Add(itemSlotHolder.GetChild(i).GetComponent<ItemSlot>());
            }

            // -> UIInventory에 있는 슬롯들을 Initialize 해줍니다!
            InitItemSlots();

            // -> Bo에 있는 아이템 정보를 인벤토리에 세팅 해줍니다!
            InitInventory();
            
            // -> 현재 소지한 골드를 인벤토리에 표시해줍니다!
            MyGoldUpdate();

            // -> 정렬 버튼 이벤트 바인딩 작업
            sortButton.onClick.AddListener(() =>
            {
                var boItems = GameManager.User.boItems;

                // -> OrderBy를 이용하여 이름순으로 정렬합니다!
                var sortItems = boItems.OrderBy(obj => obj.sdItem?.name).ToList();

                // -> 정렬한 뒤에 존재하는 슬롯들을 재초기화 해줍니다!
                InitItemSlots();

                // -> 인벤토리 슬롯의 다시 세팅 합니다!
                for (int i = 0; i < sortItems.Count; i++)
                {
                    itemSlots[i].SetSlot(sortItems[i]);
                    itemSlots[i].BoItem.slotIndex = i;
                }

                // -> 데이터에 변동이 생겼으니 저장합니다!
                DummyServer.Instance.userData.dtoItem = new DtoItem(boItems);
                DummyServer.Instance.Save();
            });
        }

        private void Update()
        {
            OnInventoryButton();
        }

        /// <summary>
        /// => 새로운 아이템을 습득시 새로운 아이템을 아이템 슬롯에 추가하거나
        ///    기존의 아이템을 습득시 아이템의 수량을 증가시켜 아이템 슬롯에 재설정하는 메서드
        /// </summary>
        /// <param name="boItem"> 플레이어가 습득한 아이템 정보 </param>
        public void AddItem(BoItem boItem)
        {
            // -> Bo데이터의 슬롯 인덱스가 0보다 크다면!
            if (boItem.slotIndex >= 0)
            {
                // -> 이미 존재한 아이템이므로 슬롯 세팅을 해줍니다!
                itemSlots[boItem.slotIndex].SetSlot(boItem);
                return;
            }

            // -> 존재하지 않는 아이템이라면 빈 슬롯에 세팅해줍니다!
            for (int i = 0; i < itemSlots.Count; i++)
            {
                // -> BoItem이 비어있는 Slot이 존재한다면 아직 아이템이 없는 슬롯이므로
                if (itemSlots[i].BoItem == null)
                {
                    boItem.slotIndex = i;
                    itemSlots[boItem.slotIndex].SetSlot(boItem);
                    break;
                }
            }
        }

        /// <summary>
        /// => 아이템을 사용한다고 할 때 호출할 메서드
        /// </summary>
        /// <param name="boActor"> 현재 아이템을 사용할 액터의 정보 </param>
        /// <param name="slotIndex"> 몇번째 키입력인지 </param>
        public void UsedItem(BoActor boActor, int slotIndex)
        {
            // -> 슬롯에 BoItem이 존재한다면!
            if (itemSlots[slotIndex].BoItem != null)
            {
                // -> 아이템이 들어있는 슬롯 이므로
                // -> 슬롯에 존재하는 아이템이 소비 아이템인지 확인합니다!
                if (itemSlots[slotIndex].BoItem.sdItem.itemType == Define.ItemType.Expendables)
                {
                    // -> 어떤 스탯에 영향을 미치는지에 따라서 나눕니다!
                    switch (itemSlots[slotIndex].BoItem.sdItem.affectingStats[0])
                    {
                        case "currentHp":
                            boActor.currentHp += itemSlots[slotIndex].BoItem.sdItem.affectingStatsValue[0];

                            if (boActor.currentHp >= boActor.maxHp)
                            {
                                boActor.currentHp = boActor.maxHp;
                            }
                            break;

                        case "currentEnergy":
                            boActor.currentEnergy += itemSlots[slotIndex].BoItem.sdItem.affectingStatsValue[0];

                            if (boActor.currentEnergy >= boActor.maxEnergy)
                            {
                                boActor.currentEnergy = boActor.maxEnergy;
                            }
                            break;
                    }

                    DiminishAmount();
                }

                // -> 로컬 함수
                void DiminishAmount()
                {
                    // -> 사용했으니 개수를 감소한다
                    itemSlots[slotIndex].BoItem.amount--;

                    // -> 만약 아이템을 다 소비 했다면
                    if (itemSlots[slotIndex].BoItem.amount == 0)
                    {
                        // -> 다 사용한 슬롯에 존재하는 Item의 정보를 제거 해줍니다!
                        // -> 그리고 새로운 아이템 슬롯 정보를 만들어서 아이템 정보를 제거한 슬롯에 넣어줍니다!
                        GameManager.User.boItems.Remove(itemSlots[slotIndex].BoItem);
                        ItemSlot itemSlot = new ItemSlot();
                        itemSlots[slotIndex].SetSlot(itemSlot.BoItem);
                    }
                    else
                    {
                        // -> 현재 개수를 업데이트 해줍니다!
                        itemSlots[slotIndex].AmountUpdate(itemSlots[slotIndex].BoItem);
                    }
                }

                // -> 저장해줍니다!
                DummyServer.Instance.userData.dtoItem = new DtoItem(GameManager.User.boItems);
                DummyServer.Instance.Save();
            }
            else
            {
                // -> 빈 슬롯이라면 종료합니다!
                return;
            }
        }

        /// <summary>
        /// => 인벤토리에 존재하는 슬롯을 한번 초기화 하는 메서드
        /// </summary>
        private void InitItemSlots()
        {
            for (int i = 0; i < itemSlots.Count; i++)
            {
                itemSlots[i].Initialize();
            }
        }

        /// <summary>
        /// => 슬롯에 아이템을 세팅하는 메서드
        /// </summary>
        private void InitInventory()
        {
            var userBoItems = GameManager.User.boItems;

            for (int i = 0; i < userBoItems.Count; i++)
            {
                AddItem(userBoItems[i]);
            }
        }

        /// <summary>
        /// => 이미 슬롯에 존재하는 아이템이라면 수량을 증가시키는 메서드
        /// </summary>
        /// <param name="boItem"> 개수가 증가된 아이템 데이터 </param>
        public void IncreaseItem(BoItem boItem)
        {
            // -> 이미 존재하는 슬롯에 아이템 개수를 증가시킵니다!
            itemSlots[boItem.slotIndex].AmountUpdate(boItem);
        }

        /// <summary>
        /// => 인벤토리 키 입력을 받는 메서드
        /// </summary>
        private void OnInventoryButton()
        {
            if (Input.GetButtonDown(Define.Input.Inventory))
            {
                // -> 켜진 상태에서 키입력이면 창을 꺼줍니다!
                if (isOpen)
                {
                    Close();
                    return;
                }

                Open();
            }
        }

        /// <summary>
        /// => 소지한 금액이 바뀌면 업데이트 해주는 메서드
        /// </summary>
        public void MyGoldUpdate()
        {
            playerGold.text = GameManager.User.boAccount.gold.ToString();
        }

        /// <summary>
        /// => 드래그를 처음 시작할때 호출되는 메서드
        /// </summary>
        /// <param name="eventData"> 마우스의 정보를 담고 있는 데이터 </param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // -> 레이캐스트의 결과를 담아놓을 공간을 만들어줍니다!
            var results = new List<RaycastResult>();

            // -> 그래픽 레이캐스트를 이용하여 캔버스 위의 UI들을 확인합니다!
            gr.Raycast(eventData, results);

            for (int i = 0; i < results.Count; i++)
            {
                // -> 결과들의 이름에서 ItmeSlot이 포함되어있다면!
                if (results[i].gameObject.name.Contains("ItemSlot"))
                {
                    // -> 현재 드래그를 할 슬롯에 담아놓습니다!
                    dragSlot = results[i].gameObject.GetComponent<ItemSlot>();

                    // -> 그리고 현재 위치를 담아놓습니다!
                    dragSlotOriginVec = dragSlot.ItemImage.transform.position;
                    break;
                }
            }
        }

        /// <summary>
        /// => 드래그를 하고 있을때 호출되는 메서드
        /// </summary>
        /// <param name="eventData"> 마우스의 정보를 담고 있는 데이터 </param>
        public void OnDrag(PointerEventData eventData)
        {
            // -> 옮기려는 슬롯이 없다면!
            if (dragSlot == null)
            {
                return;
            }

            // -> 슬롯을 옮기는 작업은 아이템 이미지를 옮기는 것이므로 
            //    포지션을 현재 마우스 포인터로 설정합니다!
            dragSlot.ItemImage.transform.position = eventData.pointerCurrentRaycast.screenPosition;
        }

        /// <summary>
        /// => 드래그가 끝나면 호출되는 메서드
        /// </summary>
        /// <param name="eventData"> 마우스의 정보를 담고 있는 데이터 </param>
        public void OnEndDrag(PointerEventData eventData)
        {
            // -> 드래그를 끝낸 지점에서 한번 더 레이 캐스트를 사용합니다!
            var results = new List<RaycastResult>();

            gr.Raycast(eventData, results);

            // -> 이미지를 아까 저장한 원 위치로 설정해줍니다!
            dragSlot.ItemImage.transform.position = dragSlotOriginVec;

            // -> 드래그를 끝낸 지점에 존재하는 슬롯 데이터를 담아놓을 변수입니다!
            ItemSlot destSlot = null;

            for (int i = 0; i < results.Count; i++)
            {
                // -> 이전의 아이템 슬롯과 같다면!
                if (results[i].gameObject == dragSlot.gameObject)
                {
                    continue;
                }

                // -> 끝낸 지점의 결과중 ItemSlot을 포함한다면!
                if (results[i].gameObject.name.Contains("ItemSlot"))
                {
                    // -> 그 지점의 드래그 슬롯 정보를 변수에 담아 놓습니다!
                    destSlot = results[i].gameObject.GetComponent<ItemSlot>();
                    break;
                }
            }

            // -> 조건에 맞는 데이터가 없다면!
            if (destSlot == null)
            {
                return;
            }

            var boItems = GameManager.User.boItems;

            // -> 이전 슬롯에 존재하는 아이템 데이터를 복사합니다!
            var tempBoItem = dragSlot.BoItem.ObjcetCopy();

            // -> 드래그한 슬롯의 아이템 데이터는 GM에 존재하는 Bo 데이터 목록에서 제거합니다!
            boItems.Remove(dragSlot.BoItem);

            // -> 그리고 위에서 복사한 데이터를 목록에 추가합니다!
            boItems.Add(tempBoItem);

            // -> 목적지에 존재하는 아이템 데이터를 드래그 슬롯에 세팅 합니다!
            dragSlot.SetSlot(destSlot.BoItem);

            // -> 슬롯 인덱스를 재설정합니다!
            SetSlotIndex(dragSlot);

            // -> 목적지 슬롯에는 드래그 슬롯에 있던 아이템 데이터를 저장합니다!
            destSlot.SetSlot(tempBoItem);

            // -> 슬롯 인덱스를 재설정합니다!
            SetSlotIndex(destSlot);

            DummyServer.Instance.userData.dtoItem = new DtoItem(boItems);
            DummyServer.Instance.Save();

            void SetSlotIndex(ItemSlot itemSlot)
            {
                if (itemSlot.BoItem == null)
                {
                    return;
                }

                // -> 정규식을 이용하여 오브젝트 이름에서 SlotIndex를 가져오는 작업
                var index = Regex.Replace(itemSlot.gameObject.name, @"[^\d]", "");
                itemSlot.BoItem.slotIndex = int.Parse(index);
            }
        }
    }
}