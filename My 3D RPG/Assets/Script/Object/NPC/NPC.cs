﻿using ProjectChan.DB;
using ProjectChan.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProjectChan.Object
{
    public class NPC : MonoBehaviour
    {
        private float currentAnimTime;  // -> 현재 애니메이션 실행 시간
        private float maxAnimTime;      // -> 최대 애니메이션 실행 시간
        private BoNPC boNPC;            // -> NPC Bo데이터
        private Collider coll;          // -> NPC가 가진 콜라이더
        public Animator anim;           // -> NPC 애니메이션

        /// <summary>
        /// => NPC 첫 세팅
        /// </summary>
        /// <param name="boNPC"> NPC 정보 </param>
        public void Initialize(BoNPC boNPC)
        {
            this.boNPC = boNPC;
            coll ??= transform.Find("Area").GetComponent<Collider>();

            // -> SD데이터에 있는 Position 값과 Rotation 값을 가지고 온다
            var npcPos = new Vector3(boNPC.sdNPC.npcPos[0], boNPC.sdNPC.npcPos[1], boNPC.sdNPC.npcPos[2]);
            var npcRot = new Vector3(boNPC.sdNPC.npcRot[0], boNPC.sdNPC.npcRot[1], boNPC.sdNPC.npcRot[2]);

            gameObject.name = boNPC.sdNPC.resourcePath.Remove(0, boNPC.sdNPC.resourcePath.LastIndexOf('/') + 1);

            // -> 위에서 가지고온 Pos값과 Rot값을 오브젝트에 적용
            transform.position = npcPos;
            transform.eulerAngles = npcRot;
        }

        /// <summary>
        /// => NPC의 업데이트 메서드
        /// </summary>
        public void NPCUpdate()
        {
            CheckInteraction();

            CheckAnimTime();

        }

        private void CheckAnimTime()
        {
            currentAnimTime += Time.deltaTime;

            if (currentAnimTime >= maxAnimTime)
            {
                var index = boNPC.sdNPC.resourcePath.LastIndexOf('/');
                var name = boNPC.sdNPC.resourcePath.Remove(0, boNPC.sdNPC.resourcePath.LastIndexOf('/') + 1);
                anim.SetTrigger(name);
                anim.SetBool("isMotion", true);

                ClearNPCAnimTime();
            }

        }

        private void ClearNPCAnimTime()
        {
            currentAnimTime = 0;
            maxAnimTime = Random.Range(1, 3);
        }

        public void NPCBaseIdle()
        {
            anim.SetBool("isMotion", false);
        }

        /*
        private void OnDrawGizmos()
        {
            var halfExtents = new Vector3(coll.bounds.center.x, coll.bounds.center.y, coll.bounds.center.z);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, halfExtents);
        }
        */

        /// <summary>
        /// => NPC에 닿은 객체들을 체크하는 메서드
        /// </summary>
        private void CheckInteraction()
        {
            var colls = Physics.OverlapBox
                (coll.bounds.center, coll.bounds.extents, transform.rotation, 1 << LayerMask.NameToLayer("Player"));

            // -> 범위에 들어온 플레이어가 없다면!
            if (colls.Length == 0)
            {
                return;
            }

            // -> 현재 범위에 들어온 플레이어!
            var playerController = colls[0].GetComponentInParent<PlayerController>();

            // -> 다이얼로그 버튼 실행
            if (Input.GetButtonDown(Define.Input.Interaction) && !playerController.isPlayerAction)
            {
                // -> 대화를 할땐 플레이어의 이동과 키입력을 막습니다!
                var character = colls[0].gameObject.GetComponent<Character>();
                character.boActor.moveDir = Vector3.zero;
                var newDir = Vector3.zero;
                character.boActor.rotDir = newDir;

                // -> 현재 NPC와 대화하는 플레이어를 세팅합니다!
                boNPC.actor = playerController;

                // -> 플레이어가 행동을 합니다!
                boNPC.actor.isPlayerAction = true;

                ///OnDialogue(playerController);
                OnDialogue();
            }
            // -> NPC와 대화중에 한번더 키 입력을 했다면!
            else if (Input.GetButtonDown(Define.Input.Interaction) && playerController.isPlayerAction)
            {
                // -> 다이얼로그 창을 닫습니다!
                UIWindowManager.Instance.GetWindow<UIDialogue>().Close();

                // -> 플레이어가 NPC와 대화를 종료합니다!
                boNPC.actor.isPlayerAction = false;
                boNPC.actor = null;
            }
        }

        /// <summary>
        /// => 다이얼로그 활성화 메서드
        /// </summary>
        /// <param name="actor"> 현재 NPC와 대화중인 액터데이터 </param>
        private void OnDialogue()
        {
            // -> 실행시킬 UIDialogue를 가져온다
            var uiDialogue = UIWindowManager.Instance.GetWindow<UIDialogue>();

            #region 퀘스트 인덱스 걸러내기

            // -> 현재 게임에 저장되어있는 Bo데이터를 가져온다
            var boQuests = GameManager.User.boQuest;

            // -> 현재 NPC가 지닌 퀘스트 인덱스 목록에서 현재 플레이어가 진행중인 퀘스트 정보들을 지운다
            //var canOrderQuest = boNPC.sdNPC.questIndex.Except(boQuests.progressQuests.Select(obj => obj.sdQuest.index));

            // -> NPC가 지닌 퀘스트 인덱스 목록에서 완료한 퀘스트 목록을 지운다
            var canOrderQuest = boNPC.sdNPC.questIndex.Except(boQuests.completedQuests.Select(obj => obj.index));

            // -> 위에 2개의 조건으로 걸러진 데이터를 List로 담아둔다
            /// -> 엠버가 지닌 1000번째 인덱스 퀘스트의 인덱스 값이 담김
            var resultQusts = canOrderQuest.ToList();

            // -> 퀘스트들이 담긴 기획 데이터를 가져옴
            var sdQuests = GameManager.SD.sdQuests;

            // -> 선행 퀘스트의 여부를 확인하는 작업
            /// -> 엠버는 1개의 퀘스트를 지녔으므로 1번 돔
            for (int i = 0; i < resultQusts.Count(); i++)
            {
                // -> 0번째 퀘스트 인덱스가 0이라면 퀘스트가 없다는 의미이므로
                if (resultQusts[0] == 0)
                {
                    continue;
                }

                // -> 현재 NPC의 퀘스트 인덱스 목록(resultQuests)과 같은 인덱스를 가진 퀘스트 기획 데이터를 가져온다
                //    만약 가져온다면 그 기획 데이터의 선행 퀘스트 목록을 가져온다
                /// -> 엠버가 가진 퀘스트는 선행 퀘스트를 필요로 하지 않으므로
                var antencedentIndex = sdQuests.Where(obj => obj.index == resultQusts[i])?.SingleOrDefault().antecedentQuest;

                // -> 만약 선행 퀘스트 목록의 0번째 값이 0이라면 
                /// -> 바로 여기로 들어옴
                if (antencedentIndex[0] == 0)
                {
                    continue;
                }

                /// => Array.Intersect : 두개의 배열을 비교하여 교집합의 개수를 구해낸다
                // -> 선행 퀘스트 길이와 선행 퀘스트와 이미 클리어한 퀘스트의 교집합의 개수가 같지않다면
                if (antencedentIndex.Length !=
                    antencedentIndex.Intersect(boQuests.completedQuests.Select(obj => obj.index)).Count())
                {
                    // -> 아직 i번째 퀘스트는 선행 퀘스트를 완료하지 못했으므로 NPC의 퀘스트 목록에서 제거해준다
                    resultQusts.RemoveAt(i);
                    i--;
                }
            }

            // -> 위의 조건등을 통해서 걸러진 퀘스트 목록을 Bo에 저장
            /// -> 여기엔 1000번째 퀘스트가 담김
            boNPC.quests = resultQusts.ToArray();

            #endregion

            // -> 현재 NPC가 가지고 있는 speechIndex의 길이까지 랜덤값을 구한다
            // -> 구한 랜덤값과 같은 인덱스를 가진 노벨 기획데이터를 가져와서 Bo에 저장한다
            var randIndex = Random.Range(0, boNPC.sdNPC.baseSpeech.Length);
            var speechIndex = boNPC.sdNPC.baseSpeech[randIndex];
            var sdNovel = GameManager.SD.sdNovels.Where(obj => obj.index == speechIndex)?.SingleOrDefault();
            var boNovel = new BoNovel(sdNovel);

            ///uiDialogue.Initialize(boNovel, boNPC, actor);
            uiDialogue.Initialize(boNovel, boNPC);
        }
    }
}