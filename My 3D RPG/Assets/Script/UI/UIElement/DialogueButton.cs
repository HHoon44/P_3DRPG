using ProjectChan.DB;
using ProjectChan.Resource;
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
    /// => 다이얼로그 버튼을 세팅하는 클래스
    /// </summary>
    public class DialogueButton : MonoBehaviour, IPoolableObject
    {
        public Image icon;                  // -> 다이얼로그 버튼의 아이콘
        public TextMeshProUGUI title;       // -> 다이얼로그 버튼의 텍스트
        public Button btn;                  // -> 다이얼로그 버튼의 버튼 기능

        /// <summary>
        /// => 재사용 가능한지
        /// </summary>
        public bool CanRecycle { get; set; } = true;

        /// <summary>
        /// => 다이얼로그 버튼을 세팅하는 메서드
        /// </summary>
        /// <param name="questIndex"> NPC가 지닌 퀘스트 인덱스 </param>
        public void Initialize(int questIndex)
        {
            icon.sprite = SpriteLoader.GetSprite(Define.Resource.AtlasType.UIAtlase, "exclamation");

            // -> 플레이어의 진행중인 퀘스트 목록을 가져옵니다!
            var progressQuests = GameManager.User.boQuest.progressQuests;
            var progressQuestIndex = -1;

            // -> 버튼이 지닌 퀘스트 데이터가
            //    플레이어가 진행중인 퀘스트 데이터 인지 확인하는 작업입니다!
            for (int i = 0; i < progressQuests.Count; i++)
            {
                // -> 진행중인 퀘스트의 인덱스와 버튼의 퀘스트 인덱스가 같다면!
                if (progressQuests[i].sdQuest.index == questIndex)
                {
                    // -> i번째에 버튼이 지닐 퀘스트가 있습니다!
                    progressQuestIndex = i;
                    break;
                }
            }
            /// <summary>
            /// => NPC가 지닌 퀘스트 다이얼로그 버튼에 바인딩될 메서드
            /// </summary>
            /// <param name="sdQuest"> 다이얼로그 버튼이 지닌 기획 데이터 </param>

            // -> 버튼이 지닐 퀘스트 데이터 입니다!
            var sdQuest = GameManager.SD.sdQuests.Where(obj => obj.index == questIndex)?.SingleOrDefault();

            title.text = sdQuest.name;
            btn.onClick.AddListener(() => { OnClickQuest(sdQuest, progressQuestIndex); });
        }

        /// <summary>
        /// => NPC가 지닌 다이얼로그 버튼에 바인딩될 메서드
        /// </summary>
        /// <param name="sdQuest"> 버튼이 지닐 퀘스트 기획 데이터 </param>
        /// <param name="progressQuestIndex"> 진행중인 퀘스트 목록중 버튼이 지닐 퀘스트가 있는 위치 값 </param>
        private void OnClickQuest(SDQuest sdQuest, int progressQuestIndex)
        {
            // -> 다이얼로그 창을 닫습니다!
            var uiWindowManager = UIWindowManager.Instance;
            uiWindowManager.GetWindow<UIDialogue>().Close();

            // -> 버튼이 지닐 퀘스트가 진행중인 퀘스트가 아니라면!
            if (progressQuestIndex == -1)
            {
                uiWindowManager.GetWindow<UIQuest>().orderTab = QuestOrderTab.NoProgress;
            }
            else
            {
                // -> 진행중인 퀘스트 입니다!
                uiWindowManager.GetWindow<UIQuest>().orderTab = QuestOrderTab.Progress;

                // -> 버튼이 지닌 퀘스트가 있는 진행중인 퀘스트를 가져옵니다!
                // -> sdQuest == progressQuest
                var progressQuest = GameManager.User.boQuest.progressQuests[progressQuestIndex];
                var detailsLength = sdQuest.questDetail.Length;

                // -> 진행중인 퀘스트의 완료한 디테일 개수 입니다!
                var progressDetail = 0;

                // -> 진행중인 퀘스트가 이제 막 완료한 퀘스트 인지 확인하는 작업입니다!
                for (int i = 0; i < detailsLength; i++)
                {
                    // -> 기획 데이터 안의 디테일 값과 진행중인 퀘스트의 디테일 값이 같다면!
                    if (sdQuest.questDetail[i] == progressQuest.details[i])
                    {
                        progressDetail++;
                    }
                }

                if (detailsLength == progressDetail)
                {
                    // -> 이제 막 클리어한 퀘스트 입니다!
                    uiWindowManager.GetWindow<UIQuest>().orderTab = QuestOrderTab.Clear;
                }
            }

            // -> 플레이어는 행동을 멈춥니다!
            uiWindowManager.GetWindow<UIDialogue>().boNPC.actor.isPlayerAction = false;
            uiWindowManager.GetWindow<UIQuest>().Open(QuestWindow.Order, sdQuest);
        }

        /// <summary>
        /// => 기능 NPC가 지닐 버튼에 바인딩될 메서드
        /// </summary>
        public void SetFuntionButton()
        {
            var boNPC = UIWindowManager.Instance.GetWindow<UIDialogue>().boNPC;

            switch (boNPC.sdNPC.npcType)
            {
                case Define.Actor.NPCType.Store:
                    btn.onClick.AddListener(() => { OnClickShop(boNPC); });
                    SetUI(SpriteLoader.GetSprite(Define.Resource.AtlasType.UIAtlase, "coin"));
                    break;
            }

            void SetUI(Sprite sprite)
            {
                icon.sprite = sprite;
                title.text = boNPC.sdNPC.npcType.ToString();
            }
        }

        /// <summary>
        /// => 상점 버튼에 바인딩될 메서드
        /// </summary>
        /// <param name="boNPC"> 상점 NPC 데이터 </param>
        private void OnClickShop(BoNPC boNPC)
        {
            // -> 다이얼로그 창을 닫습니다!
            var uiWindowManager = UIWindowManager.Instance;
            uiWindowManager.GetWindow<UIDialogue>().Close();

            // -> 상점 창을 엽니다!
            uiWindowManager.GetWindow<UIStore>().Open(boNPC);
        }
    }
}