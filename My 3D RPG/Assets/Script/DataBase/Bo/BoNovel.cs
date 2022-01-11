using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectChan.Define.Actor;
using static ProjectChan.Define.Resource;

namespace ProjectChan
{
    /// <summary>
    /// => 클라이언트 내에서 사용할 Novel 데이터
    /// => 작업과정에서 데이터를 확인하기 위해서 Serializable
    /// </summary>
    [Serializable]
    public class BoNovel
    {
        public SDNovel sdNovel;     // -> 사용할 노벨 기획 데이터

        public BoNovel(SDNovel sdNovel)
        {
            this.sdNovel = sdNovel;
        }
    }
}