using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// 인 게임 로직에서 사용할 Quest 데이터
    /// </summary>
    [Serializable]
    public class BoQuest 
    {
        public List<BoQuestProgress> progressQuests;    // 현재 진행중인 퀘스트 데이터 목록
        public List<SDQuest> completedQuests;           // 완료한 퀘스트 데이터 목록

        /// <summary>
        /// 서버에서 보내준 통신 데이터( Dto )를 유저 데이터( Bo )로 변환
        /// </summary>
        /// <param name="dtoQuest"> 서버에서 보낸 통신 데이터 </param>
        public BoQuest(DtoQuest dtoQuest)
        {
            progressQuests = new List<BoQuestProgress>();
            completedQuests = new List<SDQuest>();

            for (int i = 0; i < dtoQuest.progressQuests.Length; i++)
            {
                progressQuests.Add(new BoQuestProgress(dtoQuest.progressQuests[i]));
            }

            for (int i = 0; i < dtoQuest.completeQuests.Length; i++)
            {
                completedQuests.Add(GameManager.SD.sdQuests.Where
                    (obj => obj.index == dtoQuest.completeQuests[i])?.SingleOrDefault());
            }
        }
    }

    /// <summary>
    /// 인 게임 로직에서 사용할 BoQuestProgress 데이터
    /// </summary>
    [Serializable]
    public class BoQuestProgress
    {
        public int[] details;       // 진행중인 퀘스트의 디테일 목록
        public SDQuest sdQuest;     // 진행중인 퀘스트의 기획 데이터

        public BoQuestProgress(DtoQuestProgress dtoQuestProgress)
        {
            details = (int[])dtoQuestProgress.details.Clone();
            sdQuest = GameManager.SD.sdQuests.Where(obj => obj.index == dtoQuestProgress.index)?.SingleOrDefault();
        }
    }
}
