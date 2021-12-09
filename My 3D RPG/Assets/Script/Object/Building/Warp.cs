using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.Object
{
    public class Warp : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            {
                return;
            }

            var warpStageIndex = int.Parse(transform.parent.name);
            var user = GameManager.User;

            user.boStage.prevStageIndex = user.boStage.sdStage.index;

            user.boStage.sdStage = GameManager.SD.sdStages.Where(obj => obj.index == warpStageIndex)?.SingleOrDefault();

            var stageManager = StageManager.Instance;
            
            GameManager.Instance.OnAddtiveLoadingScene(stageManager.ChangeStage(), stageManager.OnChangeStageComplete);
        }
    }
}