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
    /// <summary>
    /// => 몬스터가 지닐 체력바 클래스
    /// </summary>
    public class MonHpBar : MonoBehaviour, IPoolableObject
    {
        public Image hpGauge;       // -> 체력바에 존재하는 이미지 컴포넌트

        private Actor target;       // -> 따라다닐 대상

        /// <summary>
        /// => 재사용이 가능한지
        /// </summary>
        public bool CanRecycle { get; set; } = true;

        /// <summary>
        /// => 따라다닐 대상을 설정하고 월드 캔버스 기준으로 스케일링 되어 있으므로
        ///    기존 스케일 값으로 변경하는 메서드
        /// </summary>
        /// <param name="target"> 따라다닐 대상 </param>
        public void Initialize(Actor target)
        {
            this.target = target;

            hpGauge.fillAmount = 1f;
            hpGauge.type = Image.Type.Filled;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// => 몬스터의 Hp상태를 업데이트 해주는 메서드
        /// </summary>
        public void MonHpBarUpdate()
        {
            // -> 대상이 없거나 대상의 콜라이더 존재하지 않는다면!
            if (target == null || target.Coll == null)
            {
                return;
            }

            // -> 대상이 죽은 상태라면!
            if (target.State == Define.Actor.ActorState.Dead)
            {
                ObjectPoolManager.Instance.GetPool<MonHpBar>(Define.PoolType.MonHpBar).ReturnPoolableObject(this);
                target = null;
                return;
            }

            // -> 캔버스의 피봇에 맞춰져서 설정되기 때문에 별도로 설정 해줍니다!
            transform.position = target.transform.position + Vector3.up * (target.Coll.bounds.size.y * 1.3f);
            hpGauge.fillAmount = target.boActor.currentHp / target.boActor.maxHp;
        }
    }
}