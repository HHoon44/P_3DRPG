using ProjectChan.Object;
using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// 인 게임 로직에서 사용할 NPC 데이터
    /// </summary>
    [Serializable]
    public class BoNPC
    {
        public SDNPC sdNPC;             // NPC 기획데이터
        public PlayerController actor;  // 현재 NPC와 상호작용중인 유저
        public int[] quests;            // NPC가 지닌 퀘스트 인덱스 목록

        public BoNPC(SDNPC sdNPC)
        {
            this.sdNPC = sdNPC;
        }
    }
}