using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// 인 게임 로직에서 사용할 Monster 데이터
    /// </summary>
    [Serializable]
    public class BoMonster : BoActor
    {
        public SDMonster sdMonster;     // 몬스터 기획 데이터

        public BoMonster(SDMonster sdMonster)
        {
            this.sdMonster = sdMonster;
        }
    }
}       