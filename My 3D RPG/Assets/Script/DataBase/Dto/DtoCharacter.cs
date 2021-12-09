using ProjectChan.NetWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// => 서버와 통신에 사용될 Character 데이터
    /// => 작업과정에서 데이터를 확인하기 위해서 Serializable
    /// </summary>
    [Serializable]
    public class DtoCharacter : DtoBase
    {
        public int index;       // -> 플레이어의 캐릭터 인덱스 번호
        public int level;       // -> 플레이어의 레벨
    }
}