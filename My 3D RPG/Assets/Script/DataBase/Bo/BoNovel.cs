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
    public class BoNovel
    {
        public SDNovel sdNovel;

        public BoNovel(SDNovel sdNovel)
        {
            this.sdNovel = sdNovel;
        }
    }
}