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
    /// => 퀘스트 슬롯을 관리하는 메서드
    /// </summary>
    public class QuestSlot : MonoBehaviour, IPoolableObject
    {
        public bool CanRecycle { get; set; } = true;

        public TextMeshProUGUI title;       // -> 퀘스트 슬롯 타이틀
        public Button btn;                  // -> 퀘스트 슬롯 버튼 기능

        // -> 아직 디테일 안넣었는데
        public void Initialize(SDQuest sdQuest, QuestTab currentTab)
        {
            title.text = sdQuest.name;

            // -> 이전에 들어있는 기능들 다 삭제
            btn.onClick.RemoveAllListeners();

            if (currentTab == QuestTab.Progress)
            {
                btn.onClick.AddListener(() => { OpenQuestWindow(sdQuest); });
            }
        }

        /// <summary>
        /// => 퀘스트 슬롯 버튼을 눌렀을 시 이벤트 바인딩 멧더ㅡ
        /// </summary>
        /// <param name="sdQuest"> 이벤트에 보내줄 퀘스트 기획 데이터 </param>
        private void OpenQuestWindow(SDQuest sdQuest)
        {
            var uiQuest = UIWindowManager.Instance.GetWindow<UIQuest>();
            uiQuest.Open(Define.Quest.QuestWindow.Content, sdQuest);
            uiQuest.listWindow.gameObject.SetActive(!uiQuest.listWindow.gameObject.activeSelf);
        }
    }
}