using ProjectChan.NetWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// 서버와 통신에 사용될 Character 데이터
    /// </summary>
    [Serializable]
    public class DtoCharacter : DtoBase
    {
        public int index;       // 캐릭터 인덱스 번호
        public int level;       // 레벨
    }
}