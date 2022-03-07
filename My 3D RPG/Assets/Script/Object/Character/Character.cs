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

    /// <summary>
    /// => 플레이어 캐릭터 오브젝트의 클래스
    /// </summary>
    public class Character : Actor
    {
        // public 
        public PlayerController playerController;       // -> 캐릭터를 컨트롤 하는 컨트롤러
        public BoCharacter boCharacter;                 // -> 현재 캐릭터의 스텟정보

        public override void Initialize(BoActor boActor)
        {
            base.Initialize(boActor);
            boCharacter = boActor as BoCharacter;

            OriginStats();
            SetAnimParam(boActor.actorType);
        }

        /// <summary>
        /// => 플레이어 스텟을 설정하는 메서드
        /// </summary>
        public override void OriginStats()
        {
            weaponController.PosClear();

            // -> 캐릭터 정보를 넣습니다!
            var sdCharacter = boCharacter.sdCharacter;

            // -> 레벨의 영향을 받지않는 스텟을 설정합니다!
            boCharacter.actorType = ActorType.Character;
            boCharacter.atkType = sdCharacter.atkType;
            boCharacter.moveSpeed = sdCharacter.moveSpeed;
            boCharacter.atkRange = sdCharacter.atkRange;
            boCharacter.atkInterval = sdCharacter.atkInterval;
            boCharacter.jumpForce = sdCharacter.jumpForce;

            // -> 레벨의 영향을 받는 스텟을 설정합니다!
            boCharacter.currentHp =
                boCharacter.maxHp = boCharacter.level * boCharacter.sdOriginInfo.maxHp * boCharacter.sdOriginInfo.maxHpFactor;
            boCharacter.currentEnergy =
                boCharacter.maxEnergy = boCharacter.level * boCharacter.sdOriginInfo.maxEnergy * boCharacter.sdOriginInfo.maxEnergyFactor;
            boCharacter.atk = boCharacter.level * boCharacter.sdOriginInfo.atk * boCharacter.sdOriginInfo.atkFactor;
            boCharacter.def = boCharacter.level * boCharacter.sdOriginInfo.def * boCharacter.sdOriginInfo.defFactor;

            // -> 현재 활성화된 캐릭터의 웨폰 정보를 가져오기 위해서 캐릭터 타입을 보내줍니다!
            weaponController.Initialize(boCharacter.actorType);

            // -> 현재 활성화된 캐릭터의 애니메이터를 사용하기 위해서 가져옵니다!
            anim = transform.GetChild(0).GetComponent<Animator>();
        }

        /// <summary>
        /// => 변신 상태의 스텟을 설정하는 메서드
        /// </summary>
        public void FormStats()
        {
            weaponController.PosClear();

            // -> 레벨의 영향을 받지않는 스텟을 설정합니다!
            boCharacter.actorType = ActorType.Form;
            boCharacter.moveSpeed = boCharacter.sdFormInfo.moveSpeed;
            boCharacter.atkRange = boCharacter.sdFormInfo.atkRange;

            // -> 레벨의 영햐을 받는 스텟을 설정합니다!
            boCharacter.currentHp =
                boCharacter.maxHp = boCharacter.level * boCharacter.sdFormInfo.maxHp * boCharacter.sdFormInfo.maxHpFactor;
            boCharacter.currentEnergy =
                boCharacter.maxEnergy = boCharacter.level * boCharacter.sdFormInfo.maxEnergy * boCharacter.sdFormInfo.maxEnergyFactor;
            boCharacter.atk = boCharacter.level * boCharacter.sdFormInfo.atk * boCharacter.sdFormInfo.atkFactor;
            boCharacter.def = boCharacter.level * boCharacter.sdFormInfo.def * boCharacter.sdFormInfo.defFactor;

            weaponController.Initialize(boCharacter.actorType);

            // -> 현재 활성화된 캐릭터의 애니메이터를 사용하기 위해서 가져옵니다!
            anim = transform.GetChild(1).GetComponent<Animator>();
        }

        /// <summary>
        /// => 현재 캐릭터의 상태를 설정하는 메서드
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
            /// => TransformDirection : 방향벡터를 로컬 좌표계 기준에서 월드 좌표계 기준으로 바꿔줌
            var velocity = boActor.moveSpeed * boActor.moveDir;
            velocity = transform.TransformDirection(velocity);

            transform.localPosition += velocity * Time.fixedDeltaTime;
            transform.Rotate(boActor.rotDir * Define.Camera.CamRotSpeed);

            // -> 달릴려고 하는데 점프 상태라면!
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
                // -> 점프 키를 눌렀을 때 점프를 실행합니다!
                SetState(ActorState.Jump);
            }

            OutJump();
        }

        /// <summary>
        /// => 플레이어가 그라운드 위에 있는지 체크하는 메서드
        /// </summary>
        private void CheckGround()
        {
            // -> 레이를 이용하여 그라운드를 확인합니다!
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, .05f, 1 << LayerMask.NameToLayer("Floor")))
            {
                Debug.DrawRay(transform.position, Vector3.down * .05f, Color.red);

                var distance = Mathf.Round(hit.distance);
                boActor.isGround = Mathf.Approximately(distance, 0f);
            }
            else
            {
                // -> 레이가 그라운드를 인지 하지 못했다면 아직 그라운드에 닿지 않은 상태 입니다!
                boActor.isGround = false;
            }

            // -> 플레이어의 상태가 점프가 아니라면!
            if (State != ActorState.Jump)
            {
                return;
            }

            // -> 점프 상태이지만 플레이어의 위치가 그라운드에 닿은 상태라면!
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
            // -> 플레이어가 땅에 없다면!
            if (!boActor.isGround)
            {
                return;
            }

            // -> 플레이어의 상태가 공격이라면!
            if (State == ActorState.Attack)
            {
                return;
            }

            // -> 리지드바디에 힘을 가하여 플레이어를 점프 시킵니다!
            rigid.AddForce(Vector3.up * boCharacter.jumpForce, ForceMode.Impulse);
            anim.SetBool(charAnim.isJump, true);
        }

        /// <summary>
        /// => 플레이어의 착지 시작 메서드
        /// </summary>
        public void OutJump()
        {
            // -> 리지드바디의 벨로시티의 Y값이 음수라면 플레이어가 하강 중이므로, 하강 애니메이션을 실행 합니다!
            // -> 또한 플레이어가 땅에 없어야 합니다!
            if (rigid.velocity.y < 0 && !boActor.isGround)
            {
                anim.SetBool(charAnim.isJump, false);
            }
        }

        /// <summary>
        /// => 플레이어가 변신 하는 메서드
        /// </summary>
        public void ChangeForm()
        {
            if (Input.GetButtonDown("FormChange") &&
                boActor.currentEnergy > Define.StaticData.ChangeFormValue &&
                !playerController.isPlayerAction)
            {
                boActor.currentEnergy -= Define.StaticData.ChangeFormValue;

                // -> 코하쿠 --> 라이덴
                if (transform.GetChild(0).gameObject.activeSelf == true && transform.GetChild(1).gameObject.activeSelf == false)
                {
                    // -> 코하쿠 상태의 Hp와 Mana를 Dic에 저장합니다!
                    SavePrevStat(boCharacter.actorType);

                    // -> 코하쿠 모델은 꺼주고 라이덴 모델을 켜줍니다!
                    transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetChild(1).gameObject.SetActive(true);

                    // -> 라이덴 스텟으로 설정해줍니다!
                    FormStats();

                    // -> 전에 사용하고 저장해둔 라이덴 모델의 스텟이 있다면 그 스텟으로 재설정 해줍니다!
                    GetPrevStat(boCharacter.actorType);
                }
                // -> 라이덴 --> 코하쿠
                else if (transform.GetChild(1).gameObject.activeSelf == true && transform.GetChild(0).gameObject.activeSelf == false)
                {
                    // -> 라이덴 상태의 Hp와 Mana를 Dic에 저장합니다!
                    SavePrevStat(boCharacter.actorType);

                    // -> 라이덴 모델은 꺼주고 라이덴 모델을 켜줍니다!
                    transform.GetChild(1).gameObject.SetActive(false);
                    transform.GetChild(0).gameObject.SetActive(true);

                    // -> 코하쿠 스텟으로 설정해줍니다!
                    OriginStats();

                    // -> 전에 사용하고 저장해둔 코하쿠 모델의 스텟이 있다면 그 스텟으로 재설정 해줍니다!
                    GetPrevStat(boCharacter.actorType);
                }
            }

            // -> 코하쿠에서 라이덴으로 변신할 때 이전의 코하쿠 스텟을 저장해놓는 작업
            // -> 라이덴에서 코하쿠로 변신할 때 이전의 라이덴 스텟을 저장해놓는 작업
            void SavePrevStat(ActorType actorType)
            {
                // -> Dic에 key값이 존재한다면 그냥 key값을 이용하여 저장한다
                if (GameManager.User.boPrevStatDic.ContainsKey(actorType))
                {
                    GameManager.User.boPrevStatDic[actorType].prevHp = boCharacter.currentHp;
                    GameManager.User.boPrevStatDic[actorType].prevEnergy = boCharacter.currentEnergy;
                }
                else
                {
                    GameManager.User.boPrevStatDic.Add
                        (actorType, new BoPrevStat(boCharacter.currentHp, boCharacter.currentEnergy));
                }
            }

            // -> 변신 후 이전에 사용 한 스텟이 있다면 가져와서 세팅하는 로컬 메서드
            void GetPrevStat(ActorType actorType)
            {
                // -> key값이 없으면 그냥 풀피 ( key값이 없다는건 라이덴 폼에서 코하쿠로 돌아온적이 아직 없다 )
                if (GameManager.User.boPrevStatDic.ContainsKey(actorType))
                {
                    var boPrevStat = GameManager.User.boPrevStatDic[actorType];

                    if (boPrevStat.prevHp < 0)
                    {
                        boCharacter.currentHp = 0;
                    }
                    else
                    {
                        boCharacter.currentHp = boPrevStat.prevHp;
                        boCharacter.currentEnergy = boPrevStat.prevEnergy;
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
        /// => 플레이어가 무기를 착용하거나 해제하는 메서드
        /// </summary>
        public void ChangeWeapon()
        {
            if (Input.GetButtonDown("WeaponChange"))
            {
                // -> 땅이 아니라면!
                if (!boActor.isGround || boActor.currentEnergy < Define.StaticData.WeaponValue)
                {
                    return;
                }

                // -> 무기를 바꿀 땐 이동 입력과 이동을 멈춥니다!
                playerController.isPlayerAction = true;
                playerController.PlayerCharacter.boActor.moveDir = Vector3.zero;
                var newDir = Vector3.zero;
                playerController.PlayerCharacter.boActor.rotDir = newDir;

                boActor.currentEnergy -= Define.StaticData.WeaponValue;

                // -> 장착을 하고 있는 상태라면 무기를 해제!
                // -> 아니라면 무기를 장착!
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
            // -> 만약 Max값을 넘어가거나 같다면!
            if (boActor.currentEnergy >= boActor.maxEnergy)
            {
                boActor.currentEnergy = boActor.maxEnergy;
                return;
            }

            // -> 차지를 충전합니다!
            boActor.currentEnergy += Time.deltaTime * 3f;
        }

        #region 애니메이션 이벤트

        /// <summary>
        /// => 공격 애니메이션이 시작할 때 실행될 메서드
        /// </summary>
        public override void OnAttackHit()
        {
            attackController.OnAttack();
        }

        /// <summary>
        /// => 공격 내이메이션이 끝날 때 실행될 메서드
        /// </summary>
        public override void OnAttackEnd()
        {
            attackController.canCheckCoolTime = true;
            anim.SetBool(charAnim.isAttack, false);
            SetState(ActorState.Idle);
        }

        /// <summary>
        /// => 무기 애니메이션이 시작할 때 실행될 메서드
        /// </summary>
        public void OnChangeWeapon()
        {
            weaponController.SetWeapon();

        }

        /// <summary>
        /// => 무기 애니메이션이 끝날 때 실행될 메서드
        /// </summary>
        public void OnChangeWeaponEnd()
        {
            // -> 무기 바꾸는게 끝나면 움직일 수 있다!
            playerController.isPlayerAction = false;
        }

        /// <summary>
        /// => 플레이어가 죽으면 실행될 메서드
        /// </summary>
        public override void OnDeadEnd()
        {
            gameObject.SetActive(false);

            base.OnDeadEnd();
        }

        #endregion
    }
}