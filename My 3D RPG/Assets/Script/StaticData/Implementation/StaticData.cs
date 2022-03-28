using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.SD
{
    /// <summary>
    /// 기획 데이터들의 인덱스
    /// 인덱스는 해당 기획 테이블 내에서 유니크
    /// </summary>
    [Serializable]
    public class StaticData
    {
        public int index;
    }
}