using ProjectChan.Novel;
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

namespace ProjectChan.UI
{
    /// <summary>
    /// => 현재까지의 이야기 요약과 다음 스테이지로 넘어가기 위한 셋팅을 해주는 클래스
    /// </summary>
    public class UIInfo : UIWindow
    {
        public TextMeshProUGUI title;   // -> 현재까지 이야기에 대한 요약 
        public Button btn;              // -> 다음 스테이지로 넘어가기 위한 버튼

        /// <summary>
        /// => 다음 스테이지로 넘어가기 위한 셋팅
        /// </summary>
        /// <param name="sdNovel"> 현재 노벨 기획 데이터 </param>
        public void Initialize(SDNovel sdNovel)
        {
            title.text = sdNovel.kr;

            switch (sdNovel.charType)
            {
                case Define.Actor.CharType.None:
                    btn.onClick.AddListener(() =>
                    {
                        NextStageSet();
                    }
                    );
                    break;
            }
        }

        /// <summary>
        /// => 다음 스테이지를 불러오는 작업
        /// </summary>
        private void NextStageSet()
        {
            var stageManager = StageManager.Instance;

            // -> 다음 씬을 불러옵니다!
            GameManager.Instance.LoadScene(Define.SceneType.InGame, stageManager.ChangeStage(), stageManager.OnChangeStageComplete);
        }
    }
}