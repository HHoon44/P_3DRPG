using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectChan.Define.Actor;
using static ProjectChan.Define.Resource;

namespace ProjectChan.SD
{
    /// <summary>
    /// => 퀘스트 대사 기획 데이터
    /// </summary>
    [Serializable]
    public class SDQuestSpeech : StaticData
    {
        public string kr;               // -> 퀘스트 대사
    }
}
