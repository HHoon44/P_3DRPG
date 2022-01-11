using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{

    /// <summary>
    /// => 클라이언트 내에서 사용할 PrevStat 데이터
    /// => 작업과정에서 데이터를 확인하기 위해서 Serializable
    /// </summary>
    [Serializable]
    public class BoPrevStat
    {
        public float prevHp;     // -> 변신 전 체력 값
        public float prevEnergy;   // -> 변신 전 에너지 값

        public BoPrevStat(float prevHp, float prevEnergy)
        {
            this.prevHp = prevHp;
            this.prevEnergy = prevEnergy;
        }
    }
}
