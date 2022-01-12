using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectChan.Define.Actor;

namespace ProjectChan.DB
{
    /// <summary>
    /// => 유저의 모든 Bo 데이터를 포함하는 데이터 셋
    /// </summary>
    [Serializable]
    public class BoUser
    {
        public BoAccount boAccount;         // -> 유저의 계정 정보
        public BoCharacter boCharacter;     // -> 유저의 캐릭터 정보
        public BoStage boStage;             // -> 유저의 스테이지 정보
        public BoQuest boQuest;             // -> 유저가 지닌 퀘스트 정보

        /// <summary>
        /// 아이템 정보가 담길 리스트
        /// </summary>
        public List<BoItem> boItems;

        /// <summary>
        /// => 변신 전 플레이어의 Hp, Energy가 담길 딕셔너리
        /// </summary>
        public Dictionary<ActorType, BoPrevStat> boPrevStatDic = new Dictionary<ActorType, BoPrevStat>();
    }
}