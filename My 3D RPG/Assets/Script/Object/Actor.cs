using ProjectChan.Battle;
using ProjectChan.DB;
using UnityEngine;

namespace ProjectChan.Object
{
    using static ProjectChan.Define.Actor;
    using ActorState = Define.Actor.ActorState;

    /// <summary>
    /// => 인게임 내에 다이나믹하게 행동하는 객체들의 추상화된 베이스 클래스
    ///    캐릭터, 몬스터 등 Actor의 파생클래스에서 공통괴는 기능은 최대한 Actor에 정의
    ///    파생 클래스에 따라 다른 기능은 해당 파생 클래스에서 별도로 정의
    /// </summary>
    public abstract class Actor : MonoBehaviour
    {
        // public 
        public BoActor boActor;                 // -> 이 스크립트를 가지고 있는 액터

        /// <summary>
        /// => 현재 액터의 상태
        /// </summary>
        public ActorState State { get; set; }

        /// <summary>
        /// => 현재 액터의 콜라이더
        /// </summary>
        public Collider Coll { get; set; }

        /// <summary>
        /// => 액터의 공격을 컨트롤 하는 컨트롤러
        /// </summary>
        public AttackController attackController { get; private set; }

        /// <summary>
        /// => 액터의 무기를 컨트롤 하는 컨트롤러
        /// </summary>
        public WeaponController weaponController { get; private set; }

        protected Rigidbody rigid;              // -> 액터의 리지드바디
        protected Animator anim;                // -> 액터의 애니메이터
        protected CharAnimParam charAnim;       // -> 캐릭터 애니메이션 파라미터 ID값 정보
        protected MonAnimParam monAnim;         // -> 몬스터 애니메이션 파라미터 ID값 정보

        /// <summary>
        /// => 액터 초기화 메서드
        /// => 초기화 시 외부에서 BoActor 데이터를 주입 받는다
        /// </summary>
        /// <param name="boActor"></param>
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

        /// <summary>
        /// => 액터 스텟 설정 추상 메서드
        /// </summary>
        public virtual void OriginStats() { }

        /// <summary>
        /// => 이동 업데이트 추상 메서드
        /// </summary>
        public virtual void MoveUpdate() { }

        /// <summary>
        /// => 액터 인스턴스의 모든 업데이트를 담당하는 메서드
        /// </summary>
        public virtual void ActorUpdate()
        {
            attackController.AttackIntervalUpdate();
            attackController.CheckAttack();

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

        /// <summary>
        /// => 액터의 상태 변경 메서드
        /// </summary>
        /// <param name="state"> 변경하고자 하는 상태 </param>
        public virtual void SetState(ActorState state)
        {
            // -> 전에 있던 State를 prevState에 담아놓고
            //    파라미터로 받아온 state를 State에 담아놓습니다!
            var prevState = State;
            State = state;

            // -> 액터의 파생 객체들의 공통적으로 갖는 상태만을 베이스에서 처리 합니다!
            //    그 후 파생 객체에 따라 추가적으로 갖는 상태는 해당 파생클래스에서 별도로 처리합니다!
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

        /// <summary>
        /// => 대기 상태로 변경 시 한번 호출되는 메서드
        /// </summary>
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

        /// <summary>
        /// => 걷는 상태로 변경 시 한번 호출되는 메서드
        /// </summary>
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

        /// <summary>
        /// => 공격 상태로 변경 시 한번 호출되는 메서드
        /// </summary>
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
                    //    현재가 공격 쿨타임인지에 대한 여부는 true 입니다!
                    attackController.canCheckCoolTime = false;
                    attackController.isCoolTime = true;
                    anim.SetBool(charAnim.isAttack, true);

                    // -> 랜덤으로 공격 애니메이션 실행 입니다!
                    var charRandAttack = UnityEngine.Random.Range(0, 7);
                    anim.SetInteger(charAnim.randAttack, charRandAttack + 1);
                    break;

                case ActorType.Monster:
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

        /// <summary>
        /// => 데미지 상태로 변경 시 한번 호출되는 메서드
        /// </summary>
        protected virtual void OnDamage()
        {
            switch (boActor.actorType)
            {
                case ActorType.Character:
                case ActorType.Form:
                    attackController.isCoolTime = false;
                    anim.SetBool(charAnim.isAttack, false);
                    anim.SetTrigger(charAnim.isDamage);
                    break;

                case ActorType.Monster:
                    break;
            }
        }

        /// <summary>
        /// => 죽음 상태로 변경 시 한번 호출되는 메서드
        /// </summary>
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