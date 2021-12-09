using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// => 클라이언트 내에서 사용할 Quest 데이터
    /// => 작업과정에서 데이터를 확인하기 위해서 직렬화
    /// </summary>
    [Serializable]
    public class BoQuest 
    {
        public List<BoQuestProgress> progressQuests;    // -> dto에 저장되어있던 현재 진행중인 퀘스트 데이터
        public List<SDQuest> completedQuests;           // -> dto에 저장되어있던 완료한 퀘스트 데이터

        public BoQuest(DtoQuest dtoQuest)
        {
            progressQuests = new List<BoQuestProgress>();
            completedQuests = new List<SDQuest>();

            // -> Dto에 저장되어있던 현재 진행중인 퀘스트 데이터를 Bo에 저장
            for (int i = 0; i < dtoQuest.progressQuests.Length; i++)
            {
                progressQuests.Add(new BoQuestProgress(dtoQuest.progressQuests[i]));
            }

            // -> Dto에 저장되어있던 이미 완료한 퀘스트 데이터를 Bo에 저장
            for (int i = 0; i < dtoQuest.completeQuests.Length; i++)
            {
                completedQuests.Add(GameManager.SD.sdQuests.Where
                    (obj => obj.index == dtoQuest.completeQuests[i])?.SingleOrDefault());
            }
        }
    }

    /// <summary>
    /// => 클라이언트 내에서 현재 진행중인 Quest의 정보를 지닌 데이터
    /// </summary>
    [Serializable]
    public class BoQuestProgress
    {
        public int[] details;
        public SDQuest sdQuest;

        public BoQuestProgress(DtoQuestProgress dtoQuestProgress)
        {
            details = (int[])dtoQuestProgress.details.Clone();
            sdQuest = GameManager.SD.sdQuests.Where(obj => obj.index == dtoQuestProgress.index)?.SingleOrDefault();
        }
    }
}
