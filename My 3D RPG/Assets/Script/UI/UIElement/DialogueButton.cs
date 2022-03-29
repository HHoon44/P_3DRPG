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
    /// 다이얼로그 버튼을 관리하는 클래스
    /// </summary>
    public class DialogueButton : MonoBehaviour, IPoolableObject
    {
        // public
        public Image icon;                  // 버튼의 아이콘
        public TextMeshProUGUI title;       // 버튼의 텍스트
        public Button btn;                  // 버튼의 버튼 기능

        /// <summary>
        /// 재사용 가능 여부
        /// </summary>
        public bool CanRecycle { get; set; } = true;

        /// <summary>
        /// 다이얼로그 버튼을 초기화 하는 메서드
        /// </summary>
        /// <param name="questIndex"> 버튼이 지닐 퀘스트 인덱스 </param>
        public void Initialize(int questIndex)
        {
            icon.sprite = SpriteLoader.GetSprite(Define.Resource.AtlasType.UIAtlase, "exclamation");

            // 버튼이 지닐 퀘스트 기획 데이터
            var sdQuest = GameManager.SD.sdQuests.Where(obj => obj.index == questIndex)?.SingleOrDefault();

            title.text = sdQuest.name;
            btn.onClick.AddListener(() => { OnClickQuest(sdQuest); });
        }

        /// <summary>
        /// 퀘스트 다이얼로그 버튼에 바인딩될 메서드
        /// </summary>
        /// <param name="sdQuest"> 버튼이 지닌 퀘스트 기획 데이터 </param>
        private void OnClickQuest(SDQuest sdQuest)
        {
            var uiWindowManager = UIWindowManager.Instance;

            // 다이얼로그 창을 닫음
            uiWindowManager.GetWindow<UIDialogue>().Close();

            // 진행중인 퀘스트 목록
            var progressQuests = GameManager.User.boQuest.progressQuests;

            // 버튼의 퀘스트가 진행중인 퀘스트 목록에 존재하는지 확인하는 작업
            var progressQuest = progressQuests.Where(obj => obj.sdQuest.index == sdQuest.index).SingleOrDefault();

            if (progressQuest == null)
            {
                // 새로운 퀘스트 이므로 수락/거절 창을 활성화
                uiWindowManager.GetWindow<UIQuest>().orderQuestType = OrderQuestType.NoProgress;
            }
            else
            {
                // 진행중인 퀘스트 이므로 진행도를 나타내는 창을 활성화
                uiWindowManager.GetWindow<UIQuest>().orderQuestType = OrderQuestType.Progress;

                var progressQuestDetailLength = progressQuest.sdQuest.questDetail.Length;

                // 완료한 디테일 개수
                var progressDetail = 0;

                // 진행중인 퀘스트이지만 완료조건을 만족한 퀘스트인지 확인하는 작업
                for (int i = 0; i < progressQuestDetailLength; i++)
                {
                    // 완료 조건의 값과 진행중인 퀘스트의 디테일 값이 같다면
                    if (progressQuest.sdQuest.questDetail[i] == progressQuest.details[i])
                    {
                        // 조건 완료를 나타내는 변수에 +1을 함
                        progressDetail++;
                    }
                }

                // 디테일 배열의 길이와 완료한 디테일 개수에 대한 변수가 같다면
                if (progressQuestDetailLength == progressDetail)
                {
                    // 클리어한 퀘스트 이므로 클리어 창을 활성화
                    uiWindowManager.GetWindow<UIQuest>().orderQuestType = OrderQuestType.Clear;
                }
            }

            // 퀘스트 창 활성화
            uiWindowManager.GetWindow<UIDialogue>().boNPC.actor.isPlayerAction = false;
            uiWindowManager.GetWindow<UIQuest>().Open(QuestWindow.Order, sdQuest);
        }

        /// <summary>
        /// NPC가 지닌 기능에 대한 버튼을 설정하는 메서드
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
        /// 상점 버튼에 바인딩될 메서드
        /// </summary>
        /// <param name="boNPC"> 상점 NPC 데이터 </param>
        private void OnClickShop(BoNPC boNPC)
        {
            var uiWindowManager = UIWindowManager.Instance;

            // 다이얼로그 창을 닫음
            uiWindowManager.GetWindow<UIDialogue>().Close();

            // 상점 창을 오픈
            uiWindowManager.GetWindow<UIStore>().Open(boNPC);
        }
    }
}