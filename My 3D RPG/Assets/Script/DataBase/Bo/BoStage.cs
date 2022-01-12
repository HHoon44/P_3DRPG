using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.DB
{
    /// <summary>
    /// => 클라이언트 내에서 사용할 Stage 데이터
    /// => 작업과정에서 데이터를 확인하기 위해서 Serializable
    /// </summary>
    [Serializable]
    public class BoStage
    {
        public int prevStageIndex;      // -> 다른 스테이지로 이동 시 이전 스테이지에 대한 인덱스를 받을 필드
        public Vector3 prevPos;         // -> 플레이어가 마지막으로 위치한 좌표
        public SDStage sdStage;         // -> 플레이어가 마지막으로 위치한 스테이지의 기획 데이터

        public BoStage(DtoStage dtoStage)
        {
            sdStage = GameManager.SD.sdStages.Where(obj => obj.index == dtoStage.lastStageIndex)?.SingleOrDefault();
            prevPos = new Vector3(dtoStage.lastPosX, dtoStage.lastPosY, dtoStage.lastPosZ); 
        }
    }
}