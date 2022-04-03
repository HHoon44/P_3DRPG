using ProjectChan.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// 인 게임 로직에서 사용할 Account 데이터
    /// </summary>
#if UNITY_EDITOR
    [Serializable]
#endif
    public class BoAccount
    {
        public string nickName;     // Dto에 저장되어있는 플레이어 이름
        public int gold;            // Dto에 저장되어있는 플레이어 골드

        /// <summary>
        /// 서버에서 보내준 Dto 데이터를 Bo 데이터로 변환
        /// </summary>
        /// <param name="dtoAccount"> 서버에서 보내준 Account 데이터 </param>
        public BoAccount(DtoAccount dtoAccount)
        {
            nickName = dtoAccount.nickName;
            gold = dtoAccount.gold;
        }
    }
}