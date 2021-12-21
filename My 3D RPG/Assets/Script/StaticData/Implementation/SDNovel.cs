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
    /// => 노벨 게임에서 사용할 기획 데이터
    /// </summary>
    [Serializable]
    public class SDNovel : StaticData
    {
        public string name;             // -> 캐릭터 이름
        public string kr;               // -> 캐릭터 대사
        public string stagePath;        // -> 배경 이름
        public string portraitPath;     // -> 캐릭터 초상화 이름
        public CharType charType;       // -> 캐릭터 타입
    }
}