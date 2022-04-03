using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{

    /// <summary>
    /// 인 게임 로직에서 사용할 PrevStat 데이터
    /// </summary>
    [Serializable]
    public class BoPrevStat
    {
        public float prevHp;        // 변신 전 체력 값
        public float prevEnergy;    // 변신 전 에너지 값

        public BoPrevStat(float prevHp, float prevEnergy)
        {
            this.prevHp = prevHp;
            this.prevEnergy = prevEnergy;
        }
    }
}
