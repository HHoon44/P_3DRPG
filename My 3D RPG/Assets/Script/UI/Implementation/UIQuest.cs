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
    public class UIQuest : UIWindow
    {
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

        [Header("Quest Content Window Filed")]
        public Button BackBtn;                      // -> 컨텐츠 창에 존재하는 뒤로 가는 버튼
        public TextMeshProUGUI contentTitle;        // -> 진행중인 퀘스트의 타이틀
        public TextMeshProUGUI contentDescription;  // -> 진행중인 퀘스트이 내용
        public TextMeshProUGUI questDetail;         // -> 진행중인 퀘스트의 디테일 내용
        public Transform contentWindow;             // -> 퀘스트 컨텐츠 창

        private List<QuestSlot> slots = new List<QuestSlot>();                  // -> 퀘스트 슬롯이 저장될 공간
        private Dictionary<int, int[]> details = new Dictionary<int, int[]>();  // -> 현재 슬롯에 저장된 퀘스트 디테일이 저장될 공간

        public override void Start()
        {
            base.Start();

            acceptBtnTitle = acceptBtn.GetComponentInChildren<TextMeshProUGUI>();

            // -> 거절 버튼, 2개의 탭 버튼 클릭 시 이벤트 바인딩
            progressTabBtn.onClick.AddListener(() => { OnClickTab(QuestTab.Progress); });
            completedTabBtn.onClick.AddListener(() => { OnClickTab(QuestTab.Completed); });
        }

        private void Update()
        {
            // -> 퀘스트 창 키 입력 시 퀘스트 리스트 오픈
            if (Input.GetButtonDown(Define.Input.Quest))
            {
                // -> 만약 이미 창이 켜져있는데 버튼을 누른거라면 창을 꺼준다
                if (isOpen)
                {
                    Close(true);
                    return;
                }

                Open(QuestWindow.List);
            }

        }

        /// <summary>
        /// => QuestList창을 열지 QuestOrder창을 열지 정하는 메서드
        /// </summary>
        /// <param name="questWindow"> 현재 열고자 하는 창 </param>
        /// <param name="sdQuest"> 현재 퀘스트 기획 데이터 </param>
        public void Open(QuestWindow questWindow, SDQuest sdQuest = null)
        {
            // -> 이미 다른 창이 열린 상태에서 컨텐츠 창이 아닌 다른 창을 연다면
            // -> 컨텐츠 창은 리스트 창이 열린 상태에서도 열릴 수 있기 때문에
            if (isOpen && questWindow != QuestWindow.Content)
            {
                // -> 그치만 컨텐츠 창에서 리스트 창으로 돌아가는 경우도 있으므로
                if (currentWindow != QuestWindow.Content)
                {
                    return;
                }
            }

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
                case QuestWindow.Content:
                    SetContentWindow(sdQuest);
                    contentWindow.gameObject.SetActive(true);
                    break;
            }

            base.Open();
        }

        /// <summary>
        /// => UIQuest창을 꺼버리는 메서드
        /// </summary>
        /// <param name="force"></param>
        public override void Close(bool force = false)
        {
            base.Close(force);

            // -> 여기 쫌 수정해야할듯
            if (listWindow.gameObject.activeSelf) { listWindow.gameObject.SetActive(!listWindow.gameObject.activeSelf); }
            if (contentWindow.gameObject.activeSelf) { contentWindow.gameObject.SetActive(!contentWindow.gameObject.activeSelf); }
            if (orderWindow.gameObject.activeSelf) { orderWindow.gameObject.SetActive(!orderWindow.gameObject.activeSelf); }

            // -> 퀘스트 창을 닫으면서 창에 있던 QuestSlot들을 다 정리한다
            ClearSlots();
        }

        /// <summary>
        /// => 퀘스트 명령 창을 세팅하는 메서드
        /// </summary>
        /// <param name="sdQuest"> 명령 창에 띄울 퀘스트 데이터 </param>
        private void SetOrderWindow(SDQuest sdQuest)
        {
            orderTitle.text = sdQuest.name;

            // -> 수락 버튼 이벤트 바인딩
            /// -> 수락 버튼을 누를 때 마다 실행 시킬 기능 자체는 똑같은데, 전달되는 데이터가 달라져야한다
            /// -> 수락 버튼을 눌렀을 때 수락한 퀘스트를 유저 DB에 저장하는 기능은 같은데 어떤 퀘스트인지 데이터가 달라지기 때문에

            // -> 버튼에 바인딩된 이벤트 전부 제거
            acceptBtn.onClick.RemoveAllListeners();

            // -> 현재 오더창에 세팅하는 퀘스트가 '진행중인 퀘스트' 인지 '아직 진행중인 퀘스트가 이닌지' 에 따라서 나눠놓음
            switch (orderTab)
            {
                // -> 미진행
                case QuestOrderTab.None:
                    // -> 수락 버튼 클릭시 이벤트 바인딩
                    acceptBtn.onClick.AddListener(() =>
                    {
                        // -> 수락한 퀘스트를 DtoQuest에 저장하는 작업
                        ServerManager.Server.AddQuest(0, sdQuest.index, new ResponsHandler<DtoQuestProgress>(dtoQeustProgress =>
                        {
                            // -> 핸들러를 통해서 받은 DtoQuestProgress를 BoQuestProgress에 담아둔다음 User에 저장한다
                            var boQuestProgress = new BoQuestProgress(dtoQeustProgress);
                            GameManager.User.boQuest.progressQuests.Add(boQuestProgress);

                            // -> 위의 작업이 끝나면 오더창을 끈다
                            orderWindow.gameObject.SetActive(!orderWindow.gameObject.activeSelf);
                            Close();
                        },
                        failed =>
                        {
                        }
                        ));
                    });

                    refuseBtn.onClick.RemoveAllListeners();
                    refuseBtn.onClick.AddListener(() =>
                    {
                        Close();
                    });

                    orderDescription.text = GameManager.SD.sdQuestSpeechs.
                        Where(obj => obj.index == sdQuest.description)?.SingleOrDefault().kr;
                    break;

                // -> 진행중
                case QuestOrderTab.Progress:
                    acceptBtn.gameObject.SetActive(false);
                    refuseBtn.onClick.AddListener(() =>
                    {
                        Close();
                    });  // 나중에 거절 하면 진행중인 퀘스트 목록에서 삭제
                    orderDescription.text = GameManager.SD.sdQuestSpeechs.
                        Where(obj => obj.index == sdQuest.description)?.SingleOrDefault().kr;
                    break;

                // -> 퀘스트 완료
                case QuestOrderTab.Clear:
                    acceptBtnTitle.text = "완료";
                    acceptBtn.onClick.AddListener(() =>
                    {
                        OnClickClearQuest(sdQuest);
                    });

                    // -> 퀘스트 완료시 텍스트 뜨는거
                    orderDescription.text = "아리가또~";
                    break;
            }
        }

        /// <summary>
        /// => 퀘스트 리스트 창을 세팅하는 메서드
        /// </summary>
        private void SetListWindow()
        {
            // -> 퀘스트 리스트창에 띄울 슬롯들의 풀을 가져온다
            var pool = ObjectPoolManager.Instance.GetPool<QuestSlot>(Define.PoolType.QuestSlot);

            // -> 현재 탭 별로 퀘스트 슬롯 설정
            switch (currentTab)
            {
                case QuestTab.Progress:
                    // -> 진행 탭을 누르면 Bo에 저장되어있는 BoQuestProgress에 대한 정보를 띄운다
                    var boProgressQuests = GameManager.User.boQuest.progressQuests;

                    for (int i = 0; i < boProgressQuests.Count; i++)
                    {
                        SetQuestSlot(boProgressQuests[i].sdQuest, boProgressQuests[i].details);
                    }
                    break;

                case QuestTab.Completed:
                    // -> 완료 탭을 누르면 Bo에 저장되어있는 BoQuestCompleted에 대한 정보를 띄운다
                    var boCompletedQuests = GameManager.User.boQuest.completedQuests;

                    for (int i = 0; i < boCompletedQuests.Count; i++)
                    {
                        SetQuestSlot(boCompletedQuests[i]);
                    }
                    break;
            }

            // -> 중복되는 작업은 로컬함수
            void SetQuestSlot(SDQuest sdQuest, params int[] questDetails)
            {
                // -> 현재 퀘스트 디테일을 필드에 저장

                if (!details.ContainsKey(sdQuest.index))
                {
                    details.Add(sdQuest.index, questDetails);
                }

                var questSlot = pool.GetPoolableObject();
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
        private void SetContentWindow(SDQuest progressQuest)
        {
            // -> 컨텐츠 창을 키면서 리스트 창에 있던 슬롯들을 풀에 반환한다
            ClearSlots();

            // -> 받은 데이터로 컨텐츠 창 세팅
            contentTitle.text = progressQuest.name;
            contentDescription.text =
                GameManager.SD.sdQuestSpeechs.Where(obj => obj.index == progressQuest.description)?.SingleOrDefault().kr;

            var sdQuest = GameManager.SD.sdQuests.Where(obj => obj.index == progressQuest.index)?.SingleOrDefault();

            questDetail.text = string.Empty;

            for (int i = 0; i < progressQuest.target.Length; i++)
            {
                var monsterName = GameManager.SD.sdMonsters.Where(obj => obj.index == progressQuest.target[i])?.SingleOrDefault().name;

                questDetail.text += monsterName + " : " + details[progressQuest.index][i] + " / " + sdQuest.questDetail[i] + '\n';
            }

            // -> 컨텐츠 창에 존재하는 뒤로가기 버튼에 이벤트 바인딩
            BackBtn.onClick.AddListener(() =>
            {
                contentWindow.gameObject.SetActive(false);
                Open(QuestWindow.List);
            });
        }

        /// <summary>
        /// => 퀘스트를 완료하고나서 완료 버튼 메서드
        /// </summary>
        /// <param name="currentQuest"> 현재 클리어한 퀘스트 기획 데이터 </param>
        private void OnClickClearQuest(SDQuest currentQuest)
        {
            // -> 이제 클리어한 퀘스트를 진행중인 퀘스트 목록에서 찾는 작업
            var boQuest = GameManager.User.boQuest;

            // -> 퀘스트가 있는 곳의 인덱스
            var progressIndex = 0;

            // -> 진행중인 퀘스트 목록에서 방금 클리어한 퀘스트를 찾는 작업
            for (int i = 0; i < boQuest.progressQuests.Count; i++)
            {
                // -> 차자따 뽐인~
                if (boQuest.progressQuests[i].sdQuest.index == currentQuest.index)
                {
                    progressIndex = i;
                }
            }

            // -> 인덱스를 찾았으니 진행중인 퀘스트에서 삭제
            // -> 그리고 정렬
            // -> 그리고 완료한 퀘스트 목록에 추가
            boQuest.progressQuests.RemoveAt(progressIndex);
            boQuest.progressQuests.Sort();
            boQuest.completedQuests.Add(currentQuest);

            // -> Bo데이터 변했으므로 Dto데이터에 다시 저장하는 작업
            var dummyServer = DummyServer.Instance;
            var dtoProgressQuest = new DtoQuestProgress();

            // -> 진행중인 퀘스트는 제거해야 하므로 -1
            // -> 완료한 퀘스트는 추가해야 하므로 +1
            var progressLength = dummyServer.userData.dtoQuest.progressQuests.Length - 1;
            var completeLength = dummyServer.userData.dtoQuest.completeQuests.Length + 1;

            // -> 진행중인 퀘스트의 배열 길이와 완료한 퀘스트의 배열 길이를 재정의한다
            Array.Resize(ref dummyServer.userData.dtoQuest.progressQuests, progressLength);
            Array.Resize(ref dummyServer.userData.dtoQuest.completeQuests, completeLength);

            // -> 빈곳을 채우는 작업
            for (int i = 0; i < progressLength; i++)
            {
                dtoProgressQuest.index = boQuest.progressQuests[i].sdQuest.index;
                dtoProgressQuest.details = boQuest.progressQuests[i].details;

                dummyServer.userData.dtoQuest.progressQuests[i] = dtoProgressQuest;
            }

            // -> Dto에 완료한 퀘스트를 저장해놓는 작업
            for (int i = 0; i < completeLength; i++)
            {
                dummyServer.userData.dtoQuest.completeQuests[i] = currentQuest.index;
            }

            #region 아이템 보상 작업

            // -> 보상 리스트
            var sdItems = new List<SDItem>();

            // -> 현재 퀘스트의 보상과 같은 아이템 기획 데이터를 찾는 작업
            for (int i = 0; i < currentQuest.compensation.Length; i++)
            {
                sdItems.Add(GameManager.SD.sdItems.Where(obj => obj.index == currentQuest.compensation[i])?.SingleOrDefault());
            }

            // -> 이미 존재한 아이템인지 비교 하거나 새로운 아이템이라면 추가하기 위해서
            //    게임매니저가 가지고 있는 BoItem 데이터를 가져온다
            var userBoItems = GameManager.User.boItems;

            // -> 아이템 슬롯에 접근하기 위해서 인벤토리를 가져온다
            var userInventory = UIWindowManager.Instance.GetWindow<UIInventory>();

            // -> 자 드가자~
            for (int i = 0; i < sdItems.Count(); i++)
            {
                // -> 아이템이 장비 아이템인지 판단
                var isEquip = sdItems[i].itemType == Define.ItemType.Equipment;

                // -> 장비 아이템이 아니라면
                if (!isEquip)
                {
                    // -> 이미 가지고 있는 아이템인지 판단
                    var sameItem = userBoItems.Where(obj => obj.sdItem.index == sdItems[i].index)?.SingleOrDefault();

                    // -> 이미 가지고 있는 아이템이 라면
                    if (sameItem != null)
                    {
                        // -> 개수값을 증가시켜주고 슬롯이 지닌 개수 텍스트 업데이트
                        sameItem.amount += currentQuest.compensationDetail[i];
                        userInventory.IncreaseItem(sameItem);
                    }
                    // -> 이미 가지고 있는 아이템이 아니라면
                    else
                    {
                        // -> 새롭게 추가한다
                        SetItem(new BoItem(sdItems[i]));
                    }
                }
                else
                {
                    // -> 장비 아이템은 무조건 한칸을 차지하므로 새롭게 추가한다
                    SetItem(new BoEquipment(sdItems[i]));
                }

                void SetItem(BoItem boItem)
                {
                    userInventory.AddItem(boItem);
                    userBoItems.Add(boItem);
                }

                // -> BoItem의 데이터에 변동이 생겼으므로 다시 DtoItem에 넣어주는 작업을 한다(나중에 변동된 데이터만 들어가게 설정하는 작업 해보도록)
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
            // -> 저장된 슬롯이 없다면 
            if (slots.Count == 0)
            {
                return;
            }

            // -> 풀에 반환하기 위해서 QuestSlot풀을 가져온다
            var pool = ObjectPoolManager.Instance.GetPool<QuestSlot>(Define.PoolType.QuestSlot);

            for (int i = 0; i < slots.Count; i++)
            {
                pool.ReturnPoolableObject(slots[i]);
            }

            slots.Clear();
        }
    }
}