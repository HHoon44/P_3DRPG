using ProjectChan.Object;
using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectChan.UI
{
    public class MonHpBar : MonoBehaviour, IPoolableObject
    {
        public bool CanRecycle { get; set; } = true;

        private Actor target;       // -> 따라다닐 대상
        private Image hpGauge;      // -> 체력바에 존재하는 이미지 컴포넌트

        /// <summary>
        /// => 따라 다닐 대상을 가져오는 작업과 월드 캔버스 기준으로 스케일링
        ///    되어 있는 MonHpBar의 스케일값을 기존 스케일값으로 변경
        /// </summary>
        /// <param name="target"> 따라다닐 대상 </param>
        public void Initialize(Actor target)
        {
            hpGauge = GetComponent<Image>();

            this.target = target;

            hpGauge.fillAmount = 1f;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// => 몬스터의 Hp상태를 업데이트 해주는 메서드
        /// </summary>
        public void MonHpBarUpdate()
        {
            // -> 대상이 없거나 대상의 콜라이더 존재하지 않는다면
            if (target == null || target.Coll == null)
            {
                return;
            }

            // -> 대상이 죽은 상태라면
            if (target.State == Define.Actor.ActorState.Dead)
            {
                // -> 들어갈때 Stat이 Dead상태로 풀에 돌아가면 재사용할때 바로 죽음
                ObjectPoolManager.Instance.GetPool<MonHpBar>(Define.PoolType.MonHpBar).ReturnPoolableObject(this);
                //target.State = Define.Actor.ActorState.None;
                target = null;
                return;
            }

            // -> Canvas의 Pivot에 맞춰져서 설정되기 때문에 별도로 설정해줌
            transform.position = target.transform.position + Vector3.up * (target.Coll.bounds.size.y * 1.3f);
            hpGauge.fillAmount = target.boActor.currentHp / target.boActor.maxHp;
        }
    }
}