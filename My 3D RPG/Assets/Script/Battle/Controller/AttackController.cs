using ProjectChan.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static ProjectChan.Define.Actor;

namespace ProjectChan.Battle
{
    /// <summary>
    /// => 공격을 관리할 클래스
    /// </summary>
    public class AttackController : MonoBehaviour
    {
        public bool hasTarget;                              // -> 공격 대상이 있는지?
        public bool canCheckCoolTime;                       // -> 공격 쿨타임을 체크할 수있는지? ( 공격 모션이 끝나기 전에는 쿨타임 체크를 막는다 )
        public bool isCoolTime;                             // -> 공격 쿨타임인지?
        public bool canAtk;                                 // -> 공격 가능 상태인지?

        private float currentAtkInterval;                   // -> 현재 공격 쿨타임을 체크하는 값
        private Actor attacker;                             // -> 공격자 ( 해당 어택 컨트롤러 인스턴스를 갖는 액터 )

        /// <summary>
        /// => 현재 타겟을 넣어 놓을 리스트
        /// </summary>
        private List<Actor> targets = new List<Actor>();

        public void Initialize(Actor attacker)
        {
            this.attacker = attacker;
        }

        /// <summary>
        /// => 공격 사능 상태일 때 공격자의 상태를 변경 하는 메서드
        /// </summary>
        public void CheckAttack()
        {
            // -> 타겟이 없다면!
            if (!hasTarget)
            {
                return;
            }

            // -> 공격 쿨타임이라면!
            if (isCoolTime)
            {
                return;
            }

            // -> 공격 불가능이라면!
            if (!canAtk)
            {
                return;
            }

            // -> 공격을 합니다!
            attacker.SetState(ActorState.Attack);
        }

        /// <summary>
        /// => 애니메이션 이벤트로 사용되며 공격을 실행할 때 사용될 메서드
        /// </summary>
        public virtual void OnAttack()
        {
            // -> 현재 공격자의 공격 타입에 따라 처리합니다!
            switch (attacker.boActor.atkType)
            {
                case AttackType.Normal:
                case AttackType.Boss:
                    CalculateAttackRange();

                    var damage = attacker.boActor.atk;

                    for (int i = 0; i < targets.Count; i++)
                    {
                        CalculateDamage(damage, targets[i]);
                    }
                    break;
            }
        }

        /// <summary>
        /// => 공격 범위에 적이 존재하는지 확인하는 메서드
        /// </summary>
        public virtual void CalculateAttackRange()
        {
            // -> 오브젝트의 레이어와 공격자의 액터 타입을 이용하여 타겟이 존재하는지 확인합니다!
            var targetLayer = attacker.boActor.actorType !=
                ActorType.Monster ? LayerMask.NameToLayer("Monster") : LayerMask.NameToLayer("Player");

            // -> 구체를 발사해서 구체에 맞은 객체가 존재하는지 확인합니다!
            var hits = Physics.SphereCastAll(attacker.transform.position, .5f, attacker.transform.forward,
                attacker.boActor.atkRange, 1 << targetLayer);

            // -> 새로운 타겟 정보를 얻었으니 저장 되어있는 타겟 정보를 지웁니다!
            targets.Clear();

            // -> 새로 얻은 타겟 정보를 저장합니다!
            for (int i = 0; i < hits.Length; i++)
            {
                targets.Add(hits[i].transform.GetComponent<Actor>());
            }
        }

        /// <summary>
        /// => 데미지 연산을 통해 타겟에게 데미지를 입히는 메서드
        /// </summary>
        /// <param name="damage"> 타겟에게 입힐 데미지 값 </param>
        /// <param name="target"> 데미지 처리를 할 타겟 </param>
        public virtual void CalculateDamage(float damage, Actor target)
        {
            // -> Mathf 함수를 이용하여 데미지를 계산합니다!
            var calDamage = Mathf.Max(damage - target.boActor.def, 0);

            // -> 계산된 데미지를 타겟의 Hp에서 빼줍니다!
            target.boActor.currentHp = Mathf.Max(target.boActor.currentHp - calDamage, 0);

            target.SetState(ActorState.Damage);

            // -> 연산 후 타겟의 Hp가 0이거나 0보다 작다면!
            if (target.boActor.currentHp <= 0)
            {
                // -> 타겟은 죽었습니다!
                target.boActor.currentHp = 0;
                target.SetState(ActorState.Dead);
            }
        }

        /// <summary>
        /// => 공격 쿨타임을 업데이트 하는 메서드
        /// </summary>
        public void AttackIntervalUpdate()
        {
            // -> 쿨타입을 체크 할 수 없다면!
            if (!canCheckCoolTime)
            {
                return;
            }

            // -> 지금 공격 쿨타임이 아니라면!
            if (!isCoolTime)
            {
                return;
            }

            currentAtkInterval += Time.fixedDeltaTime;

            if (currentAtkInterval >= attacker.boActor.atkInterval)
            {
                IniAttackInterval();
            }
        }

        /// <summary>
        /// => 공격 쿨타임을 초기화 하는 메서드
        /// </summary>
        public void IniAttackInterval()
        {
            currentAtkInterval = 0;
            isCoolTime = false;
        }
    }
}
