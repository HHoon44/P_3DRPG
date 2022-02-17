using ProjectChan.DB;
using ProjectChan.Dummy;
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
    /// <summary>
    /// => NPC 객체가 지닐 클래스
    /// </summary>
    public class NPC : MonoBehaviour
    {
        // public 
        public Animator anim;           // -> NPC 애니메이션

        // private
        private float currentAnimTime;  // -> 현재 애니메이션 실행 시간
        private float maxAnimTime;      // -> 최대 애니메이션 실행 시간
        private BoNPC boNPC;            // -> NPC Bo데이터
        private Collider coll;          // -> NPC가 가진 콜라이더

        /// <summary>
        /// => NPC 첫 세팅 메서드
        /// </summary>
        /// <param name="boNPC"> NPC 정보 </param>
        public void Initialize(BoNPC boNPC)
        {
            this.boNPC = boNPC;
            coll ??= transform.Find("Area").GetComponent<Collider>();

            // -> SD데이터에 있는 Position 값과 Rotation 값을 이용하여 Position과 Rotation을 세팅합니다!
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

        #region NPC 애니메이션

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

        #endregion

        /// <summary>
        /// => NPC에 닿은 객체들을 체크하는 메서드
        /// </summary>
        private void CheckInteraction()
        {
            // -> NPC의 상호작용 에리어에 플레이어가 닿았는지 확인합니다!
            var colls = Physics.OverlapBox
                (coll.bounds.center, coll.bounds.extents, transform.rotation, 1 << LayerMask.NameToLayer("Player"));

            // -> 범위에 들어온 플레이어가 없다면!
            if (colls.Length == 0)
            {
                return;
            }

            // -> 현재 범위에 들어온 플레이어 입니다!
            var playerController = colls[0].GetComponentInParent<PlayerController>();

            if (Input.GetButtonDown(Define.Input.Interaction) && !playerController.isPlayerAction)
            {
                // -> 대화를 할땐 플레이어의 이동과 키입력을 막습니다!
                var character = colls[0].gameObject.GetComponent<Character>();
                character.boActor.moveDir = Vector3.zero;
                var newDir = Vector3.zero;
                character.boActor.rotDir = newDir;

                // -> 현재 NPC와 대화하는 플레이어를 담아둡니다!
                // -> 플레이어가 행동을 합니다!
                boNPC.actor = playerController;
                boNPC.actor.isPlayerAction = true;


                #region NPC 대화 퀘스트 작업 처리!

                var boProgressQuests = GameManager.User.boQuest.progressQuests;

                // -> 대화 퀘스트가 존재하는 곳의 인덱스 값 입니다!
                var conversationQuest = -1;

                // -> 진행중인 퀘스트 목록에 NPC 대화 퀘스트가 있는지 확인하는 작업 입니다!
                for (int i = 0; i < boProgressQuests.Count; i++)
                {
                    // -> 만약 진행중인 퀘스트 중에 NPC와 대화 하는 퀘스트가 존재 한다면?
                    if (boProgressQuests[i].sdQuest.questType == Define.QuestType.Conversation)
                    {
                        conversationQuest = i;
                        break;
                    }
                }

                if (conversationQuest != -1)
                {
                    // -> 대화 퀘스트 대상이 현재 NPC인지 확인하는 작업 입니다!
                    for (int i = 0; i < boProgressQuests[conversationQuest].sdQuest.target.Length; i++)
                    {
                        // -> 만약 대화 퀘스트 대상목록에 현재 NPC가 포함 되어있다면!
                        if (boProgressQuests[conversationQuest].sdQuest.target[i] == boNPC.sdNPC.index)
                        {
                            // -> 퀘스트 디테일 값을 세팅 합니다!
                            boProgressQuests[conversationQuest].details[i] = boNPC.sdNPC.index;

                            var dummyServer = DummyServer.Instance;
                            dummyServer.userData.dtoQuest.progressQuests[conversationQuest].details = boProgressQuests[conversationQuest].sdQuest.questDetail;
                            dummyServer.Save();
                        }
                    }
                }

                #endregion


                OnDialogue();
            }
            else if (Input.GetButtonDown(Define.Input.Interaction) && playerController.isPlayerAction)
            {
                var uiWindowManager = UIWindowManager.Instance;

                // -> 상점을 닫을려고 한다면!
                if (uiWindowManager.GetWindow<UIStore>().isOpen)
                {
                    return;
                }
                else
                {
                    // -> 다이얼로그 창을 닫습니다!
                    uiWindowManager.GetWindow<UIDialogue>().Close();

                    // -> 플레이어가 NPC와 대화를 종료합니다!
                    boNPC.actor.isPlayerAction = false;
                    boNPC.actor = null;
                }
            }
        }

        /// <summary>
        /// => 다이얼로그 활성화 메서드
        /// </summary>
        private void OnDialogue()
        {
            var uiDialogue = UIWindowManager.Instance.GetWindow<UIDialogue>();


            #region 퀘스트 인덱스 걸러내기 작업!

            var boQuests = GameManager.User.boQuest;
            var sdQuests = GameManager.SD.sdQuests;

            // -> NPC가 지닌 퀘스트 목록에서 Bo에 저장 되어있는 완료 퀘스트 목록과 같은 인덱스 들을 지웁니다!
            var canOrderQuest = boNPC.sdNPC.questIndex.Except(boQuests.completedQuests.Select(obj => obj.index));

            // -> 남은 데이터를 리스트로 저장합니다!
            var resultQusts = canOrderQuest.ToList();

            // -> 선행 퀘스트의 여부를 확인하는 작업 입니다!
            for (int i = 0; i < resultQusts.Count(); i++)
            {
                // -> 0번째 퀘스트 인덱스가 0이라면 퀘스트가 없다는 의미 입니다!
                if (resultQusts[0] == 0)
                {
                    continue;
                }

                // -> resultsQuests와 같은 인덱스를 가진 퀘스트 기획 데이터를 가져온 뒤 그 기획 데이터의 선행 퀘스트 목록을 가져옵니다!
                var antencedentIndex = sdQuests.Where(obj => obj.index == resultQusts[i])?.SingleOrDefault().antecedentQuest;

                // -> 만약 선행 퀘스트 목록의 첫번째 값이 0이라면!
                if (antencedentIndex[0] == 0)
                {
                    continue;
                }

                /// => Array.Intersect : 두개의 배열을 비교하여 교집합의 개수를 구해낸다
                // -> 선행 퀘스트의 배열 길이와 선행 퀘스트 값과 이미 클리어한 퀘스트 값의 교집합 개수가 같지 않다면!
                if (antencedentIndex.Length !=
                    antencedentIndex.Intersect(boQuests.completedQuests.Select(obj => obj.index)).Count())
                {
                    // -> 선행 퀘스트를 완료하지 못했으므로 NPC의 퀘스트 목록에서 제거 합니다!
                    resultQusts.RemoveAt(i);
                    i--;
                }
            }

            // -> 최종적으로 남은 퀘스트를 BoNPC 데이터에 저장합니다!
            boNPC.quests = resultQusts.ToArray();

            #endregion


            // -> NPC가 지닌 기본 대화 배열의 길이를 이용하여 랜덤 값을 구합니다!
            // -> 랜덤 값을 통해 기본 대화 데이터를 가져와서 다이얼로그를 세팅합니다!
            var randIndex = Random.Range(0, boNPC.sdNPC.baseSpeech.Length);
            var speechIndex = boNPC.sdNPC.baseSpeech[randIndex];
            var sdNovel = GameManager.SD.sdNovels.Where(obj => obj.index == speechIndex)?.SingleOrDefault();
            var boNovel = new BoNovel(sdNovel);

            uiDialogue.Initialize(boNovel, boNPC);
        }
    }
}