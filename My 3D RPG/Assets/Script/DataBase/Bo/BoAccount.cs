using ProjectChan.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// => 클라이언트 내에서 사용할 Account 데이터
    /// => 작업과정에서 데이터를 확인하기 위해서 Serializable
    /// </summary>
#if UNITY_EDITOR
    [Serializable]
#endif
    public class BoAccount
    {
        public string nickName;     // -> Dto에 저장되어있는 플레이어 이름
        public int gold;            // -> Dto에 저장되어있는 플레이어 골드

        /// <summary>
        /// => 서버를 통해 받은 DtoAccount 데이터를 BoAccount데이터로 변환해주는 메서드
        /// </summary>
        /// <param name="dtoAccount"> 서버를 통해 받은 DtoAccount 데이턴 </param>
        public BoAccount(DtoAccount dtoAccount)
        {
            this.nickName = dtoAccount.nickName;
            this.gold = dtoAccount.gold;
        }
    }
}