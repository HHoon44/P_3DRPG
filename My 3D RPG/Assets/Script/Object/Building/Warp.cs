using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.Object
{
    /// <summary>
    /// 스테이지를 넘어가는 포탈 클래스
    /// </summary>
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

            // 스테이지 이동을 하므로, 이전 스테이지의 Index를 Bo에 저장한다
            user.boStage.prevStageIndex = user.boStage.sdStage.index;

            // 이동할 스테이지의 기획 데이터를 가져온다
            user.boStage.sdStage = GameManager.SD.sdStages.Where(obj => obj.index == warpStageIndex)?.SingleOrDefault();

            var stageManager = StageManager.Instance;
            
            /*
             *  LoadScene과 비슷한 역할을 합니다
             *  실제 씬을 변경하지는 않고 유저에게 씬을 변경하는 것처럼 로딩 씬을 추가해서 
             *  잠시 메인화면을 로딩 씬으로 보여준 후 이전 스테이지에서 사용하던 리소스를 해제한 후
             *  현재 스테이지 리소르를 불어온 후 필요한 객체를 인스턴스 하는 작업 입니다!
             */
            GameManager.Instance.OnAddtiveLoadingScene(stageManager.ChangeStage(), stageManager.OnChangeStageComplete);
        }
    }
}