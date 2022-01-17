using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.SD
{
    /// <summary>
    /// => 기존 캐릭터 세팅에 사용할 기획 데이터
    /// </summary>
    [Serializable]
    public class SDOriginInfo : StaticData
    {
        public int weaponIndex;         // -> 변신폼의 무기 인덱스
        public float maxHp;             // -> 변신폼의 체력
        public float maxHpFactor;       // -> 변신폼의 실제 체력
        public float maxEnergy;         // -> 변신폼의 마나
        public float maxEnergyFactor;   // -> 변신폼의 실제 마나
        public float atk;               // -> 변신폼의 공격력
        public float atkFactor;         // -> 변신폼의 실제 공격력
        public float def;               // -> 변신폼의 방어력
        public float defFactor;         // -> 변신폼의 실제 방어력
    }
}