using ProjectChan.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.SD
{
    /// <summary>
    /// => 퀘스트 SD 데이터
    /// </summary>
    [Serializable]
    public class SDQuest : StaticData
    {
        public string name;                 // -> 퀘스트 이름
        public QuestType questType;         // -> 퀘스트 타입
        public int description;             // -> 퀘스트 대사 기획 데이터에 존재하는 퀘스트 내용에 대한 인덱스 값
        public int[] antecedentQuest;       // -> 선행 퀘스트 EX: 선행 퀘스트를 필요로하는 퀘스트들이 존재할 수 도있음
        public int[] novelIndex;            // -> 퀘스트 대사 기획 데이터에 존재하는 퀘스트 대사에 대한 인덱스 값
        public int[] target;                // -> 처지해야하는 대상 or 대화해야하는 대상
        public int[] questDetail;           // -> 남은 몬스터 개수 or 남은 대화 대상
        public int[] compensation;          // -> 보상 아이템 인덱스
        public int[] compensationDetail;    // -> 보상 아이템 개수
    }
}