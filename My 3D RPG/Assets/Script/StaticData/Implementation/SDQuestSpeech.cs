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
    /// => 퀘스트 대사들을 지닌 기획 데이터
    /// </summary>
    [Serializable]
    public class SDQuestSpeech : StaticData
    {
        public string kr;               // -> 퀘스트 대사
        ///public string portraitPath;     // -> 해당 캐릭터 초상화 경로
        ///public CharType charType;       // -> 해당 캐릭터의 타입
        ///public AtlasType atlasType;     // -> 해당 캐릭터의 아틀라스 타입
        ///public int portraitIndex;       // -> 초상화 인덱스
    }
}
