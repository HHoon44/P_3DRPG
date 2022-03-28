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
        public QuestWindow currentWindow;           // -> 현재 윈도우 창
        public QuestTab currentTab;                 // -> 현재 탭 창

        [Header("Quest List Window Filed")]
        public Button progressTabBtn;               // -> 진행 탭 버튼
        public Button completedTabBtn;              // -> 완료 탭 버튼
        public Transform ContentsHolder;            // -> 스크롤뷰에 존재하는 컨텐츠 홀더
        public Transform listWindow;                // -> 퀘스트 리스트 창


        [Header("Quest Order Window Filed")]
        public Button acceptBtn;                    // -> 수락 버튼
        public Button refuseBtn;                    // -> 거절 버튼
        public TextMeshProUGUI orderTitle;          // -> 퀘스트 타이틀
        public TextMeshProUGUI orderDescription;    // -> 퀘스트 내용
        public TextMeshProUGUI acceptBtnTitle;      // -> 수락 버튼이 지닌 텍스트 컴포넌트
        public Transform orderWindow;               // -> 퀘스트 오더 창
        public QuestOrderTab orderTab;              // -> 오더창에 올릴 퀘스트가 진행중인 퀘스트 또는 아직 미진행 퀘스트


        [Header("Quest Info Window Filed")]
        public Button BackBtn;                      // -> 정보 창에 존재하는 뒤로 가는 버튼
        public TextMeshProUGUI infoTitle;           // -> 진행중인 퀘스트의 타이틀
        public TextMeshProUGUI infoDescription;     // -> 진행중인 퀘스트이 내용
        public TextMeshProUGUI questDetail;         // -> 진행중인 퀘스트의 디테일 내용
        public Transform infoWindow;                // -> 퀘스트 정보 창

        /// <summary>
        /// => 퀘스트 창에 존재하는 퀘스트 슬롯들을 담아놓을 리스트
        /// </summary>
        private List<QuestSlot> slots = new List<QuestSlot>();

        /// <summary>
        /// => 진행중인 퀘스트의 인덱스와 디테일 값을 담아놓을 딕셔너리
        /// </summary>
        private Dictionary<int, int[]> details = new Dictionary<int, int[]>();

        public override void Start()
        {
            base.Start();

            acceptBtnTitle = acceptBtn.GetComponentInChildren<TextMeshProUGUI>();

            // -> 탭 버튼에 이벤트를 바인딩 합니다!
            progressTabBtn.onClick.AddListener(() => { OnClickTab(QuestTab.Progress); });
            completedTabBtn.onClick.AddListener(() => { OnClickTab(QuestTab.Completed); });
        }

        private void Update()
        {
            if (Input.GetButtonDown(Define.Input.Quest))
            {
                // -> 이미 창이 켜져있다면!
                if (isOpen)
                {
                    // -> 창을 끕니다!
                    Close(true);
                    return;
                }

                // -> 리스트 창을 켜줍니다!
                Open(QuestWindow.List);
            }
        }

        /// <summary>
        /// => QuestWindow에 따라 퀘스트 창을 키는 메서드
        /// </summary>
        /// <param name="questWindow"> 현재 열고자 하는 창 </param>
        /// <param name="sdQuest"> 현재 퀘스트 기획 데이터 </param>
        public void Open(QuestWindow questWindow, SDQuest sdQuest = null)
        {
            // -> 다른 퀘스트 창이 열린 상태에서 정보 창이 아닌 다른 창을 연다면!
            if (isOpen && questWindow != QuestWindow.Info)
            {
                // -> 현재 창이 정보 창이 아니라면!
                if (currentWindow != QuestWindow.Info)
                {
                    return;
                }
            }

            // -> 현재 활성화 할려는 윈도우 창을 담아놓습니다!
            currentWindow = questWindow;

            // -> 창 타입에 따라 창 활성화
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
        /// => 퀘스트 창을 닫는 메서드
        /// </summary>
        /// <param name="force"></param>
        public override void Close(bool force = false)
        {
            base.Close(force);

            // -> 퀘스트 창을 닫는다면 켜져있는 창들은 모두 닫습니다!
            if (listWindow.gameObject.activeSelf) { listWindow.gameObject.SetActive(!listWindow.gameObject.activeSelf); }
            if (infoWindow.gameObject.activeSelf) { infoWindow.gameObject.SetActive(!infoWindow.gameObject.activeSelf); }
            if (orderWindow.gameObject.activeSelf) { orderWindow.gameObject.SetActive(!orderWindow.gameObject.activeSelf); }

            // -> 창을 닫으면서 창에 존재하는 QuestSlot들을 정리 합니다!
            ClearSlots();
        }

        /// <summary>
        /// => 퀘스트 명령 창을 세팅하는 메서드
        /// </summary>
        /// <param name="sdQuest"> 명령 창에 띄울 퀘스트 데이터 </param>
        private void SetOrderWindow(SDQuest sdQuest)
        {
            orderTitle.text = sdQuest.name;

            /*
             *  수락 버튼 이벤트 바인딩
             *  1. 수락 버튼을 누를 때 마다 실행 시킬 기능 자체는 똑같은데 전달되는 데이터가 달라져야한다
             *  2. 수락 버튼을 눌렀을 때 수락한 퀘스트를 유저 DB에 저장하는 기능은 같은데 어떤 퀘스트인지 데이터가 달라지기 때문에
             */

            // -> 버튼에 바인딩된 이벤트를 전부 제거 합니다!
            acceptBtn.onClick.RemoveAllListeners();
            refuseBtn.onClick.RemoveAllListeners();

            // -> 현재 퀘스트의 진행도 상태에 따라 오더창을 나눕니다!
            switch (orderTab)
            {
                // -> 미진행
                case QuestOrderTab.NoProgress:
                    acceptBtnTitle.text = "수락";

                    // -> 수락 버튼에 이벤트를 바인딩합니다!
                    acceptBtn.onClick.AddListener(() =>
                    {
                        // -> 수락한 퀘스트를 DB에 저장합니다!
                        ServerManager.Server.AddQuest(0, sdQuest.index, new ResponsHandler<DtoQuestProgress>(dtoQeustProgress =>
                        {
                            // -> 핸들러를 통해서 받은 Dto데이터를를 Bo데이터로 변환 후 GM에 저장합니다!
                            var boQuestProgress = new BoQuestProgress(dtoQeustProgress);
                            GameManager.User.boQuest.progressQuests.Add(boQuestProgress);

                            // -> 오더 창과 현재 UI를 닫습니다!
                            orderWindow.gameObject.SetActive(!orderWindow.gameObject.activeSelf);
                            Close();
                        },
                        failed => { }));
                    });

                    refuseBtn.onClick.AddListener(() =>
                    {
                        Close();
                    });
                    break;

                // -> 진행중
                case QuestOrderTab.Progress:
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

                // -> 클리어
                case QuestOrderTab.Clear:
                    acceptBtnTitle.text = "완료";

                    acceptBtn.onClick.AddListener(() =>
                    {
                        OnClickClearQuest(sdQuest);
                    });
                    break;
            }

            orderDescription.text = GameManager.SD.sdQuestSpeechs.Where(obj => obj.index == sdQuest.description)?.SingleOrDefault().kr;
        }

        /// <summary>
        /// => 퀘스트 리스트 창을 세팅하는 메서드
        /// </summary>
        private void SetListWindow()
        {
            var pool = ObjectPoolManager.Instance.GetPool<QuestSlot>(Define.PoolType.QuestSlot);

            // -> 현재 탭 별로 퀘스트 슬롯 설정
            switch (currentTab)
            {
                case QuestTab.Progress:
                    // -> 진행중인 퀘스트 목록을 가져옵니다!
                    var boProgressQuests = GameManager.User.boQuest.progressQuests;

                    for (int i = 0; i < boProgressQuests.Count; i++)
                    {
                        SetQuestSlot(boProgressQuests[i].sdQuest, boProgressQuests[i].details);

                        if (!details.ContainsKey(boProgressQuests[i].sdQuest.index))
                        {
                            // -> 진행중인 퀘스트의 인덱스와 디테일을
                            //    퀘스트 정보 창에서 사용하기 위해 담아둡니다!
                            details.Add(boProgressQuests[i].sdQuest.index, boProgressQuests[i].details);
                        }
                    }
                    break;

                case QuestTab.Completed:
                    // -> 완료한 퀘스트 목록을 가져옵니다!
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
        /// => 컨텐츠 창 활성화 메서드
        /// </summary>
        /// <param name="progressQuest"> 현재 진행중인 퀘스트의 기획데이터 </param>
        private void SetInfoWindow(SDQuest progressQuest)
        {
            // -> 컨텐츠 창을 활성화 화면서 리스트 창에 있던 슬롯들은 풀에 반환합니다!
            ClearSlots();

            // -> 받은 데이터로 컨텐츠 창을 세팅합니다!
            infoTitle.text = progressQuest.name;
            infoDescription.text =
                GameManager.SD.sdQuestSpeechs.Where(obj => obj.index == progressQuest.description)?.SingleOrDefault().kr;

            questDetail.text = string.Empty;

            // -> 진행중인 퀘스트의 퀘스트 타입에 따라 디테일 텍스트를 세팅합니다!
            switch (progressQuest.questType)
            {
                case Define.QuestType.Collection:
                case Define.QuestType.Hunt:
                    // -> 퀘스트의 디테일 정보를 세팅하는 작업입니다!
                    for (int i = 0; i < progressQuest.target.Length; i++)
                    {
                        var targetName = GameManager.SD.sdMonsters.Where(obj => obj.index == progressQuest.target[i])?.SingleOrDefault().name;
                        questDetail.text += targetName + " : " + details[progressQuest.index][i] + " / " + progressQuest.questDetail[i] + '\n';
                    }
                    break;

                case Define.QuestType.Conversation:
                    for (int i = 0; i < progressQuest.target.Length; i++)
                    {
                        var targetName = GameManager.SD.sdNPCs.Where(obj => obj.index == progressQuest.target[i])?.SingleOrDefault().name;
                        questDetail.text += targetName + '\n';
                    }
                    break;
            }


            // -> 퀘스트 정보창에 존재하는 뒤로가기 버튼에 이벤트 바인딩
            BackBtn.onClick.AddListener(() =>
            {
                infoWindow.gameObject.SetActive(false);

                // -> 뒤로가기 버튼을 누르면 다시 리스트 창으로 돌아갑니다!
                Open(QuestWindow.List);
            });
        }

        /// <summary>
        /// => 퀘스트를 완료 후 오더 창에 존재하는 완료 버튼에 바인딩할 메서드
        /// </summary>
        /// <param name="currentQuest"> 방금 클리어한 퀘스트의 데이터 </param>
        private void OnClickClearQuest(SDQuest currentQuest)
        {
            var boQuest = GameManager.User.boQuest;

            // -> 진행중인 퀘스트가 위치한 곳의 인덱스 값 입니다!
            var progressIndex = 0;

            // -> 진행 퀘스트 목록에서 완료한 퀘스트를 찾는 작업 입니다!
            for (int i = 0; i < boQuest.progressQuests.Count; i++)
            {
                if (boQuest.progressQuests[i].sdQuest.index == currentQuest.index)
                {
                    // -> 진행 퀘스트 목록에 저장 되어있는 클리어한 퀘스트 위치 값 입니다!
                    progressIndex = i;
                    break;
                }
            }

            // -> 인덱스 값을 이용하여 진행 퀘스트 목록에서 제거 합니다!
            boQuest.progressQuests.RemoveAt(progressIndex);

            // -> 진행 퀘스트 목록에서 빵꾸가 났으니 정렬을 한번 합니다!
            boQuest.progressQuests.Sort();

            // -> 퀘스트를 완료 했으므로 완료 퀘스트 목록에 저장 합니다!
            boQuest.completedQuests.Add(currentQuest);

            var dummyServer = DummyServer.Instance;

            // -> 진행 퀘스트가 하나 줄었으므로 Dto의 진행 퀘스트 목록 길이는 1을 뺍니다!
            // -> 완료 퀘스트가 하나 늘었으므로 Dto의 완료 퀘스트 목록 길이는 1을 더합니다!
            var progressArrayLength = dummyServer.userData.dtoQuest.progressQuests.Length - 1;
            var completeArrayLength = dummyServer.userData.dtoQuest.completeQuests.Length + 1;

            // -> 진행, 완료 퀘스트 배열의 길이를 재설정 합니다!
            Array.Resize(ref dummyServer.userData.dtoQuest.progressQuests, progressArrayLength);
            Array.Resize(ref dummyServer.userData.dtoQuest.completeQuests, completeArrayLength);

            // -> Bo의 진행중인 퀘스트 목록의 데이터를 Dto에 저장합니다!
            for (int i = 0; i < progressArrayLength; i++)
            {
                dummyServer.userData.dtoQuest.progressQuests[i].index = boQuest.progressQuests[i].sdQuest.index;
                dummyServer.userData.dtoQuest.progressQuests[i].details = boQuest.progressQuests[i].details;
            }

            for (int i = 0; i < completeArrayLength; i++)
            {
                dummyServer.userData.dtoQuest.completeQuests[i] = boQuest.completedQuests[i].index;
            }

            #region 아이템 보상 작업

            // -> 보상 리스트
            var sdItems = new List<SDItem>();

            // -> 현재 퀘스트의 보상과 같은 아이템 기획 데이터를 찾는 작업
            // -> 
            for (int i = 0; i < currentQuest.compensation.Length; i++)
            {
                sdItems.Add(GameManager.SD.sdItems.Where(obj => obj.index == currentQuest.compensation[i])?.SingleOrDefault());
            }

            var userBoItems = GameManager.User.boItems;
            var userInventory = UIWindowManager.Instance.GetWindow<UIInventory>();

            // -> 소지한 아이템인지 소지하지 않은 아이템인지
            //    장비 아이템인지 소비 아이템인지 판별하는 작업입니다!
            for (int i = 0; i < sdItems.Count(); i++)
            {
                // -> 보상 목록의 아이템이 장비 아이템인지 확인합니다!
                var isEquip = sdItems[i].itemType == Define.ItemType.Equipment;

                // -> 장비 아이템이 아니라면!
                if (!isEquip)
                {
                    // -> 이미 가지고 있는 아이템인지 확인합니다!
                    var sameItem = userBoItems.Where(obj => obj.sdItem.index == sdItems[i].index)?.SingleOrDefault();

                    // -> 이미 가지고 있는 아이템이 라면!
                    if (sameItem != null)
                    {
                        // -> 아이템 개수를 업데이트 해줍니다!
                        sameItem.amount += currentQuest.compensationDetail[i];
                        userInventory.IncreaseItem(sameItem);
                    }
                    // -> 처음 얻은 아이템이라면!
                    else
                    {
                        // -> 새롭게 추가해줍니다!

                        var boItem = new BoItem(sdItems[i]);
                        boItem.amount = currentQuest.compensationDetail[i];
                        SetItem(boItem);
                    }
                }
                else
                {
                    // -> 장비 아이템은 무조건 한칸을 차지하므로 새롭게 추가합니다!
                    SetItem(new BoEquipment(sdItems[i]));
                }

                void SetItem(BoItem boItem)
                {
                    userInventory.AddItem(boItem);
                    userBoItems.Add(boItem);
                }

                // -> DB에 새로운 아이템 정보를 업데이트 합니다!
                DummyServer.Instance.userData.dtoItem = new DtoItem(GameManager.User.boItems);
            }

            #endregion

            dummyServer.Save();
            Close();
        }

        /// <summary>
        /// => 진행 퀘스트 탭과 완료 퀘스트 탭에 넣을 메서드
        /// </summary>
        /// <param name="tab"> 현재 플레이어가 열려는 탭 이름 </param>
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
        /// => 현재 풀이 저장되어있는 리스트를 비우는 메서드
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