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
    public class AttackController : MonoBehaviour
    {
        public bool hasTarget;                              // -> 공격 대상이 있는지?
        public bool canCheckCoolTime;                       // -> 공격 쿨타임을 체크할 수있는지? ( 공격 모션이 끝나기 전에는 쿨타임 체크를 막는다 )
        public bool isCoolTime;                             // -> 공격 쿨타임인지?
        public bool canAtk;                                 // -> 공격 가능 상태인지?
        private float currentAtkInterval;                   // -> 현재 공격 쿨타임을 체크하는 값
        private Actor attacker;                             // -> 공격자 ( 해당 어택 컨트롤러 인스턴스를 갖는 액터 )
        private List<Actor> targets = new List<Actor>();    // -> 타겟을 넣어놓을 리스트

        public void Initialize(Actor attacker)
        {
            this.attacker = attacker;
        }

        /// <summary>
        /// 공격 가능 상태라면! 공격자의 상태를 공격상태로 변경
        /// </summary>
        public void CheckAttack()
        {
            // -> 타겟이 없다면 리턴
            if (!hasTarget)
            {
                return;
            }

            // -> 공격 쿨타임이라면 리턴
            if (isCoolTime)
            {
                return;
            }

            // -> 공격 불가능이라면 리턴
            if (!canAtk)
            {
                return;
            }

            attacker.SetState(ActorState.Attack);
        }

        /// <summary>
        /// => OnAttackHit메서드가 실행되면 실행될 메서드
        /// </summary>
        public virtual void OnAttack()
        {
            switch (attacker.boActor.atkType)
            {
                // -> 일반 공격
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
        /// => 공격 범위에 적이 있는지? 연산
        /// </summary>
        public virtual void CalculateAttackRange()
        {
            // -> 적을 감지한다
            var targetLayer = attacker.boActor.actorType !=
                ActorType.Monster ? LayerMask.NameToLayer("Monster") : LayerMask.NameToLayer("Player");

            var hits = Physics.SphereCastAll(attacker.transform.position, .5f, attacker.transform.forward,
                attacker.boActor.atkRange, 1 << targetLayer);

            // -> 새로운 타겟의 정보를 얻었으니, 저장 되어있는 타겟의 정보를 지운다
            targets.Clear();

            // -> 새로운 타겟 정보를 목록에 저장한다
            for (int i = 0; i < hits.Length; i++)
            {
                targets.Add(hits[i].transform.GetComponent<Actor>());
            }
        }

        /// <summary>
        /// => 데미지를 공식에 따라 연산하여 타겟에 적용시킴
        /// </summary>
        /// <param name="damage"> 타겟에게 입힐 데미지 값 </param>
        /// <param name="target"> 데미지 처리를 할 타겟 </param>
        public virtual void CalculateDamage(float damage, Actor target)
        {
            // -> Mathf 함수를 이용하여 데미지 계산
            var calDamage = Mathf.Max(damage - target.boActor.def, 0);

            // -> 계산된 데미지를 타겟에게 적용
            target.boActor.currentHp = Mathf.Max(target.boActor.currentHp - calDamage, 0);

            target.SetState(ActorState.Damage);

            // -> 타겟의 체력이 0이라면 죽은 상태
            if (target.boActor.currentHp <= 0)
            {
                // -> Dead 애니메이션 실행
                target.boActor.currentHp = 0;
                target.SetState(ActorState.Dead);
            }
        }

        /// <summary>
        /// => 공격 쿨타임을 업데이트 한다
        /// </summary>
        public void AttackIntervalUpdate()
        {
            // -> 쿨타임을 체크 할 수없다면 컷
            if (!canCheckCoolTime)
            {
                return;
            }

            // -> 공격 쿨타임이 아니라면 컷
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
        /// => 공격 쿨타임을 초기화 한다
        /// </summary>
        public void IniAttackInterval()
        {
            currentAtkInterval = 0;
            isCoolTime = false;
        }
    }
}
