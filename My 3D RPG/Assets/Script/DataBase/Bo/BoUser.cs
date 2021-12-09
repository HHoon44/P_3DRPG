using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectChan.Define.Actor;

namespace ProjectChan.DB
{
    /// <summary>
    /// => 유저가 지닌 Bo데이터들
    /// </summary>
    [Serializable]
    public class BoUser
    {
        public BoAccount boAccount;         // -> 유저의 계정 정보
        public BoCharacter boCharacter;     // -> 유저의 캐릭터 정보
        public BoStage boStage;             // -> 유저의 스테이지 정보
        public List<BoItem> boItems;        // -> 유저가 지닌 아이템 정보
        public BoQuest boQuest;             // -> 유저가 지닌 퀘스트 정보
        public Dictionary<ActorType, BoPrevStat> boPrevStatDic = new Dictionary<ActorType, BoPrevStat>();
    }
}