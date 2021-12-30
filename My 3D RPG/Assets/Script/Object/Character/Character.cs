using ProjectChan.DB;
using ProjectChan.Dummy;
using ProjectChan.SD;
using ProjectChan.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.Object
{
    using ActorState = Define.Actor.ActorState;
    using static ProjectChan.Define.Actor;

    public class Character : Actor
    {
        private float charged;                          // -> 기력을 충전하기 위한 대기시간

        public bool canCharge;
        public bool isRun;
        public PlayerController playerController;       // -> 캐릭터를 컨트롤 해줄 컨트롤러
        public BoCharacter boCharacter;                 // -> 현재 캐릭터가 지닌 스텟정보

        public override void Initialize(BoActor boActor)
        {
            base.Initialize(boActor);
            boCharacter = boActor as BoCharacter;

            SetStats();
            SetAnimParam(boActor.actorType);
        }

        /// <summary>
        /// => 기존의 플레이어 스텟을 설정
        /// </summary>
        public override void SetStats()
        {
            weaponController.PosClear();

            // -> 캐릭터 정보 넣기
            var sdCharacter = boCharacter.sdCharacter;

            boCharacter.actorType = ActorType.Character;
            boCharacter.atkType = sdCharacter.atkType;
            boCharacter.moveSpeed = sdCharacter.moveSpeed;
            boCharacter.atkRange = sdCharacter.atkRange;
            boCharacter.atkInterval = sdCharacter.atkInterval;
            boCharacter.jumpForce = sdCharacter.jumpForce;

            // -> 기본 상태 스텟
            boCharacter.currentHp =
                boCharacter.maxHp = boCharacter.level * boCharacter.sdOriginInfo.maxHp * boCharacter.sdOriginInfo.maxHpFactor;
            boCharacter.currentEnergy =
                boCharacter.maxEnergy = boCharacter.level * boCharacter.sdOriginInfo.maxMana * boCharacter.sdOriginInfo.maxManaFactor;

            boCharacter.atk = boCharacter.level * boCharacter.sdOriginInfo.atk * boCharacter.sdOriginInfo.atkFactor;
            boCharacter.def = boCharacter.level * boCharacter.sdOriginInfo.def * boCharacter.sdOriginInfo.defFactor;

            var sdWeapon = GameManager.SD.sdWeapons.Where
                (obj => obj.index == boCharacter.sdOriginInfo.weaponIndex)?.SingleOrDefault();

            weaponController.Initialize(boCharacter.actorType, sdWeapon);

            // -> 현재 활성화된 캐릭터의 Animator을 사용하기 위해 가져온다
            anim = transform.GetChild(0).GetComponent<Animator>();
        }

        /// <summary>
        /// => 변신 스텟 설정
        /// </summary>
        public void ChangeStats()
        {
            weaponController.PosClear();

            //boCharacter.atkType = sdFormStat.atkType;      
            boCharacter.actorType = ActorType.Form;
            boCharacter.moveSpeed = boCharacter.sdFormInfo.moveSpeed;
            boCharacter.atkRange = boCharacter.sdFormInfo.atkRange;

            // -> 변신 상태 스텟
            boCharacter.currentHp =
                boCharacter.maxHp = boCharacter.level * boCharacter.sdFormInfo.maxHp * boCharacter.sdFormInfo.maxHpFactor;
            boCharacter.currentEnergy =
                boCharacter.maxEnergy = boCharacter.level * boCharacter.sdFormInfo.maxMana * boCharacter.sdFormInfo.maxManaFactor;

            boCharacter.atk = boCharacter.level * boCharacter.sdFormInfo.atk * boCharacter.sdFormInfo.atkFactor;
            boCharacter.def = boCharacter.level * boCharacter.sdFormInfo.def * boCharacter.sdFormInfo.defFactor;

            var sdWeapon = GameManager.SD.sdWeapons.Where
                (obj => obj.index == boCharacter.sdFormInfo.weaponIndex)?.SingleOrDefault();

            weaponController.Initialize(boCharacter.actorType, sdWeapon);

            // -> 현재 활성화된 캐릭터의 Animator을 사용하기 위해 가져온다
            anim = transform.GetChild(1).GetComponent<Animator>();
        }

        /// <summary>
        /// 캐릭터의 상태를 설정
        /// </summary>
        /// <param name="state"> 캐릭터의 상태 </param>
        public override void SetState(ActorState state)
        {
            var prevState = State;      

            base.SetState(state);

            switch (state)
            {
                case ActorState.Sit:
                    break;

                case ActorState.Rise:
                    break;

                case ActorState.Jump:

                    if (prevState == ActorState.Attack)
                    {
                        Debug.Log("공격중 점프 안됩니다");
                        State = prevState;
                        return;
                    }

                    OnJump();
                    break;
            }
        }

        /// <summary>
        /// => 플레이어 업데이트 메서드
        /// => 계속해서 도는 중
        /// </summary>
        public override void ActorUpdate()
        {
            CheckGround();
            ChangeForm();
            ChangeWeapon();
            JumpUpdate();
            EnergyReCharge();
            ItemUsed();
            base.ActorUpdate();
        }

        /// <summary>
        /// => 플레이어 이동에 관련된 업데이트 메서드
        /// </summary>
        public override void MoveUpdate()
        {
            /// => TransformDirection : 방향벡터를 로컬 좌표계 기준에서 월드 좌표꼐 기준으로 바꿔줌
            var velocity = boActor.moveSpeed * boActor.moveDir;
            velocity = transform.TransformDirection(velocity);

            transform.localPosition += velocity * Time.fixedDeltaTime;
            transform.Rotate(boActor.rotDir * Define.Camera.CamRotSpeed);

            // -> 점프 상태는 예외 처리
            if (State == ActorState.Jump)
            {
                return;
            }

            /// => Mathf.Approximately : a데이터와 b데이터가 비슷한지 비교해주는 함수 (float 데이터를 비교할때 사용한다)
            if (Mathf.Approximately(velocity.magnitude, 0))
            {
                SetState(ActorState.Idle);
            }
            else
            {
                SetState(ActorState.Walk);
            }
        }

        /// <summary>
        /// => 플레이어의 점프에 관련된 메서드
        /// </summary>
        public void JumpUpdate()
        {
            if (Input.GetButtonDown(Define.Input.Jump))
            {
                // -> 점프 키를 눌렀을 때 점프 실행
                SetState(ActorState.Jump);
            }

            // -> 착지 애니메이션
            OutJump();
        }

        /// <summary>
        /// => 플레이어의 위치가 Floor인지 체크하는 메서드
        /// </summary>
        private void CheckGround()
        {
            // -> 레이를 이용하여 Floor를 확인한다
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, .05f, 1 << LayerMask.NameToLayer("Floor")))
            {
                Debug.DrawRay(transform.position, Vector3.down * .05f, Color.red);

                var distance = Mathf.Round(hit.distance);
                boActor.isGround = Mathf.Approximately(distance, 0f);
            }
            else
            {
                // -> 레이가 Floor를 인지 하지 못했다면 아직 땅에 닿지 않은 상태
                boActor.isGround = false;
            }

            // -> 플레이어의 상태가 점프가 아니라면
            if (State != ActorState.Jump)
            {
                return;
            }

            // -> 점프 상태이지만 플레이어의 위치가 땅에 닿은 상태라면
            if (boActor.isGround)
            {
                SetState(ActorState.Idle);
            }
        }

        /// <summary>
        /// => 플레이어 점프 시작 메서드
        /// </summary>
        public void OnJump()
        {
            // -> 플레이어가 땅에 없다면
            if (!boActor.isGround)
            {
                return;
            }

            // -> 플레이어의 상태가 공격이라면
            if (State == ActorState.Attack)
            {
                return;
            }

            // -> 리지드바디에 힘을 가하여 플레이어를 점프 시킨다
            rigid.AddForce(Vector3.up * boCharacter.jumpForce, ForceMode.Impulse);
            anim.SetBool(charAnim.isJump, true);
        }

        /// <summary>
        /// => 플레이어의 착지 시작 메서드
        /// </summary>
        public void OutJump()
        {
            // -> 리지드바디의 벨로시티의 Y값이 음수라면 플레이어가 하강 중이므로, 하강 애니메이션 실행
            // -> 또한 플레이어가 땅에 없어야 한다
            if (rigid.velocity.y < 0 && !boActor.isGround)
            {
                anim.SetBool(charAnim.isJump, false);
            }
        }
        public void ChangeForm()
        {
            if (Input.GetButtonDown("FormChange") && boActor.currentEnergy > Define.StaticData.ChangeFormValue && !playerController.isPlayerAction)
            {
                boActor.currentEnergy -= Define.StaticData.ChangeFormValue;

                // -> 코하쿠 >> 라이덴
                if (transform.GetChild(0).gameObject.activeSelf == true &&
                    transform.GetChild(1).gameObject.activeSelf == false)
                {
                    // -> 코하쿠 상태의 Hp와 Mana를 Dic에 담아놓는다
                    // -> Dic에 key값이 존재한다면 그냥 key값을 이용하여 저장한다
                    SavePrevStat(boCharacter.actorType);

                    // -> 코하쿠 모델은 false
                    // -> 라이덴 모델은 true
                    transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetChild(1).gameObject.SetActive(true);

                    ChangeStats();

                    // -> 라이덴으로 변신했을때 전에 사용하고 저장해뒀던 라이덴 폼의 스탯을 재설정해준다
                    // -> key값이 없으면 그냥 풀피 ( key값이 없다는건 라이덴 폼에서 코하쿠로 돌아온적이 아직 없다 )
                    SetPrevStat(boCharacter.actorType);
                }
                // -> 라이덴 >> 코하쿠
                else if (transform.GetChild(1).gameObject.activeSelf == true &&
                         transform.GetChild(0).gameObject.activeSelf == false)
                {
                    // -> 라이덴 상태의 Hp와 Mana를 Dic에 담아놓는다
                    // -> Dic에 key값이 존재한다면 그냥 key값을 이용하여 저장한다
                    SavePrevStat(boCharacter.actorType);

                    transform.GetChild(1).gameObject.SetActive(false);
                    transform.GetChild(0).gameObject.SetActive(true);

                    SetStats();

                    // -> 코하쿠로 돌아왔을때 전에 사용하고 저장해뒀던 코하쿠의 스탯을 재설정해준다
                    // -> key값이 없으면 그냥 풀피 ( key값이 없다는건 코하쿠에서 라이덴 폼으로 변신한적이 아직 없다 )
                    SetPrevStat(boCharacter.actorType);
                }
            }

            // -> 코하쿠에서 라이덴으로 변신할때 이전의 코하쿠 스텟을 저장해놓는 작업
            // -> 라이덴에서 코하쿠로 변신할때 이전의 라이덴 스텟을 저장해놓는 작업
            void SavePrevStat(ActorType actorType)
            {
                if (GameManager.User.boPrevStatDic.ContainsKey(actorType))
                {
                    GameManager.User.boPrevStatDic[actorType].currentHp = boCharacter.currentHp;
                    GameManager.User.boPrevStatDic[actorType].currentMana = boCharacter.currentEnergy;
                }
                else
                {
                    GameManager.User.boPrevStatDic.Add
                        (actorType, new BoPrevStat(boCharacter.currentHp, boCharacter.currentEnergy));
                }
            }

            // -> 코하쿠에서 라이덴으로 변신할때 이전에 저장해놓은 라이덴 스텟을 적용하는 작업
            // -> 라이덴에서 코하쿠로 변신할때 이전에 저장해놓은 코하쿠 스텟을 적용하는 작업
            void SetPrevStat(ActorType actorType)
            {
                if (GameManager.User.boPrevStatDic.ContainsKey(actorType))
                {
                    var boPrevStat = GameManager.User.boPrevStatDic[actorType];

                    if (boPrevStat.currentHp < 0)
                    {
                        boCharacter.currentHp = 0;
                    }
                    else
                    {
                        boCharacter.currentHp = boPrevStat.currentHp;
                        boCharacter.currentEnergy = boPrevStat.currentMana;
                    }
                }
                else
                {
                    GameManager.User.boPrevStatDic.Add
                      (actorType, new BoPrevStat(boCharacter.currentHp, boCharacter.currentEnergy));
                }
            }
        }

        /// <summary>
        /// => 플레이어가 무기 착용을 실행하는 메서드
        /// </summary>
        public void ChangeWeapon()
        {
            if (Input.GetButtonDown("WeaponChange"))
            {
                if (!boActor.isGround)
                {
                    return;
                }

                // -> 무기 바꿀땐 이동 입력이랑 이동 멈추기
                playerController.isPlayerAction = true;
                playerController.PlayerCharacter.boActor.moveDir = Vector3.zero;
                var newDir = Vector3.zero;
                playerController.PlayerCharacter.boActor.rotDir = newDir;

                if (!weaponController.isWeapon)
                {
                    anim.SetTrigger(charAnim.InWeapon);
                }
                else if (weaponController.isWeapon)
                {
                    anim.SetTrigger(charAnim.OutWeapon);
                }
            }
        }

        /// <summary>
        /// => 아이템 키의 입력을 받는 메서드
        /// </summary>
        /// 
        private void ItemUsed()
        {
            var uiInventory = UIWindowManager.Instance.GetWindow<UIInventory>();

            for (KeyCode i = Define.ItemData.startNumer; i <= Define.ItemData.endNumber; ++i)
            {
                if (Input.GetKeyDown(i))
                {
                    ItemKey(i);
                }
            }

            // -> 로컬 함수
            void ItemKey(KeyCode value)
            {
                int index = (int)value - Define.ItemData.interval;
                uiInventory.UsedItem(boActor, index - 1);
            }
        }

        /// <summary>
        /// => 플레이어 기력 충전 메서드
        /// </summary>
        private void EnergyReCharge()
        {
            // -> 0보다 작다면 차지를 시작
            if (boActor.currentEnergy <= 0)
            {
                canCharge = true;
            }

            // -> 만약 Max값을 넘어가거나 같다면!
            if (boActor.currentEnergy >= boActor.maxEnergy)
            {
                boActor.currentEnergy = boActor.maxEnergy;
                /// 이제 달릴 수 있습니다
                canCharge = false;
                return;
            }

            if (canCharge)
            {
                // -> 차지 충전
                boActor.currentEnergy += Time.deltaTime * 5f;
            }
        }

        #region 애니메이션 이벤트

        public override void OnAttackHit()
        {
            attackController.OnAttack();
        }

        public override void OnAttackEnd()
        {
            attackController.canCheckCoolTime = true;
            anim.SetBool(charAnim.isAttack, false);
            SetState(ActorState.Idle);
        }

        public void OnChangeWeapon()
        {
            weaponController.SetWeapon();

        }
        public override void OnDeadEnd()
        {
            gameObject.SetActive(false);

            base.OnDeadEnd();
        }

        public void OnChangeWeaponEnd()
        {
            // -> 무기 바꾸는게 끝나면 움직일 수 있다!
            playerController.isPlayerAction = false;
        }

        public void OnJumpStart()
        {

        }

        public void OnJumpEnd()
        {

        }


        #endregion
    }
}