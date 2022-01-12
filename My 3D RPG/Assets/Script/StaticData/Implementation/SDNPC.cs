using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectChan.Define.Actor;

namespace ProjectChan.SD
{
    /// <summary>
    /// => NPC 기본 세팅에 사용할 기획 데이터
    /// </summary>
    [Serializable]
    public class SDNPC : StaticData
    {
        public string name;             // -> NPC 이름
        public string resourcePath;     // -> NPC 저장 경로
        public NPCType npcType;         // -> NPC 타입
        public int stageIndex;          // -> NPC 스테이지 인덱스
        public int[] baseSpeech;        // -> NPC 대화 인덱스
        public int[] needQuestIndex;    // -> NPC가 특정 퀘스트에서만 생성된다면 특정 퀘스트 인덱스 값
        public int[] questIndex;        // -> NPC가 지닌 퀘스트 인덱스
        public int[] storeItem;         // -> 상점NPC가 지닌 아이템 인덱스
        public float[] npcPos;          // -> NPC Position 값
        public float[] npcRot;          // -> NPC Rotation 값
    }
}