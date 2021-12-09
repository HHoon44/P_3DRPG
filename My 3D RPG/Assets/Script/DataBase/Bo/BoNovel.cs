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
        public int currentPortrait;     // -> 현재 초상화 인덱스
        public string name;             // -> 현재 말하는 캐릭터 이름
        public string speeches;         // -> 현재 말하는 캐릭터의 대사 
        public string portraitPath;     // -> 현재 말하는 캐릭터의 초상화 경로
        public string stagePath;        // -> 현재 뒷배경의 경로
        public CharType charType;       // -> 현재 말하는 캐릭터의 타입
        public AtlasType atlasType;     // -> 말하는 캐릭터의 아틀라스 타입

        public BoNovel(SDNovel sdNovel)
        {
            currentPortrait = sdNovel.portraitIndex;
            speeches = sdNovel.kr;
            charType = sdNovel.charType;
            atlasType = sdNovel.atlasType;
            portraitPath = sdNovel.portraitPath;
            stagePath = sdNovel.stagePath;
            name = sdNovel.name;
        }
    }
}