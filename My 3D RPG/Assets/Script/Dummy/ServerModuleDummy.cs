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
    public class ServerModuleDummy : INetworkClient
    {
        private DummyServer serverData;

        public ServerModuleDummy(DummyServer serverData)
        {
            this.serverData = serverData;
        }

        public void GetAccount(int uniqueId, ResponsHandler<DtoAccount> responsHandler)
        {
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoAccount));
        }

        public void GetCharacter(int uniqueId, ResponsHandler<DtoCharacter> responsHandler)
        {
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoCharacter));
        }

        public void GetStage(int uniqueId, ResponsHandler<DtoStage> responsHandler)
        {
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoStage));
        }

        public void GetItem(int uniqueId, ResponsHandler<DtoItem> responsHandler)
        {
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoItem));
        }

        public void GetQuest(int uniqueId, ResponsHandler<DtoQuest> responsHandler)
        {
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
            // -> 파라미터로 받은 퀘스트 인덱스를 통해 기획 데이터를 가져옵니다!
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
            
            // -> DtoQuest에 위에서 생성한 DtoQuestProgress에 대한 정보를 추가하기 위해서
            //    현재 DtoQuest에 배열로 존재하는 progressQuests에 공간을 추가
            var length = serverData.userData.dtoQuest.progressQuests.Length + 1;

            // -> 데이터를 저장할 공간을 늘리기 위해서 현재 progressQuests의 길이를 ReSize해준다
            Array.Resize(ref serverData.userData.dtoQuest.progressQuests, length);

            // -> length - 1번째에 위에서 생성한 DtoQuestProgress 데이터를 저장
            serverData.userData.dtoQuest.progressQuests[length - 1] = dtoQuestProgress;

            // -> DB에 데이터를 저장했으니 Save
            DummyServer.Instance.Save();

            // -> 추가된 진행 퀘스트 정보를 클라에 보내준다
            responsHandler.HandleSuccess(SerializationUtil.ToJson(serverData.userData.dtoQuest.progressQuests[length - 1]));
        }
    }
}