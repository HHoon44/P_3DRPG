using ProjectChan.NetWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// => 서버와 통신에 사용될 Stage 데이터
    /// => 작업 과정에서 데이터를 확인하기 위해서 Serializable
    /// </summary>
    [Serializable]
    public class DtoStage : DtoBase
    {
        public float lastPosX;          // -> 플레이어가 마지막으로 위치한 X좌표
        public float lastPosY;          // -> 플레이어가 마지막으로 위치한 Y좌표
        public float lastPosZ;          // -> 플레이어가 마지막으로 위치한 Z좌표
        public int lastStageIndex;      // -> 마지막으로 존재했던 스테이지의 인덱스
    }
}