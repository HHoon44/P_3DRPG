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
    /// NPC 오브젝트의 기능을 관리하는 클래스
    /// </summary>
    public class NPC : MonoBehaviour
    {
        // public 
        public Animator anim;           // NPC 애니메이션

        // private
        private float currentAnimTime;  // 현재 애니메이션 실행 시간
        private float maxAnimTime;      // 최대 애니메이션 실행 시간
        private BoNPC boNPC;            // NPC Bo데이터
        private Collider coll;          // NPC가 가진 콜라이더

        /// <summary>
        /// NPC 초기화 메서드
        /// </summary>
        /// <param name="boNPC"> NPC 정보 </param>
        public void Initialize(BoNPC boNPC)
        {
            this.boNPC = boNPC;
            coll ??= transform.Find("Area").GetComponent<Collider>();

            // 전달 받은 Bo에 있는 NPC 기획 데이터값을 이용해서 Pos와 Rot을 작성
            var npcPos = new Vector3(boNPC.sdNPC.npcPos[0], boNPC.sdNPC.npcPos[1], boNPC.sdNPC.npcPos[2]);
            var npcRot = new Vector3(boNPC.sdNPC.npcRot[0], boNPC.sdNPC.npcRot[1], boNPC.sdNPC.npcRot[2]);

            gameObject.name = boNPC.sdNPC.resourcePath.Remove(0, boNPC.sdNPC.resourcePath.LastIndexOf('/') + 1);

            // 위에서 작성한 Pos와 Rot을 NPC에게 적용
            transform.position = npcPos;
            transform.eulerAngles = npcRot;
        }

        /// <summary>
        /// NPC의 업데이트 메서드
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
        /// NPC에 닿은 유저를 체크하는 메서드
        /// </summary>
        private void CheckInteraction()
        {
            // NPC에게 유저가 닿았는지 체크
            var colls = Physics.OverlapBox
                (coll.bounds.center, coll.bounds.extents, transform.rotation, 1 << LayerMask.NameToLayer("Player"));

            // 닿은 유저가 없다면
            if (colls.Length == 0)
            {
                return;
            }

            // 닿은 유저가 있다면 0번째 유저의 데이터를 가져옴
            var playerController = colls[0].GetComponentInParent<PlayerController>();

            if (Input.GetButtonDown(Define.Input.Interaction) && !playerController.isPlayerAction)
            {
                // 상호작용 시, 유저의 이동, 키 입력을 막음
                var character = colls[0].gameObject.GetComponent<Character>();
                character.boActor.moveDir = Vector3.zero;
                var newDir = Vector3.zero;
                character.boActor.rotDir = newDir;

                // 현재 NPC와 상호작용하는 유저의 컨트롤러를 담아둠
                boNPC.actor = playerController;

                // 상호작용 실시
                boNPC.actor.isPlayerAction = true;

                OnDialogue();

                #region NPC 대화 퀘스트 작업 처리!

                // 유저의 진행중인 퀘스트 데이터
                var boProgressQuests = GameManager.User.boQuest.progressQuests;

                // 대화 퀘스트가 들어있는 인덱스 값
                var conversationQuest = -1;

                // 진행중인 퀘스트 목록에 NPC 대화 퀘스트가 존재하는지 확인하는 작업
                for (int i = 0; i < boProgressQuests.Count; i++)
                {
                    // 존재한다면
                    if (boProgressQuests[i].sdQuest.questType == Define.QuestType.Conversation)
                    {
                        // 위치 값을 담아둠
                        conversationQuest = i;
                        break;
                    }
                }

                if (conversationQuest != -1)
                {
                    // 진행중인 대화 퀘스트의 대상이 현재 NPC인지 확인하는 작업
                    for (int i = 0; i < boProgressQuests[conversationQuest].sdQuest.target.Length; i++)
                    {
                        // 퀘스트 대상 목록에 현재 NPC가 포함 되어있다면
                        if (boProgressQuests[conversationQuest].sdQuest.target[i] == boNPC.sdNPC.index)
                        {
                            // 퀘스트 완료 조건 값을 세팅
                            boProgressQuests[conversationQuest].details[i] = boNPC.sdNPC.index;

                            var dummyServer = DummyServer.Instance;
                            dummyServer.userData.dtoQuest.progressQuests[conversationQuest].details = boProgressQuests[conversationQuest].sdQuest.questDetail;
                            dummyServer.Save();
                        }
                    }
                }

                #endregion
            }
            // 상점 창 닫기
            else if (Input.GetButtonDown(Define.Input.Interaction) && playerController.isPlayerAction)
            {
                var uiWindowManager = UIWindowManager.Instance;

                if (uiWindowManager.GetWindow<UIStore>().isOpen)
                {
                    return;
                }
                else
                {
                    uiWindowManager.GetWindow<UIDialogue>().Close();

                    // 상호작용 종료
                    boNPC.actor.isPlayerAction = false;
                    boNPC.actor = null;
                }
            }
        }

        /// <summary>
        /// 다이얼로그 창을 활성화 하는 메서드
        /// </summary>
        private void OnDialogue()
        {
            var uiDialogue = UIWindowManager.Instance.GetWindow<UIDialogue>();

            #region 퀘스트 인덱스 걸러내기 작업

            var boQuests = GameManager.User.boQuest;
            var sdQuests = GameManager.SD.sdQuests;

            // NPC가 지닌 퀘스트 목록에서 유저가 이미 완료한 퀘스트를 지움
            var canOrderQuest = boNPC.sdNPC.questIndex.Except(boQuests.completedQuests.Select(obj => obj.index));

            // 남은 데이터를 리스트로 저장
            var resultQusts = canOrderQuest.ToList();

            // 남은 퀘스트 목록에서 선행 퀘스트를 지닌 퀘스트가 있는지 확인하는 작업
            for (int i = 0; i < resultQusts.Count(); i++)
            {
                if (resultQusts[0] == 0)
                {
                    // 0번째 퀘스트 인덱스가 0이라면 퀘스트가 없다는 의미
                    continue;
                }

                // 퀘스트 기획 데이터 목록에서 남은 퀘스트 목록의 i번째 인덱스와 같은 기획 데이터를 가져와
                // 가져온 기획 데이터의 선행 퀘스트 목록을 가져옴
                var antencedentIndex = sdQuests.Where(obj => obj.index == resultQusts[i])?.SingleOrDefault().antecedentQuest;

                if (antencedentIndex[0] == 0)
                {
                    // 0번째 선행 퀘스트 인덱스가 0이라면 선행 퀘스트가 없다는 의미
                    continue;
                }

                // 선행 퀘스트 목록의 길이와 가져온 선행 퀘스트 목록과 클리어한 퀘스트 목록의 교집합 개수가 같지 않다면
                if (antencedentIndex.Length !=
                    antencedentIndex.Intersect(boQuests.completedQuests.Select(obj => obj.index)).Count())
                {
                    // 선행 퀘스트를 완료하지 못했으므로, 남은 퀘스트 목록에서 제거 
                    resultQusts.RemoveAt(i);
                    i--;
                }
            }

            // 최종적으로 남은 퀘스트 목록을 BoNPC에 저장
            boNPC.quests = resultQusts.ToArray();

            #endregion

            var randIndex = Random.Range(0, boNPC.sdNPC.baseSpeech.Length);
            var speechIndex = boNPC.sdNPC.baseSpeech[randIndex];

            // 대화 기획 데이터에서 NPC의 기본 대화 대사로 사용할 기획 데이터를 가져옴
            var sdNovel = GameManager.SD.sdNovels.Where(obj => obj.index == speechIndex)?.SingleOrDefault();
            var boNovel = new BoNovel(sdNovel);

            uiDialogue.Initialize(boNovel, boNPC);
        }
    }
}