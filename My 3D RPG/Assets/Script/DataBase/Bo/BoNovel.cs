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
    /// 인 게임 로직에서 사용할 Novel 데이터
    /// </summary>
    [Serializable]
    public class BoNovel
    {
        public SDNovel sdNovel;     // 노벨 기획 데이터

        public BoNovel(SDNovel sdNovel)
        {
            this.sdNovel = sdNovel;
        }
    }
}