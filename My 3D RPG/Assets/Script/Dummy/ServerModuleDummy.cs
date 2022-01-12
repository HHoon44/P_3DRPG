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
    /// => 인터페이스 INetworkClient를 상속 받음
    /// => 실제 더미 서버에서의 통신 프로토콜 구현부를 갖는 클래스
    /// </summary>
    public class ServerModuleDummy : INetworkClient
    {
        private DummyServer serverData;

        public ServerModuleDummy(DummyServer serverData)
        {
            this.serverData = serverData;
        }

        // -> 혼자 다 한다 
        // -> 1. 더미 서버 이므로 실제로 클라이언트에서 클라이언트의 요청을 처리하는 것과 같음
        // -> 2. 통신 요청에 대한 실패가 발생할 일이 일반적으로 없음
        // -> 3. 강제로 요청 성공 메서드를 실행 시킴
        // -> 4. DtoAccount 값을 ToJson을 통해 json 파일로 변형 후 handleSuccess에 인자로 전달
        // -> 5. 실패가 없으므로 강제로 성공 메서드를 부른다

        public void GetAccount(int uniqueId, ResponsHandler<DtoAccount> responsHandler)
        {
            // -> 더미 서버에서는 계정정보를 요청해서 어떻게 처리할지를 작성합니다!
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoAccount));
        }

        public void GetCharacter(int uniqueId, ResponsHandler<DtoCharacter> responsHandler)
        {
            // -> 더미서버에서는 캐릭터 정보를 요청해서 어떻게 처리할지를 작성합니다!
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoCharacter));
        }

        public void GetStage(int uniqueId, ResponsHandler<DtoStage> responsHandler)
        {
            // -> 더미서버에서는 스테이지 정보를 요청해서 어떻게 처리할지를 작성합니다!
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoStage));
        }

        public void GetItem(int uniqueId, ResponsHandler<DtoItem> responsHandler)
        {
            // -> 더미서버에서는 아이템 정보를 요청해서 어떻게 처리할지를 작성합니다!
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoItem));
        }

        public void GetQuest(int uniqueId, ResponsHandler<DtoQuest> responsHandler)
        {
            // -> 더미서버에서는 퀘스트 정보를 요청해서 어떻게 처리할지를 작성합니다!
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoQuest));
        }

        /// <summary>
        /// => 새로운 퀘스트를 추가할 때 사용되는 메서드
        /// </summary>
        /// <param name="uniqueId"></param>
        /// <param name="questIndex"></param>
        /// <param name="responsHandler"></param>
        public void AddQuest(int uniqueId, int questIndex, ResponsHandler<DtoQuestProgress> responsHandler)
        {
            // -> 파라미터로 받은 퀘스트 인덱스를 통해 퀘스트 기획 데이터를 가져옵니다!
            var sdQuest = GameManager.SD.sdQuests.Where(obj => obj.index == questIndex)?.SingleOrDefault();

            // -> 클라이언트에 보낼 새로운 Dto데이터를 생성합니다!
            var dtoQuestProgress = new DtoQuestProgress();
            dtoQuestProgress.index = sdQuest.index;

            // -> 퀘스트의 디테일 정보는 현재 퀘스트의 종류에 따라 달라집니다!
            switch (sdQuest.questType)
            {
                // -> 사냥 : 몇 종류의 몬스터를 몇마리 잡아라
                // -> 수집 : 몇 종류의 아이템을 몇개 가져와라
                // -> 대화 : 어떤 NPC와 대화를 해라
                case Define.QuestType.Collection:
                case Define.QuestType.Hunt:
                case Define.QuestType.Conversation:
                    // -> Dto에 퀘스트 디테일을 저장해놓기 위해 배열의 길이를 설정합니다!
                    Array.Resize(ref dtoQuestProgress.details, sdQuest.target.Length);

                    // -> 길이가 설정된 배열안의 데이터를 모두 0으로 초기화 합니다!
                    Array.ForEach(dtoQuestProgress.details, obj => obj = 0);
                    break;
            }
            
            // -> DtoProgressQuest에 현재 퀘스트를 추가 해야하기 때문에 
            //    현재 진행중인 퀘스트 목록의 길이에 저장할 공간을 추가 합니다!
            var length = serverData.userData.dtoQuest.progressQuests.Length + 1;

            // -> 퀘스트 데이터를 저장하기 위해서 공간을 늘려 DtoQuestProgress의 길이를 재설정 합니다!
            Array.Resize(ref serverData.userData.dtoQuest.progressQuests, length);

            // -> length - 1번째에 추가할 진행중인 퀘스트를 넣어줍니다!
            serverData.userData.dtoQuest.progressQuests[length - 1] = dtoQuestProgress;

            // -> DB 데이터에 변화를 주었으니 저장 합니다!
            DummyServer.Instance.Save();

            // -> 추가된 진행중인 퀘스트 목록을 보내줍니다!
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoQuest.progressQuests[length - 1]));
        }
    }
}