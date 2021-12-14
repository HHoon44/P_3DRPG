using ProjectChan.DB;
using ProjectChan.Dummy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ProjectChan.UI
{
    public class UIInventory : UIWindow, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Transform itemSlotHolder;       // -> 아이템 슬롯 홀더를 담을 필드
        public Button sortButton;               // -> 정렬 버튼 (아직 안넣음)
        private GraphicRaycaster gr;            // -> UICanvas가 지닌 캔버스 안을 검색하기 위한 레이캐스트
        private ItemSlot dragSlot;              // -> 옮기려는 아이템 슬롯
        private Vector3 dragSlotOriginVec;      // -> 옮기려는 아이템 슬롯의 원 위치

        /// <summary>
        /// => 아이템 슬롯 홀더에 자식으로 존재하는 슬롯들을 담아놓을 공간
        /// </summary>
        public List<ItemSlot> itemSlots { get; private set; } = new List<ItemSlot>();

        public override void Start()
        {
            base.Start();

            // -> UIInventory가 지닌 그래픽 레이캐스트 컴포넌트를 가져온다
            // -> 아이템 슬롯을 담아놓을 홀더를 찾아서 가져온다
            gr = GetComponentInParent<GraphicRaycaster>();
            itemSlotHolder = transform.GetChild(0).GetChild(0);

            // -> 홀더에 존재하는 슬롯들을 리스트에 저장 해놓는 작업
            for (int i = 0; i < itemSlotHolder.childCount; i++)
            {
                itemSlots.Add(itemSlotHolder.GetChild(i).GetComponent<ItemSlot>());
            }

            // -> 리스트에 저장된 슬롯들을 초기화 하는 작업과 Bo데이터가 지닌 아이템 정보를 슬롯에 초기화 하는 작업
            InitItemSlots();
            InitInventory();

            // -> 정렬 버튼 이벤트 바인딩 작업
            sortButton.onClick.AddListener(() =>
            {
                var boItems = GameManager.User.boItems;

                // -> OrderBy 메서드를 이용하여 이름순으로 정렬
                var sortItems = boItems.OrderBy(obj => obj.sdItem?.name).ToList();

                // -> 한번 싹 초기화
                InitItemSlots();

                // -> 정렬한 아이템 개수 만큼 반목문을 돌린다
                for (int i = 0; i < sortItems.Count; i++)
                {
                    itemSlots[i].SetSlot(sortItems[i]);
                    itemSlots[i].BoItem.slotIndex = i;
                }

                // -> 데이터에 변동이 생겼으니 다시 저장
                DummyServer.Instance.userData.dtoItem = new DtoItem(boItems);
                DummyServer.Instance.Save();
            });
        }

        /// <summary>
        /// => 새로운 아이템을 습득시 새로운 아이템을 아이템 슬롯에 추가하거나
        ///    기존의 아이템을 습득시 아이템의 수량을 증가시켜 아이템 슬롯에 재설정하는 메서드
        /// </summary>
        /// <param name="boItem"> 플레이어가 습득한 아이템 정보 </param>
        public void AddItem(BoItem boItem)
        {
            // -> Bo데이터의 슬롯 인덱스가 0보다 크다면 아이템이 존재하는 것 이므로 슬롯 세팅
            if (boItem.slotIndex >= 0)
            {
                itemSlots[boItem.slotIndex].SetSlot(boItem);
                return;
            }

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
            // -> BoItem이 존재한다면 슬롯에 아이템이 존재한다
            if (itemSlots[slotIndex].BoItem != null)
            {
                // -> 슬롯에 존재하는 아이템이 소비 아이템인가 확인
                if (itemSlots[slotIndex].BoItem.sdItem.itemType == Define.ItemType.Expendables)
                {
                    // -> 어떤 스탯에 영향을 미치는지에 따라서 나눔
                    switch (itemSlots[slotIndex].BoItem.sdItem.affectingStats[0])
                    {
                        case "currentHp":
                            /// 사용하면 스탯 올려주고 
                            /// 개수 줄어들어야함 그리고 Dto에 다시 저장해야함
                            boActor.currentHp += itemSlots[slotIndex].BoItem.sdItem.affectingStatsValue[0];

                            if (boActor.currentHp >= boActor.maxHp)
                            {
                                boActor.currentHp = boActor.maxHp;
                            }

                            DiminishAmount();
                            break;

                        case "currentEnergy":
                            boActor.currentEnergy += itemSlots[slotIndex].BoItem.sdItem.affectingStatsValue[0];

                            if (boActor.currentEnergy >= boActor.maxEnergy)
                            {
                                boActor.currentEnergy = boActor.maxEnergy;
                            }

                            DiminishAmount();
                            break;
                    }
                }

                // -> 로컬 함수
                void DiminishAmount()
                {
                    // -> 사용했으니 개수를 감소한다
                    itemSlots[slotIndex].BoItem.amount--;

                    // -> 만약 아이템을 다 소비 했다면
                    if (itemSlots[slotIndex].BoItem.amount == 0)
                    {
                        // -> 기존의 아이템 슬롯의 정보를 제거
                        // -> 새로운 슬롯 정보를 대입
                        GameManager.User.boItems.Remove(itemSlots[slotIndex].BoItem);
                        ItemSlot itemSlot = new ItemSlot();
                        itemSlots[slotIndex].SetSlot(itemSlot.BoItem);
                    }
                    else
                    {
                        itemSlots[slotIndex].AmountUpdate(itemSlots[slotIndex].BoItem);
                    }
                }

                DummyServer.Instance.userData.dtoItem = new DtoItem(GameManager.User.boItems);
                DummyServer.Instance.Save();
            }
            else
            {
                // -> 빈 슬롯이면 종료
                return;
            }
        }

        /// <summary>
        /// => 슬롯에 아이템 정보를 담기전 가지고 있는 슬롯들을 초기화 하는 메서드
        /// </summary>
        private void InitItemSlots()
        {
            for (int i = 0; i < itemSlots.Count; i++)
            {
                itemSlots[i].Initialize();
            }
        }

        /// <summary>
        /// => 게임 매니저가 지닌 BoItem 데이터를 슬롯에 세팅하는 메서드
        /// </summary>
        private void InitInventory()
        {
            // -> 게임매니저에 저장되어있는 BoItem 데이터를 가져온다
            var userBoItems = GameManager.User.boItems;

            for (int i = 0; i < userBoItems.Count; i++)
            {
                AddItem(userBoItems[i]);
            }
        }

        /// <summary>
        /// => 인벤토리에 이미 존재하는 아이템의 수량을 올리는 메서드
        /// </summary>
        /// <param name="boItem"> 개수가 증가된 아이템 데이터 </param>
        public void IncreaseItem(BoItem boItem)
        {
            // -> ItemSlot에 존재하는 AmounUpdate를 통해서 텍스트에 아이템 개수를 재설정한다
            itemSlots[boItem.slotIndex].AmountUpdate(boItem);
        }

        private void Update()
        {
            OnInventoryButton();
        }

        /// <summary>
        /// => 인벤토리 키 입력을 받는 메서드
        /// </summary>
        private void OnInventoryButton()
        {
            if (Input.GetButtonDown(Define.Input.Inventory))
            {
                if (isOpen)
                {
                    Close();
                    return;
                }

                Open();
            }
        }

        /// <summary>
        /// => 드래그를 처음 시작할때 호출되는 메서드
        /// </summary>
        /// <param name="eventData"> 마우스의 정보를 담고 있는 데이터 </param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            // -> 레이캐스트의 결과를 담아놓을 공간
            var results = new List<RaycastResult>();

            gr.Raycast(eventData, results);

            // -> 레이를 통해 얻은 결과의 개수만큼 반복문을 실행
            for (int i = 0; i < results.Count; i++)
            {
                // -> 결과중의 이름에 ItemSlot이 포함된다면
                if (results[i].gameObject.name.Contains("ItemSlot"))
                {
                    // -> 조건을 만족한 오브젝트를 현재 드래그를 할 슬롯에 담아놓는다
                    // -> 현재 위치를 담아놓는다
                    dragSlot = results[i].gameObject.GetComponent<ItemSlot>();
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
            // -> 옮기려는 슬롯이 없다면
            if (dragSlot == null)
            {
                return;
            }

            // -> 옮기는 작업은 아이템 이미지를 옮기는 것이므로 포지션을 현재 마우스 포인터로 설정해준다
            dragSlot.ItemImage.transform.position = eventData.pointerCurrentRaycast.screenPosition;
        }

        /// <summary>
        /// => 드래그가 끝나면 호출되는 메서드
        /// </summary>
        /// <param name="eventData"> 마우스의 정보를 담고 있는 데이터 </param>
        public void OnEndDrag(PointerEventData eventData)
        {
            // -> 드래그를 마춘 지점에서 다시 한번더 레이캐스트를 사용
            var results = new List<RaycastResult>();

            gr.Raycast(eventData, results);

            // -> 이미지를 원 위치로 설정
            dragSlot.ItemImage.transform.position = dragSlotOriginVec;

            // -> 드래그가 끝난 지점에 존재하는 슬롯의 데이터를 담을 변수
            ItemSlot destSlot = null;

            for (int i = 0; i < results.Count; i++)
            {
                // -> 이전의 아이템 슬롯과 같다면
                if (results[i].gameObject == dragSlot.gameObject)
                {
                    continue;
                }

                // -> 위의 조건을 만족하고 이름에 ItemSlot을 포함한다면
                if (results[i].gameObject.name.Contains("ItemSlot"))
                {
                    destSlot = results[i].gameObject.GetComponent<ItemSlot>();
                    break;
                }
            }

            // -> 반복문을 다 돌고도 조건에 맞는 데이터가 없다면
            if (destSlot == null)
            {
                return;
            }

            var boItems = GameManager.User.boItems;

            // -> 이전 슬롯에 존재하는 Bo데이터를 복사
            var tempBoItem = dragSlot.BoItem.ObjcetCopy();

            // -> 옮기려는 슬롯의 Bo데이터는 삭제
            // -> 위에서 복사한 데이터를 저장
            boItems.Remove(dragSlot.BoItem);
            boItems.Add(tempBoItem);

            // -> 목적지에 존재하는 데이터를 옮기려는 슬롯에 세팅
            dragSlot.SetSlot(destSlot.BoItem);
            SetSlotIndex(dragSlot);

            // -> 목적지 슬롯에는 옮겼던 슬롯의 데이터를 저장
            destSlot.SetSlot(tempBoItem);
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