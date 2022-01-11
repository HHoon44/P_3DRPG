using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// => 클라이언트 내에사 사용할 Monster 데이터
    /// => 작업과정에서 데이터를 확인하기 위해서 Serializable
    /// </summary>
    [Serializable]
    public class BoMonster : BoActor
    {
        public SDMonster sdMonster;     // -> BoMonster가 지닐 SD값

        public BoMonster(SDMonster sdMonster)
        {
            this.sdMonster = sdMonster;
        }
    }
}       