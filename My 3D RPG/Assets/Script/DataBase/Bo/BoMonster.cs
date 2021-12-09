using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    public class BoMonster : BoActor
    {
        public SDMonster sdMonster;     // -> BoMonster가 지닐 SD값

        public BoMonster(SDMonster sdMonster)
        {
            this.sdMonster = sdMonster;
        }
    }
}       