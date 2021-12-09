using ProjectChan.Battle;
using ProjectChan.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.Object
{
    using ActorState = Define.Actor.ActorState;
    using static ProjectChan.Define.Actor;

    public abstract class Actor : MonoBehaviour
    {
        [SerializeField]
        public ActorState State { get; set; }
        protected Vector3 CapVec { get; set; }

        public BoActor boActor;

        public Collider Coll { get; set; }
        protected Rigidbody rigid;
        protected Animator anim;

        public AttackController attackController { get; private set; }
        public WeaponController weaponController { get; private set; }

        protected CharAnimParam charAnim;        // -> 캐릭터 애니메이션 파라미터 ID값 정보
        protected MonAnimParam monAnim;         // -> 몬스터 애니메이션 파라미터 ID값 정보

        public virtual void Initialize(BoActor boActor)
        {
            this.boActor = boActor;

            attackController ??= gameObject.AddComponent<AttackController>();
            attackController.Initialize(this);

            weaponController ??= gameObject.AddComponent<WeaponController>();
        }

        protected virtual void Start()
        {
            Coll ??= GetComponent<Collider>();
            rigid ??= GetComponent<Rigidbody>();
        }

        public virtual void SetStats() { }
        public virtual void MoveUpdate() { }

        public virtual void ActorUpdate()
        {
            attackController.AttackIntervalUpdate();
            attackController.CheckAttack();

            // 공격 상태면 MoveUpdate가 실행되지 않도록 return
            if (State == ActorState.Attack)
                return;

            MoveUpdate();
        }

        /// <summary>
        /// => 오브젝트의 애니메이터에서 애니메이션 파라미터를 얻어올 메서드
        /// </summary>
        /// <param name="type"> ID값을 얻어올 애니메이터를 가진 오브젝트 타입 </param>
        protected void SetAnimParam(ActorType type) 
        {
            switch (boActor.actorType)
            {
                case ActorType.Character:
                case ActorType.Form:
                    charAnim = new CharAnimParam();
                    break;

                case ActorType.Monster:
                    monAnim = new MonAnimParam();
                    break;
            }
        }

        public virtual void SetState(ActorState state)
        {
            var prevState = State;
            State = state;

            // 공통적인 작업은 여기서 처리
            switch (state)
            {
                case ActorState.Idle:
                    OnIdle();
                    break;

                case ActorState.Walk:
                    OnWalk();
                    break;

                case ActorState.Attack:
                    // -> 무기를 장착한 상태에서만!
                    if (!weaponController.isWeapon && boActor.actorType != ActorType.Monster)
                    {
                        State = prevState;
                        return;
                    }

                    // -> 플레이어는 땅에 있을때만!
                    if (!boActor.isGround && boActor.actorType != ActorType.Monster)
                    {
                        State = prevState;
                        return;
                    }

                    // -> 쿨타임이 아닐때만!
                    if (attackController.isCoolTime)
                    {
                        State = prevState;
                        return;
                    }

                    // -> 그러면 공격 실행!
                    OnAttack();
                    break;

                case ActorState.Dead:
                    OnDead();
                    break;
            }
        }

        #region 캐릭터 상태

        protected virtual void OnIdle()
        {
            switch (boActor.actorType)
            {
                case ActorType.Character:
                case ActorType.Form:
                    anim.SetFloat(charAnim.isWalk, 0);
                    anim.SetBool(charAnim.isJump, false);
                    break;

                case ActorType.None:
                case ActorType.Monster:
                    anim.SetBool(monAnim.isWalk, false);
                    break;
            }
        }

        protected virtual void OnWalk()
        {
            switch (boActor.actorType)
            {
                case ActorType.Character:
                case ActorType.Form:
                    anim.SetFloat(charAnim.isWalk, boActor.moveDir.z);
                    break;

                case ActorType.None:
                case ActorType.Monster:
                    anim.SetBool(monAnim.isWalk, true);
                    anim.SetBool(monAnim.isAttack, false);
                    break;
            }
        }

        protected virtual void OnAttack()
        {
            switch (boActor.actorType)
            {
                case ActorType.Character:
                case ActorType.Form:
                    // -> 공격이 시작함에 따라 쿨타임을 체크할 수있는지에 대한 여부는 false
                    //    현재가 공격 쿨타임인지에 대한 여부는 true
                    attackController.canCheckCoolTime = false;
                    attackController.isCoolTime = true;
                    anim.SetBool(charAnim.isAttack, true);

                    // -> 랜덤으로 공격 애니메이션 실행
                    var randAttack = UnityEngine.Random.Range(0, 7);
                    anim.SetInteger(charAnim.randAttack, randAttack + 1);
                    break;

                case ActorType.Monster:
                    // -> 공격이 시작함에 따라 쿨타임을 체크할 수있는지에 대한 여부는 false
                    //    현재가 공격 쿨타임인지에 대한 여부는 true
                    attackController.canCheckCoolTime = false;
                    attackController.isCoolTime = true;
                    anim.SetBool(monAnim.isAttack, true);
                    anim.SetBool(monAnim.isWalk, false);
                    break;
            }
        }

        protected virtual void OnDead()
        {
            switch (boActor.actorType)
            {
                case ActorType.Character:
                case ActorType.Form:
                    anim.SetBool(charAnim.isDead, true);
                    anim.SetBool(charAnim.isAttack, false);
                    break;

                case ActorType.Monster:
                    anim.SetBool(monAnim.isDead, true);
                    anim.SetBool(monAnim.isAttack, false);
                    anim.SetBool(monAnim.isWalk, false);
                    break;
            }
        }

        #endregion

        #region 애니메이션 이벤트

        public virtual void OnAttackHit() { }
        public virtual void OnAttackEnd() { }
        public virtual void OnDeadEnd() { }

        #endregion

    }
}