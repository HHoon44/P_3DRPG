using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.SD
{
    /// <summary>
    /// => 기본 캐릭터의 스텟의 SD데이터
    /// </summary>
    [Serializable]
    public class SDOriginInfo : StaticData
    {
        public int weaponIndex;         // -> 변신폼의 무기 인덱스
        public float maxHp;             // -> 변신폼의 체력
        public float maxHpFactor;       // -> 변신폼의 실제 체력
        public float maxMana;           // -> 변신폼의 마나
        public float maxManaFactor;     // -> 변신폼의 실제 마나
        public float atk;               // -> 변신폼의 공격력
        public float atkFactor;         // -> 변신폼의 실제 공격력
        public float def;               // -> 변신폼의 방어력
        public float defFactor;         // -> 변신폼의 실제 방어력
        public float behaviour;         // -> ?
        public float behaviourFactor;   // -> ?
    }
}