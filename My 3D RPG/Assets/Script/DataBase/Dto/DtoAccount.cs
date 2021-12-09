using ProjectChan.NetWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// => 서버와 통신에 사용될 Account 데이터
    /// => 작업 과정에서 데이터를 확인하기 위해서 Serializable
    /// </summary>
    [Serializable]
    public class DtoAccount : DtoBase
    {
        public string nickName;     // -> 플레이어의 이름
        public int gold;            // -> 플레이어가 보유한 금액
    }
}