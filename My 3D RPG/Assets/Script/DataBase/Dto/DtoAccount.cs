using ProjectChan.NetWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// 서버와 통신에 사용될 Account 데이터
    /// </summary>
    [Serializable]
    public class DtoAccount : DtoBase
    {
        public string nickName;     // 이름
        public int gold;            // 보유한 금액
    }
}