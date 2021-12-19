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
        public bool CanRecycle { get; set; } = true;

        public Image icon;                  // -> 다이얼로그 버튼의 아이콘
        public TextMeshProUGUI title;       // -> 다이얼로그 버튼의 텍스트
        public Button btn;                  // -> 다이얼로그 버튼의 버튼 기능

        /// <summary>
        /// => 다이얼로그 버튼의 초기 세팅
        /// </summary>
        /// <param name="questIndex"> 현재 NPC의 퀘스트 인덱스 </param>
        /// <param name="boNPC"> 현재 NPC가 어떤 타입의 NPC인지 확인하기 위한 데이터 </param>
        public void Initialize(int questIndex)
        {
            icon.sprite = SpriteLoader.GetSprite(Define.Resource.AtlasType.UIAtlase, "exclamation");

            // -> 현재 진행중인 퀘스트 목록을 가져온다
            var progressQuests = GameManager.User.boQuest.progressQuests;
            var progressQuestIndex = -1;


            // -> 현재 다이얼로그에 세팅할 퀘스트 정보가 진행중인 퀘스트 인지 확인하는 작업
            for (int i = 0; i < progressQuests.Count; i++)
            {
                // -> 진행중인 퀘스트라면
                if (progressQuests[i].sdQuest.index == questIndex)
                {
                    progressQuestIndex = i;
                    break;
                }
            }

            var sdQuest = GameManager.SD.sdQuests.Where(obj => obj.index == questIndex)?.SingleOrDefault();
            title.text = sdQuest.name;
            btn.onClick.AddListener(() => { OnClickQuest(sdQuest, progressQuestIndex); });
        }

        /// <summary>
        /// => 기능 버튼을 세팅하는 작업
        /// </summary>
        /// <param name="boNPC"> 기능 버튼을 지닌 NPC의 데이터 </param>
        public void SetFuntionButton(BoNPC boNPC)
        {
            // -> 나중가서 NPCType마다 나눌듯
            switch (boNPC.sdNPC.npcType)
            {
                case Define.Actor.NPCType.Store:
                    btn.onClick.AddListener(() => { OnClickShop(boNPC); });
                    SetUI(SpriteLoader.GetSprite(Define.Resource.AtlasType.UIAtlase, "coin"));
                    break;
            }

            // -> 공통 작업은 로컬함수로
            void SetUI(Sprite sprite)
            {
                icon.sprite = sprite;
                title.text = boNPC.sdNPC.npcType.ToString();
            }
        }

        /// <summary>
        /// => 상점 NPC에 대한 버튼에 바인딩 될 메서드
        /// </summary>
        private void OnClickShop(BoNPC boNPC)
        {
            // -> 다이얼로그 창을 닫습니다!
            var uiWindowManager = UIWindowManager.Instance;
            uiWindowManager.GetWindow<UIDialogue>().Close();

            // -> 상점 창을 엽니다!
            uiWindowManager.GetWindow<UIStore>().Open(boNPC);
        }

        /// <summary>
        /// => NPC가 지닌 퀘스트에 대한 버튼에 바인딩 될 메서드
        /// </summary>
        /// <param name="sdQuest"></param>
        private void OnClickQuest(SDQuest sdQuest, int progressQuestIndex)
        {
            // -> 다이얼로그 창을 닫습니다!
            var uiWindowManager = UIWindowManager.Instance;
            uiWindowManager.GetWindow<UIDialogue>().Close();

            // -> 아직 진행중인 퀘스트가 아니므로
            if (progressQuestIndex < 0)
            {
                // -> 미진행 퀘스트
                uiWindowManager.GetWindow<UIQuest>().orderTab = QuestOrderTab.None;

            }
            // -> 진행중인 퀘스트 이므로
            else
            {
                // -> 진행중인 퀘스트
                uiWindowManager.GetWindow<UIQuest>().orderTab = QuestOrderTab.Progress;
                var progressQuest = GameManager.User.boQuest.progressQuests[progressQuestIndex];
                var detailsLength = sdQuest.questDetail.Length;

                // -> 완료한 디테일 개수
                var progressDetail = 0;

                for (int i = 0; i < detailsLength; i++)
                {
                    if (sdQuest.questDetail[i] == progressQuest.details[i])
                    {
                        progressDetail++;
                    }
                }

                // -> 만약 디테일 개수와 퀘스트 기획데이터가 가진 디테일 배열의 길이와 같다면
                if (progressDetail == detailsLength)
                {
                    // -> 클리어한 퀘스트
                    uiWindowManager.GetWindow<UIQuest>().orderTab = QuestOrderTab.Clear;
                }
            }

            // -> 오더창 열기
            uiWindowManager.GetWindow<UIQuest>().Open(QuestWindow.Order, sdQuest);
        }
    }
}