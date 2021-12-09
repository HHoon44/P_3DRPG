using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.DB
{
    [Serializable]
    public class BoStage
    {
        public int prevStageIndex;
        public Vector3 prevPos;
        public SDStage sdStage;

        public BoStage(DtoStage dtoStage)
        {
            sdStage = GameManager.SD.sdStages.Where(obj => obj.index == dtoStage.lastStageIndex)?.SingleOrDefault();
            prevPos = new Vector3(dtoStage.lastPosX, dtoStage.lastPosY, dtoStage.lastPosZ); 
        }
    }
}