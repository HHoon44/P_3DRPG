using ProjectChan.DB;
using ProjectChan.NetWork;
using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.Dummy
{
    /// <summary>
    /// 실제 더미 서버에서의 통신 프로토콜 구현부를 갖는 클래스
    /// </summary>
    public class ServerModuleDummy : INetworkClient
    {
        private DummyServer serverData;

        public ServerModuleDummy(DummyServer serverData)
        {
            this.serverData = serverData;
        }

        /*
         *  혼자 다 한다
         *  1. 더미 서버 이므로 실제로 클라이언트에서 클라이언트의 요청을 처리하는 것과 같음
         *  2. 통신 요청에 대한 실패가 발생할 일이 일반적으로 없음
         *  3. 강제로 요청 성공 메서드를 실행 시킴
         *  4. DtoAcocunt 값을 ToJson을 통해 Json 파일로 변형 후 HandlerSuccess에 인자로 전달
         *  5. 실패가 없으므로 강제로 성공 메서드를 부른다
         */

        public void GetAccount(int uniqueId, ResponsHandler<DtoAccount> responsHandler)
        {
            // 더미 서버에서는 계정 정보를 요청해서 어떻게 처리할지
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoAccount));
        }

        public void GetCharacter(int uniqueId, ResponsHandler<DtoCharacter> responsHandler)
        {
            // 더미서버에서는 캐릭터 정보를 요청해서 어떻게 처리할지
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoCharacter));
        }

        public void GetStage(int uniqueId, ResponsHandler<DtoStage> responsHandler)
        {
            // 더미서버에서는 스테이지 정보를 요청해서 어떻게 처리할지
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoStage));
        }

        public void GetItem(int uniqueId, ResponsHandler<DtoItem> responsHandler)
        {
            // 더미서버에서는 아이템 정보를 요청해서 어떻게 처리할지
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoItem));
        }

        public void GetQuest(int uniqueId, ResponsHandler<DtoQuest> responsHandler)
        {
            // 더미서버에서는 퀘스트 정보를 요청해서 어떻게 처리할지
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoQuest));
        }

        /// <summary>
        /// 새로운 퀘스트를 DB에 추가하는 메서드
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="questIndex"></param>
        /// <param name="responsHandler"></param>
        public void AddQuest(int uniqueId, int questIndex, ResponsHandler<DtoQuestProgress> responsHandler)
        {
            // 전달 받은 퀘스트 인덱스를 통해 퀘스트 기획 데이터를 가져옴
            var sdQuest = GameManager.SD.sdQuests.Where(obj => obj.index == questIndex)?.SingleOrDefault();

            // 클라이언트에 보낼 새로운 DtoQuest 생성
            var dtoQuestProgress = new DtoQuestProgress();
            dtoQuestProgress.index = sdQuest.index;

            // 퀘스트의 디테일 정보는 현재 퀘스트의 종류에 따라 달라짐
            switch (sdQuest.questType)
            {
                /*
                 *  사냥 : 몇 종류의 몬스터를 몇마리 잡아와라!
                 *  수집 : 몇 종류의 아이템을 몇개정도 가져와라!
                 *  대화 : 특정 NPC와 대화를 해라!
                 */

                case Define.QuestType.Collection:
                case Define.QuestType.Hunt:
                case Define.QuestType.Conversation:
                    // 진행중인 퀘스트의 디테일을 Dto에 저장하기 위해 배열의 길이를 재설정
                    Array.Resize(ref dtoQuestProgress.details, sdQuest.target.Length);
                    Array.ForEach(dtoQuestProgress.details, obj => obj = 0);
                    break;
            }

            // 진행중인 퀘스트를 추가 해야 하므로 +1
            var length = serverData.userData.dtoQuest.progressQuests.Length + 1;

            // 진행중인 퀘스트를 추가 해야 하므로 배열의 길이 재설정
            Array.Resize(ref serverData.userData.dtoQuest.progressQuests, length);

            // 현재 진행중인 퀘스트를 Dto에 저장
            serverData.userData.dtoQuest.progressQuests[length - 1] = dtoQuestProgress;

            DummyServer.Instance.Save();

            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoQuest.progressQuests[length - 1]));
        }
    }
}