using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectChan.Define.Actor;

/*
 *  일반적으로 서버에서 받은 Dto 데이터를 Bo 데이터로 변환하여 사용
 *  Dto 데이터를 직접적으로 인게임 로직에서 사용할 일은 없음
 *  Bo 데이터는 인게임 로직에서만 사용되고 통신을 하지 않으므로 직렬화할 필요가 없음
 *  하지만 작업과정에서 데이터를 확인하기 위해 최종적인 빌드 전에는 직렬화하여 데이터를 인스펙터에 노출
 */

/*
 *  Bo 데이터는 통신으로 받은 Dto 데이터( 서버 데이터 ), 기획에서 정의한 데이터, 
 *  인게임에서만 사용하는 임시 데이터이다( 휘발성 )
 */

namespace ProjectChan.DB
{
    /// <summary>
    /// 유저의 모든 Bo 데이터를 포함하는 데이터 셋 클래스
    /// </summary>
    [Serializable]
    public class BoUser
    {
        // public 
        public BoAccount boAccount;         // 유저의 계정 정보
        public BoCharacter boCharacter;     // 유저의 캐릭터 정보
        public BoStage boStage;             // 유저의 스테이지 정보
        public BoQuest boQuest;             // 유저가 지닌 퀘스트 정보

        /// <summary>
        /// 유저의 아이템 정보
        /// </summary>
        public List<BoItem> boItems;

        /// <summary>
        /// 변신 전 플레이어의 Hp, Energy의 정보
        /// </summary>
        public Dictionary<ActorType, BoPrevStat> boPrevStatDic = new Dictionary<ActorType, BoPrevStat>();
    }
}