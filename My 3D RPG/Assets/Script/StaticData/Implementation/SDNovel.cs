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
    /// => 노벨 SD데이터
    /// </summary>
    [Serializable]
    public class SDNovel : StaticData
    {
        public string name;             // -> 캐릭터 이름
        public string kr;               // -> 캐릭터 대사
        public string stagePath;        // -> 배경에 넣을 이미지 이름
        public string portraitPath;     // -> 캐릭터 초상화 이름
        public CharType charType;       // -> 캐릭터 타입
        public AtlasType atlasType;     // -> 초상화 이미지를 불러올때 사용할 아틀라스 타입
        public int portraitIndex;       // -> 현재 캐릭터와 일치한 초상화 인덱스
    }
}