﻿using ProjectChan.Battle;
using ProjectChan.DB;
using UnityEngine;

namespace ProjectChan.Object
{
    using static ProjectChan.Define.Actor;
    using ActorState = Define.Actor.ActorState;

    public abstract class Actor : MonoBehaviour
    {
        [SerializeField]

        /// <summary>
        /// => 현재 액터의 상태
        /// </summary>
        public ActorState State { get; set; }

        ///protected Vector3 CapVec { get; set; }

        /// <summary>
        /// => 현재 액터의 콜라이더
        /// </summary>
        public Collider Coll { get; set; }

        /// <summary>
        /// => 액터의 공격 작업을 컨트롤 하는 컨트롤러
        /// </summary>
        public AttackController attackController { get; private set; }

        /// <summary>
        /// => 액터의 무기를 컨트롤 하는 컨트롤러
        /// </summary>
        public WeaponController weaponController { get; private set; }

        public BoActor boActor;                 // -> 이 스크립트를 가지고 있는 액터

        protected Rigidbody rigid;              // -> 액터의 리지드바디
        protected Animator anim;                // -> 액터의 애니메이터
        protected CharAnimParam charAnim;       // -> 캐릭터 애니메이션 파라미터 ID값 정보
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
            {
                return;
            }

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

                case ActorState.Damage:
                    OnDamage();
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
                    attackController.canCheckCoolTime = false;
                    attackController.isCoolTime = true;
                    anim.SetBool(charAnim.isAttack, true);
                    break;

                case ActorType.Form:
                    // -> 공격이 시작함에 따라 쿨타임을 체크할 수있는지에 대한 여부는 false
                    //    현재가 공격 쿨타임인지에 대한 여부는 true
                    attackController.canCheckCoolTime = false;
                    attackController.isCoolTime = true;
                    anim.SetBool(charAnim.isAttack, true);

                    // -> 랜덤으로 공격 애니메이션 실행
                    var charRandAttack = UnityEngine.Random.Range(0, 7);
                    anim.SetInteger(charAnim.randAttack, charRandAttack + 1);
                    break;

                case ActorType.Monster:
                    // -> 공격이 시작함에 따라 쿨타임을 체크할 수있는지에 대한 여부는 false
                    //    현재가 공격 쿨타임인지에 대한 여부는 true
                    attackController.canCheckCoolTime = false;
                    attackController.isCoolTime = true;
                    anim.SetBool(monAnim.isAttack, true);
                    anim.SetBool(monAnim.isWalk, false);

                    // -> 어택타입이 보스라면!
                    if (boActor.atkType == AttackType.Boss)
                    {
                        var monRandAttack = UnityEngine.Random.Range(0, 2);
                        anim.SetInteger(monAnim.randAttack, monRandAttack + 1);
                    }
                    break;
            }
        }

        protected virtual void OnDamage()
        {
            switch (boActor.actorType)
            {
                case ActorType.Character:
                case ActorType.Form:
                    anim.SetTrigger(charAnim.isDamage);
                    break;

                case ActorType.Monster:
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