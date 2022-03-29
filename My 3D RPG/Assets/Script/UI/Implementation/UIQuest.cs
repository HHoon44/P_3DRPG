using ProjectChan.DB;
using ProjectChan.Dummy;
using ProjectChan.NetWork;
using ProjectChan.SD;
using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ProjectChan.Define.Quest;

namespace ProjectChan.UI
{
    /// <summary>
    /// 퀘스트 창의 UI를 관리하는 클래스
    /// </summary>
    public class UIQuest : UIWindow
    {
        // public
        public QuestWindow currentWindow;           // 현재 윈도우 창
        public QuestTab currentTab;                 // 현재 탭 창

        [Header("Quest List Window Filed")]
        public Button progressTabBtn;               // 진행 탭 버튼
        public Button completedTabBtn;              // 완료 탭 버튼
        public Transform ContentsHolder;            // 스크롤뷰에 존재하는 컨텐츠 홀더
        public Transform listWindow;                // 퀘스트 리스트 창

        [Header("Quest Info Window Filed")]
        public Button BackBtn;                      // 정보 창에 존재하는 뒤로 가는 버튼
        public TextMeshProUGUI infoTitle;           // 진행중인 퀘스트의 타이틀
        public TextMeshProUGUI infoDescription;     // 진행중인 퀘스트이 내용
        public TextMeshProUGUI questDetail;         // 진행중인 퀘스트의 디테일 내용
        public Transform infoWindow;                // 퀘스트 정보 창

        [Header("Quest Order Window Filed")]
        public Button acceptBtn;                    // 수락 버튼
        public Button refuseBtn;                    // 거절 버튼
        public TextMeshProUGUI orderTitle;          // 퀘스트 타이틀
        public TextMeshProUGUI orderDescription;    // 퀘스트 내용
        public TextMeshProUGUI acceptBtnTitle;      // 수락 버튼이 지닌 텍스트 컴포넌트
        public Transform orderWindow;               // 퀘스트 오더 창
        public OrderQuestType orderQuestType;        // 오더창에 세팅할 퀘스트 타입

        /// <summary>
        /// 퀘스트 리스트 창에서 사용중인 퀘스트 슬롯
        /// </summary>
        private List<QuestSlot> slots = new List<QuestSlot>();

        /// <summary>
        /// 유저가 진행중인 퀘스트의 디테일 목록
        /// </summary>
        private Dictionary<int, int[]> details = new Dictionary<int, int[]>();

        public override void Start()
        {
            base.Start();

            acceptBtnTitle = acceptBtn.GetComponentInChildren<TextMeshProUGUI>();

            // 리스트 창의 탭 버튼에 이벤트를 바인딩
            progressTabBtn.onClick.AddListener(() => { OnClickTab(QuestTab.Progress); });
            completedTabBtn.onClick.AddListener(() => { OnClickTab(QuestTab.Completed); });
        }

        private void Update()
        {
            if (Input.GetButtonDown(Define.Input.Quest))
            {
                if (isOpen)
                {
                    // 창 닫기
                    Close(true);
                    return;
                }

                // 기본 퀘스트 창인 리스트 창을 활성화
                Open(QuestWindow.List);
            }
        }

        /// <summary>
        /// 요청한 퀘스트 창 타입에 따라 창을 여는 메서드
        /// </summary>
        /// <param name="questWindow">  현재 열고자 하는 창 </param>
        /// <param name="sdQuest">      퀘스트 창에 세팅할 퀘스트 </param>
        public void Open(QuestWindow questWindow, SDQuest sdQuest = null)
        {
            // 다른 퀘스트 창이 열린 상태에서 정보 창이 아닌 다른 창을 연다면
            if (isOpen && questWindow != QuestWindow.Info)
            {
                // 현재 창이 정보 창이 아니라면
                if (currentWindow != QuestWindow.Info)
                {
                    return;
                }
            }

            // 활성화 하려는 윈도우 창 타입을 저장
            currentWindow = questWindow;

            // 윈도우 창 타입에 따라 퀘스트 창을 세팅
            switch (currentWindow)
            {
                case QuestWindow.Order:
                    SetOrderWindow(sdQuest);
                    orderWindow.gameObject.SetActive(true);
                    break;

                case QuestWindow.List:
                    SetListWindow();
                    listWindow.gameObject.SetActive(true);
                    break;

                case QuestWindow.Info:
                    SetInfoWindow(sdQuest);
                    infoWindow.gameObject.SetActive(true);
                    break;
            }

            base.Open();
        }

        /// <summary>
        /// 퀘스트 창을 닫는 메서드
        /// </summary>
        /// <param name="force"></param>
        public override void Close(bool force = false)
        {
            base.Close(force);

            // 활성화 된 창을 모두 닫는다
            if (listWindow.gameObject.activeSelf) { listWindow.gameObject.SetActive(!listWindow.gameObject.activeSelf); }
            if (infoWindow.gameObject.activeSelf) { infoWindow.gameObject.SetActive(!infoWindow.gameObject.activeSelf); }
            if (orderWindow.gameObject.activeSelf) { orderWindow.gameObject.SetActive(!orderWindow.gameObject.activeSelf); }

            // 퀘스트 슬롯 정리
            ClearSlots();
        }

        /// <summary>
        /// 오더 창을 세팅하는 메서드
        /// </summary>
        /// <param name="sdQuest"> 오더 창에 띄울 퀘스트 데이터 </param>
        private void SetOrderWindow(SDQuest sdQuest)
        {
            orderTitle.text = sdQuest.name;

            /*
             *  수락 버튼 이벤트 바인딩
             *  1. 수락 버튼을 누를 때 마다 실행 시킬 기능 자체는 똑같은데 전달되는 데이터가 달라져야한다
             *  2. 수락 버튼을 눌렀을 때 수락한 퀘스트를 유저 DB에 저장하는 기능은 같은데 어떤 퀘스트인지 데이터가 달라지기 때문에
             */

            // 수락/거절 버튼에 바인딩된 이벤트를 전부 제거
            acceptBtn.onClick.RemoveAllListeners();
            refuseBtn.onClick.RemoveAllListeners();

            // 오더 창에 띄우려는 퀘스트의 타입에 따라 세팅
            switch (orderQuestType)
            {
                // 미진행 퀘스트
                case OrderQuestType.NoProgress:
                    acceptBtnTitle.text = "수락";

                    // 수락 버튼에 이벤트 바인딩
                    acceptBtn.onClick.AddListener(() =>
                    {
                        // 수락한 퀘스트를 DB에 저장
                        ServerManager.Server.AddQuest(0, sdQuest.index, new ResponsHandler<DtoQuestProgress>(dtoQeustProgress =>
                        {
                            var boQuestProgress = new BoQuestProgress(dtoQeustProgress);

                            // Dto데이터를 Bo에 업데이트
                            GameManager.User.boQuest.progressQuests.Add(boQuestProgress);
                            Close();
                        },
                        failed => { }));
                    });

                    refuseBtn.onClick.AddListener(() =>
                    {
                        Close();
                    });
                    break;

                // 진행중인 퀘스트
                case OrderQuestType.Progress:
                    acceptBtnTitle.text = "확인";

                    acceptBtn.onClick.AddListener(() =>
                    {
                        Close();
                    });

                    refuseBtn.onClick.AddListener(() =>
                    {
                        Close();
                    });
                    break;

                // 완료한 퀘스트
                case OrderQuestType.Clear:
                    acceptBtnTitle.text = "완료";

                    // 완료 버튼에 이벤트 바인딩
                    acceptBtn.onClick.AddListener(() =>
                    {
                        OnClickClearQuest(sdQuest);
                    });
                    break;
            }

            orderDescription.text = 
                GameManager.SD.sdQuestSpeechs.Where(obj => obj.index == sdQuest.description)?.SingleOrDefault().kr;
        }

        /// <summary>
        /// 퀘스트 완료 버튼에 바인딩될 이벤트 메서드
        /// </summary>
        /// <param name="currentQuest"> 방금 클리어한 퀘스트 </param>
        private void OnClickClearQuest(SDQuest currentQuest)
        {
            var boQuest = GameManager.User.boQuest;

            // 진행중인 퀘스트 목록에서 방금 클리어한 진행중인 퀘스트를 찾음
            var progressQuest = boQuest.progressQuests.Where(obj => obj.sdQuest.index == currentQuest.index).SingleOrDefault();

            // 찾은 퀘스트를 목록에서 지움
            boQuest.progressQuests.Remove(progressQuest);

            boQuest.progressQuests.Sort();

            // 클리어한 퀘스트를 완료 퀘스트 목록에 저장
            boQuest.completedQuests.Add(currentQuest);

            var dummyServer = DummyServer.Instance;

            // 진행 퀘스트가 줄었으므로 Dto의 진행 퀘스트 목록 길이는 -1
            var progressArrayLength = dummyServer.userData.dtoQuest.progressQuests.Length - 1;

            // 완료 퀘스트가 늘었으므로 Dto의 완료 퀘스트 목록 길이는 +1
            var completeArrayLength = dummyServer.userData.dtoQuest.completeQuests.Length + 1;

            // 진행/완료 퀘스트 배열의 길이를 재설정
            Array.Resize(ref dummyServer.userData.dtoQuest.progressQuests, progressArrayLength);
            Array.Resize(ref dummyServer.userData.dtoQuest.completeQuests, completeArrayLength);

            // Bo의 진행중인 퀘스트들을 Dto에 다시 저장하는 작업
            for (int i = 0; i < progressArrayLength; i++)
            {
                dummyServer.userData.dtoQuest.progressQuests[i].index = boQuest.progressQuests[i].sdQuest.index;
                dummyServer.userData.dtoQuest.progressQuests[i].details = boQuest.progressQuests[i].details;
            }

            // Bo에 완료한 퀘스트들을 Dto에 다시 저장하는 작업
            for (int i = 0; i < completeArrayLength; i++)
            {
                dummyServer.userData.dtoQuest.completeQuests[i] = boQuest.completedQuests[i].index;
            }

            #region 아이템 보상 작업

            // 퀘스트 보상 목록
            var sdItems = new List<SDItem>();

            // 보상 목록에 퀘스트의 보상을 넣는 작업
            for (int i = 0; i < currentQuest.compensation.Length; i++)
            {
                sdItems.Add(GameManager.SD.sdItems.Where(obj => obj.index == currentQuest.compensation[i])?.SingleOrDefault());
            }

            // 유저가 소지한 아이템 목록
            var userBoItems = GameManager.User.boItems;
            var userInventory = UIWindowManager.Instance.GetWindow<UIInventory>();

            // 보상 목록의 아이템이 소지/비소지 아이템인지 판별하는 작업
            for (int i = 0; i < sdItems.Count(); i++)
            {
                // 보상 목록의 아이템이 장비 아이템인지 확인
                var isEquip = sdItems[i].itemType == Define.ItemType.Equipment;

                // 장비 아이템이 아니라면
                if (!isEquip)
                {
                    // 소지한 아이템인지 확인
                    var sameItem = userBoItems.Where(obj => obj.sdItem.index == sdItems[i].index)?.SingleOrDefault();

                    // 소지한 아이템 이라면
                    if (sameItem != null)
                    {
                        // 아이템 개수 업데이트
                        sameItem.amount += currentQuest.compensationDetail[i];
                        userInventory.IncreaseItem(sameItem);
                    }
                    else
                    {
                        // 새로운 아이템이므로 인벤토리에 추가
                        var boItem = new BoItem(sdItems[i]);
                        boItem.amount = currentQuest.compensationDetail[i];
                        SetItem(boItem);
                    }
                }
                else
                {
                    // 장비 아이템은 무조건 한칸을 차지
                    SetItem(new BoEquipment(sdItems[i]));
                }

                void SetItem(BoItem boItem)
                {
                    userInventory.AddItem(boItem);
                    userBoItems.Add(boItem);
                }

                // DB에 아이템 정보 업데이트
                DummyServer.Instance.userData.dtoItem = new DtoItem(GameManager.User.boItems);
            }

            #endregion

            dummyServer.Save();
            Close();
        }

        /// <summary>
        /// 퀘스트 리스트 창을 세팅하는 메서드
        /// </summary>
        private void SetListWindow()
        {
            var pool = ObjectPoolManager.Instance.GetPool<QuestSlot>(Define.PoolType.QuestSlot);

            switch (currentTab)
            {
                // 진행중인 퀘스트 탭
                case QuestTab.Progress:
                    var boProgressQuests = GameManager.User.boQuest.progressQuests;

                    for (int i = 0; i < boProgressQuests.Count; i++)
                    {
                        SetQuestSlot(boProgressQuests[i].sdQuest, boProgressQuests[i].details);

                        if (!details.ContainsKey(boProgressQuests[i].sdQuest.index))
                        {
                            // 퀘스트 정보 창에서 사용하기 위해
                            // 진행중인 퀘스트의 디테일을 Dictionary에 저장
                            details.Add(boProgressQuests[i].sdQuest.index, boProgressQuests[i].details);
                        }
                    }
                    break;

                // 완료한 퀘스트 탭
                case QuestTab.Completed:
                    var boCompletedQuests = GameManager.User.boQuest.completedQuests;

                    for (int i = 0; i < boCompletedQuests.Count; i++)
                    {
                        SetQuestSlot(boCompletedQuests[i]);
                    }
                    break;
            }

            void SetQuestSlot(SDQuest sdQuest, params int[] questDetails)
            {
                var questSlot = pool.GetPoolableObject(obj => obj.CanRecycle);
                questSlot.Initialize(sdQuest, currentTab);
                questSlot.transform.SetParent(ContentsHolder);
                questSlot.transform.localScale = Vector3.one;
                questSlot.gameObject.SetActive(true);

                slots.Add(questSlot);
            }
        }

        /// <summary>
        /// 퀘스트 정보 창을 세팅하는 메서드
        /// </summary>
        /// <param name="progressQuest"> 현재 진행중인 퀘스트의 기획데이터 </param>
        private void SetInfoWindow(SDQuest progressQuest)
        {
            var sd = GameManager.SD;

            // 리스트 창에 있는 슬롯 반환
            ClearSlots();

            infoTitle.text = progressQuest.name;
            infoDescription.text =
                sd.sdQuestSpeechs.Where(obj => obj.index == progressQuest.description)?.SingleOrDefault().kr;

            questDetail.text = string.Empty;

            // 퀘스트 타입에 따라 디테일 텍스트 세팅
            switch (progressQuest.questType)
            {
                case Define.QuestType.Collection:
                case Define.QuestType.Hunt:
                    // 디테일 텍스트를 세팅하는 작업
                    for (int i = 0; i < progressQuest.target.Length; i++)
                    {
                        var targetName =
                            sd.sdMonsters.Where(obj => obj.index == progressQuest.target[i])?.SingleOrDefault().name;
                        questDetail.text += 
                            targetName + " : " + details[progressQuest.index][i] + " / " + progressQuest.questDetail[i] + '\n';
                    }
                    break;

                case Define.QuestType.Conversation:
                    for (int i = 0; i < progressQuest.target.Length; i++)
                    {
                        var targetName = sd.sdNPCs.Where(obj => obj.index == progressQuest.target[i])?.SingleOrDefault().name;
                        questDetail.text += targetName + '\n';
                    }
                    break;
            }

            // 뒤로가기 버튼에 이벤트 바인딩
            BackBtn.onClick.AddListener(() =>
            {
                Close();
                Open(QuestWindow.List);
            });
        }

        /// <summary>
        /// 진행/완료 퀘스트 탭에 바인딩할 메서드
        /// </summary>
        /// <param name="tab"> 현재 플레이어가 요청한 탭 </param>
        private void OnClickTab(QuestTab tab)
        {
            var isOtherTab = currentTab != tab;

            currentTab = tab;

            if (isOtherTab)
            {
                ClearSlots();
                SetListWindow();
            }
        }

        /// <summary>
        /// 퀘스트 창의 슬롯을 초기화 하는 메서드
        /// </summary>
        private void ClearSlots()
        {
            if (slots.Count == 0)
            {
                return;
            }

            var pool = ObjectPoolManager.Instance.GetPool<QuestSlot>(Define.PoolType.QuestSlot);

            for (int i = 0; i < slots.Count; i++)
            {
                pool.ReturnPoolableObject(slots[i]);
            }

            slots.Clear();
        }
    }
}