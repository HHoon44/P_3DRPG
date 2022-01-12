using ProjectChan.NetWork;
using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// => 서버와 통신에 사용될 Quest 데이터
    /// => 작업 과정에서 확인하기 위해서 Serializable
    /// </summary>
    [Serializable]
    public class DtoQuest : DtoBase
    {
        public DtoQuestProgress[] progressQuests;     // -> 현재 진행중인 퀘스트의 데이터
        public int[] completeQuests;                  // -> 완료한 퀘스트들의 인덱스 데이터
    }


    /// <summary>
    /// => 현재 진행중인 Quest를 담아놓을 데이터
    /// => 작업 과정에서 확인하기 위해서 Serializable
    /// </summary>
    [Serializable]
    public class DtoQuestProgress : DtoBase
    {
        public int index;               // -> 진행중인 퀘스트의 인덱스
        public int[] details;           // -> 진행중인 퀘스트의 정보 Ex: 처치 해야할 남은 몬스터 개수
    }
}