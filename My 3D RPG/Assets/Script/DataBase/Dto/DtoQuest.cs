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
    /// 서버와 통신에 사용될 Quest 데이터
    /// </summary>
    [Serializable]
    public class DtoQuest : DtoBase
    {
        public DtoQuestProgress[] progressQuests;     // 현재 진행중인 퀘스트 목록
        public int[] completeQuests;                  // 완료한 퀘스트들의 인덱스 목록
    }

    /// <summary>
    /// 진행중인 퀘스트의 정보를 필드로 지닌 클래스
    /// </summary>
    [Serializable]
    public class DtoQuestProgress : DtoBase
    {
        public int index;               // 진행중인 퀘스트의 인덱스
        public int[] details;           // 진행중인 퀘스트의 정보 Ex: 처치 해야할 남은 몬스터 개수
    }
}