using ProjectChan.DB;
using ProjectChan.Object;
using ProjectChan.Resource;
using ProjectChan.SD;
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
    /// NPC 대화 창 UI를 관리하는 클래스
    /// </summary>
    public class UIDialogue : UIWindow
    {
        // public 
        public Transform buttonHolder;              // 다이얼로그 버튼들이 생성될 홀더
        public Transform functionHolder;            // 기능 버튼들이 생성될 홀더

        // private
        private UINovel uiNovelSet;                 // 대화창 셋

        /// <summary>
        /// 현재 플레이어와 대화 중인 NPC 정보
        /// </summary>
        public BoNPC boNPC { get; private set; }

        /// <summary>
        /// 활성화 된 다이얼로그 버튼을 담아놓을 리스트
        /// </summary>
        private List<DialogueButton> dialogueButtons = new List<DialogueButton>();

        /// <summary>
        /// 다이얼로그를 초기화 하는 메서드
        /// </summary>
        /// <param name="boNovel">  노벨셋에 사용할 Bo데이터 </param>
        /// <param name="boNPC">    현재 대화하는 NPC Bo데이터 </param>
        /// <param name="actor">    현재 NPC와 대화하는 플레이어 </param>
        public void Initialize(SDNovel sdNovel, BoNPC boNPC)
        {
            this.boNPC = boNPC;

            // NPC의 대사와 일러스트를 보여주기 위해, 인 게임의 노벨 UI를 세팅
            uiNovelSet = transform.Find("NovelSet").GetComponent<UINovel>();
            uiNovelSet.SetNovel(sdNovel);

            // 다이얼로그 버튼 활성화
            OnDialogueButton();

            // 다이얼로그 창 활성화
            Open();
        }

        /// <summary>
        /// 다이얼로그 버튼을 활성화 하는 메서드
        /// </summary>
        private void OnDialogueButton()
        {
            var pool = ObjectPoolManager.Instance.GetPool<DialogueButton>(Define.PoolType.DialogueButton);

            // NPC가 기능을 가진 NPC라면
            if (boNPC.sdNPC.npcType != Define.Actor.NPCType.Normal)
            {
                // 기능 버튼을 세팅
                var button = pool.GetPoolableObject(obj => obj.CanRecycle);
                button.transform.SetParent(functionHolder);
                button.SetFuntionButton();

                dialogueButtons.Add(button);
                button.gameObject.SetActive(true);
            }

            // NPC가 지닌 퀘스트가 없다면
            if (boNPC.quests.Length == 0)
            {
                return;
            }
            else
            {
                if (boNPC.quests[0] == 0)
                {
                    return;
                }

                // NPC가 지닌 퀘스트 개수만큼 다이얼로그 버튼을 세팅하는 작업
                for (int i = 0; i < boNPC.quests.Length; i++)
                {
                    // 풀에서 사용가능한 다이얼로그 버튼의 오브젝트를 가져옴
                    var button = pool.GetPoolableObject(obj => obj.CanRecycle);
                    button.transform.SetParent(buttonHolder);

                    // 버튼에 NPC가 지닌 퀘스트 데이터를 전달
                    button.Initialize(boNPC.quests[i]);

                    dialogueButtons.Add(button);
                    button.gameObject.SetActive(true);
                }
            }
        }

        public override void Close(bool force = false)
        {
            base.Close(force);

            // 다이얼로그 버튼을 풀로 반환하기 위해, 풀을 가져옴
            var pool = ObjectPoolManager.Instance.GetPool<DialogueButton>(Define.PoolType.DialogueButton);

            // 버튼을 풀로 반환하는 작업
            for (int i = 0; i < dialogueButtons.Count; i++)
            {
                pool.ReturnPoolableObject(dialogueButtons[i]);
            }

            dialogueButtons.Clear();
        }
    }
}