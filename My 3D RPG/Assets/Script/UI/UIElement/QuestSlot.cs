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
    /// => 퀘스트 슬롯을 관리하는 클래스
    /// </summary>
    public class QuestSlot : MonoBehaviour, IPoolableObject
    {
        public TextMeshProUGUI title;       // -> 퀘스트 슬롯 타이틀
        public Button btn;                  // -> 퀘스트 슬롯 버튼 기능

        /// <summary>
        /// => 재사용 가능한지
        /// </summary>
        public bool CanRecycle { get; set; } = true;

        public void Initialize(SDQuest sdQuest, QuestTab currentTab)
        {
            title.text = sdQuest.name;

            // -> 이전에 들어있는 기능들 다 삭제
            btn.onClick.RemoveAllListeners();

            // -> 현재 리스스 탭이 진행중인 리스트라면!
            if (currentTab == QuestTab.Progress)
            {
                // -> 슬롯에 이벤트를 바인딩 해줍니다!
                btn.onClick.AddListener(() => { OpenQuestWindow(sdQuest); });
            }
        }

        /// <summary>
        /// => 진행중인 퀘스트의 슬롯을 눌렀을 시 이벤트 바인딩 메서드
        /// </summary>
        /// <param name="sdQuest"> 이벤트에 보내줄 퀘스트 기획 데이터 </param>
        private void OpenQuestWindow(SDQuest sdQuest)
        {
            // -> 퀘스트 정보 창을 열어줍니다!
            var uiQuest = UIWindowManager.Instance.GetWindow<UIQuest>();
            uiQuest.Open(QuestWindow.Info, sdQuest);
            uiQuest.listWindow.gameObject.SetActive(!uiQuest.listWindow.gameObject.activeSelf);
        }
    }
}