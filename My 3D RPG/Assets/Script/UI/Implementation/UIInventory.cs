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
    /// 인벤토리를 관리하는 클래스
    /// </summary>
    public class UIInventory : UIWindow, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // public
        public Button sortButton;               // 인벤토리 정렬 버튼
        public TextMeshProUGUI playerGold;      // 플레이어가 지닌 금액

        // private
        private Transform itemSlotHolder;       // 아이템 슬롯 홀더를 담을 필드
        private GraphicRaycaster gr;            // 컨버스안을 탐지하기 위한 레이캐스트
        private ItemSlot dragSlot;              // 옮기려는 아이템 슬롯
        private Vector3 dragSlotOriginVec;      // 옮기려는 아이템 슬롯의 원 위치

        /// <summary>
        /// 인벤토리에 존재하는 아이템 슬롯을 저장해놓을 리스트
        /// </summary>
        public List<ItemSlot> itemSlots { get; private set; } = new List<ItemSlot>();

        public override void Start()
        {
            base.Start();

            gr = GetComponentInParent<GraphicRaycaster>();
            itemSlotHolder = transform.GetChild(0).GetChild(0);

            // 아이템 슬롯을 리스트에 저장하는 작업
            for (int i = 0; i < itemSlotHolder.childCount; i++)
            {
                itemSlots.Add(itemSlotHolder.GetChild(i).GetComponent<ItemSlot>());
            }

            // 가져온 슬롯을 초기화
            InitItemSlots();

            // 플레이어가 소지한 아이템을 인벤토리에 세팅
            InitInventory();
            MyGoldUpdate();

            // 정렬 버튼 이벤트 바인딩 작업
            sortButton.onClick.AddListener(() =>
            {
                var boItems = GameManager.User.boItems;

                // 이름순으로 정렬
                var sortItems = boItems.OrderBy(obj => obj.sdItem?.name).ToList();

                // 정렬한 뒤에 슬롯을 초기화
                InitItemSlots();

                // 업데이트 된 아이템 정보를 다시 슬롯에 세팅하는 작업
                for (int i = 0; i < sortItems.Count; i++)
                {
                    itemSlots[i].SetSlot(sortItems[i]);
                    itemSlots[i].BoItem.slotIndex = i;
                }

                // Dto에 아이템 데이터 업데이트
                DummyServer.Instance.userData.dtoItem = new DtoItem(boItems);
                DummyServer.Instance.Save();
            });
        }

        private void Update()
        {
            OnInventoryButton();
        }

        /// <summary>
        /// 새로운 아이템을 습득시 새로운 아이템을 아이템 슬롯에 추가하거나
        /// 기존의 아이템을 습득시 아이템의 수량을 증가시켜 아이템 슬롯에 재설정하는 메서드
        /// </summary>
        /// <param name="boItem"> 플레이어가 습득한 아이템 정보 </param>
        public void AddItem(BoItem boItem)
        {
            // 존재하는 아이템 이라면
            if (boItem.slotIndex >= 0)
            {
                // 아이템이 존재하는 슬롯에 업데이트
                itemSlots[boItem.slotIndex].SetSlot(boItem);
                return;
            }

            // 새로운 아이템을 인벤토리에 추가하는 작업
            for (int i = 0; i < itemSlots.Count; i++)
            {
                // 빈 슬롯을 찾아 새로운 아이템을 추가
                if (itemSlots[i].BoItem == null)
                {
                    boItem.slotIndex = i;
                    itemSlots[boItem.slotIndex].SetSlot(boItem);
                    break;
                }
            }
        }

        /// <summary>
        /// 아이템을 사용 요청 시, 아이템을 사용하는 메서드
        /// </summary>
        /// <param name="boActor">      현재 아이템을 사용할 액터의 정보 </param>
        /// <param name="slotIndex">    몇번째 키입력인지 </param>
        public void UsedItem(BoActor boActor, int slotIndex)
        {
            // 요청한 슬롯에 아이템이 존재한다면
            if (itemSlots[slotIndex].BoItem != null)
            {
                // 슬롯의 아이템이 소비 아이템이라면
                if (itemSlots[slotIndex].BoItem.sdItem.itemType == Define.ItemType.Expendables)
                {
                    // 아이템이 영향을 미치는 스탯에 따라 나눔
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

                    // 아이템 개수 감소
                    DiminishAmount();
                }

                // 로컬 함수
                void DiminishAmount()
                {
                    // 사용된 슬롯의 아이템 개수를 감소
                    itemSlots[slotIndex].BoItem.amount--;

                    // 슬롯의 아이템을 다 소비했다면
                    if (itemSlots[slotIndex].BoItem.amount == 0)
                    {
                        // 슬롯에 존재하는 아이템 정보를 제거하고 빈 슬롯으로 세팅한다
                        GameManager.User.boItems.Remove(itemSlots[slotIndex].BoItem);
                        ItemSlot itemSlot = new ItemSlot();
                        itemSlots[slotIndex].SetSlot(itemSlot.BoItem);
                    }
                    else
                    {
                        // 슬롯에 수량을 업데이트
                        itemSlots[slotIndex].AmountUpdate(itemSlots[slotIndex].BoItem);
                    }
                }

                DummyServer.Instance.userData.dtoItem = new DtoItem(GameManager.User.boItems);
                DummyServer.Instance.Save();
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 인벤토리에 존재하는 모든 슬롯을 초기화 하는 메서드
        /// </summary>
        private void InitItemSlots()
        {
            for (int i = 0; i < itemSlots.Count; i++)
            {
                itemSlots[i].Initialize();
            }
        }

        /// <summary>
        /// 슬롯에 소지한 아이템을 세팅하는 
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
        /// 중복 아이템 획득 시, 아이템 수량을 증가하는 메서드
        /// </summary>
        /// <param name="boItem"> 개수가 증가된 아이템 데이터 </param>
        public void IncreaseItem(BoItem boItem)
        {
            // 수량 업데이트
            itemSlots[boItem.slotIndex].AmountUpdate(boItem);
        }

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
        /// 소지한 금액을 업데이트 하는 메서드
        /// </summary>
        public void MyGoldUpdate()
        {
            playerGold.text = GameManager.User.boAccount.gold.ToString();
        }

        /// <summary>
        /// 드래그 시작 시, 호출되는 메서드
        /// </summary>
        /// <param name="eventData"> 마우스의 정보를 담고 있는 데이터 </param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // 레이 캐스트 결과를 담아놓을 리스트
            var results = new List<RaycastResult>();

            // 캔버스 UI들을 results에 담는다
            gr.Raycast(eventData, results);

            for (int i = 0; i < results.Count; i++)
            {
                // 감지한 UI에서 ItemSlot이 있다면
                if (results[i].gameObject.name.Contains("ItemSlot"))
                {
                    // i번째 ItemSlot을 드래그 할 슬롯으로 정함
                    dragSlot = results[i].gameObject.GetComponent<ItemSlot>();

                    // 정한 슬롯이 빈 슬롯이라면
                    if (dragSlot.BoItem == null)
                    {
                        dragSlot = null;
                        return;
                    }
                    
                    // i번째 ItemSlot의 위치를 저장
                    dragSlotOriginVec = dragSlot.ItemImage.transform.position;
                    break;
                }
            }
        }

        /// <summary>
        /// 드래그 중 호출되는 메서드
        /// </summary>
        /// <param name="eventData"> 마우스의 정보를 담고 있는 데이터 </param>
        public void OnDrag(PointerEventData eventData)
        {
            // 드래그 슬롯이 없다면
            if (dragSlot == null)
            {
                return;
            }

            // 슬롯을 옮기는 작업은 아이템의 이미지를 옮기는 작업이므로, 이미지의 포지션을 마우스로 지정
            dragSlot.ItemImage.transform.position = eventData.pointerCurrentRaycast.screenPosition;
        }

        /// <summary>
        /// 드래그가 끝나면 호출되는 메서드
        /// </summary>
        /// <param name="eventData"> 마우스의 정보를 담고 있는 데이터 </param>
        public void OnEndDrag(PointerEventData eventData)
        {
            // 드래그 슬롯이 없다면
            if (dragSlot == null)
            {
                return;
            }

            // 종료 지점의 레이캐스트 결과를 담을 리스트
            var results = new List<RaycastResult>();

            gr.Raycast(eventData, results);

            // 옮기던 아이템 이미지를 원 위치로 설정
            dragSlot.ItemImage.transform.position = dragSlotOriginVec;

            // 종료 지점의 ItemSlot의 정보를 담을 공간
            ItemSlot destSlot = null;

            for (int i = 0; i < results.Count; i++)
            {
                // 레이캐스르로 감지한 슬롯중, 드래그 슬롯과 같은 슬롯이 있다면
                if (results[i].gameObject == dragSlot.gameObject)
                {
                    continue;
                }

                // 감지한 슬롯중, 드래그 슬롯과는 다른 ItemSlot이 있다면
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