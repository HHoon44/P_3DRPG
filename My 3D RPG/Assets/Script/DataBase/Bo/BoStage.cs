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
    /// 인 게임 로직에서 사용할 Stage 데이터
    /// </summary>
    [Serializable]
    public class BoStage
    {
        public int prevStageIndex;      // 다른 스테이지로 이동 시, 이전 스테이지 인덱스
        public Vector3 prevPos;         // 플레이어가 마지막으로 위치한 좌표
        public SDStage sdStage;         // 플레이어가 마지막으로 위치한 스테이지의 기획 데이터

        /// <summary>
        /// 서버에서 보내준 통신 데이터( Dto )를 유저 데이터( Bo )로 변환
        /// </summary>
        /// <param name="dtoStage"> 서버에서 보낸 통신 데이터 </param>
        public BoStage(DtoStage dtoStage)
        {
            sdStage = GameManager.SD.sdStages.Where(obj => obj.index == dtoStage.lastStageIndex)?.SingleOrDefault();
            prevPos = new Vector3(dtoStage.lastPosX, dtoStage.lastPosY, dtoStage.lastPosZ); 
        }
    }
}