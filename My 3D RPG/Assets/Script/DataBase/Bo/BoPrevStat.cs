using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// 변신폼 전에 현재 hp랑 현재 mana를 담아 놓을 공간
    public class BoPrevStat
    {
        public float currentHp;
        public float currentMana;

        public BoPrevStat(float currenHp, float currenMana)
        {
            this.currentHp = currenHp;
            this.currentMana = currenMana;
        }
    }
}
