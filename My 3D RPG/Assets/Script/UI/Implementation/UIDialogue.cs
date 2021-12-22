using ProjectChan.DB;
using ProjectChan.Object;
using ProjectChan.Resource;
using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.UI
{
    /// <summary>
    /// => 대화창을 관리할 클래스
    /// </summary>
    public class UIDialogue : UIWindow
    {
        private UINovel uiNovelSet;                 // -> 대화창 셋
        private BoNovel boNovel;                    // -> 대화창을 세팅할때 사용할 데이터
        //private BoNPC boNPC;                        // -> 현재 대화하는 NPC데이터
        public BoNPC boNPC { get; private set; }

        public Transform buttonHolder;              // -> 다이얼로그 버튼들이 생성될 홀더
        public Transform functionHolder;            // -> 기능 버튼들이 생성될 홀더

        /// <summary>
        /// => 현재 활성화된 다이얼 로그 버튼들을 저장해놓는 곳
        /// </summary>
        private List<DialogueButton> dialogueButtons = new List<DialogueButton>();  

        /// <summary>
        /// => UIDialogue를 초기 설정하는 메서드
        /// </summary>
        /// <param name="boNovel"> 노벨셋에 사용할 Bo데이터 </param>
        /// <param name="boNPC"> 현재 대화하는 NPC Bo데이터 </param>
        /// <param name="actor"> 현재 NPC와 대화하는 플레이어 </param>
        public void Initialize(BoNovel boNovel, BoNPC boNPC)
        {
            this.boNovel = boNovel;
            this.boNPC = boNPC;

            uiNovelSet = transform.Find("NovelSet").GetComponent<UINovel>();

            uiNovelSet.SetNovel(this.boNovel);

            OnDialogueButton();

            Open();
        }

        /// <summary>
        /// => 다이얼로그 버튼을 활성화하는 메서드
        /// </summary>
        private void OnDialogueButton()
        {
            var pool = ObjectPoolManager.Instance.GetPool<DialogueButton>(Define.PoolType.DialogueButton);

            // -> 기능 버튼을 세팅합니다!
            if (boNPC.sdNPC.npcType != Define.Actor.NPCType.Normal)
            {
                var button = pool.GetPoolableObject();
                button.transform.SetParent(functionHolder);
                button.SetFuntionButton(boNPC);

                dialogueButtons.Add(button);
                button.gameObject.SetActive(true);
            }

            // -> NPC가 지닌 퀘스트가 없다면!
            if (boNPC.quests.Length == 0)
            {
                return;
            }

            // -> 지닌 퀘스트가 있다면 퀘스트 버튼을 세팅합니다!
            if (boNPC.quests[0] != 0)
            {
                for (int i = 0; i < boNPC.quests.Length; i++)
                {
                    var button = pool.GetPoolableObject();
                    button.transform.SetParent(buttonHolder);
                    button.Initialize(boNPC.quests[i]);

                    dialogueButtons.Add(button);
                    button.gameObject.SetActive(true);
                }
            }
        }

        public override void Close(bool force = false)
        {
            base.Close(force);

            var pool = ObjectPoolManager.Instance.GetPool<DialogueButton>(Define.PoolType.DialogueButton);

            // -> 가져왔던 버튼들을 다시 돌려줍니다1
            for (int i = 0; i < dialogueButtons.Count; i++)
            {
                pool.ReturnPoolableObject(dialogueButtons[i]);
            }

            // -> 청소!
            dialogueButtons.Clear();
        }
    }
}