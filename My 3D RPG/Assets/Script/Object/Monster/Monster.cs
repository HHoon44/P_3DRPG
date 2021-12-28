﻿using ProjectChan.DB;
using ProjectChan.Define;
using ProjectChan.Dummy;
using ProjectChan.NetWork;
using ProjectChan.Resource;
using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using static ProjectChan.Define.Actor;
using Random = UnityEngine.Random;

namespace ProjectChan.Object
{
    public class Monster : Actor, IPoolableObject
    {
        private float currentPatrolWaitTime;    // -> 현재 정찰 대기시간
        private float patrolWaitTime;           // -> 정찰 대기시간 최대값
        private Vector3 destPos;                // -> 목적지 위치 (정찰 위치, 타겟 위치)
        public BoMonster boMonster;             // -> 몬스터의 정보가 담겨있는 Bo데이터
        private NavMeshAgent agent;             // -> NavMesh 위에서 움직일 대상 (몬스터)
        private NavMeshPath path;               // -> 에이전트가 돌아다닐 경로
        private GameObject itemHolder;          // -> 몬스터가 떨군 아이템을 자식으로 달아놓을 게임오브젝트

        public bool CanRecycle { get; set; } = true;

        public override void Initialize(BoActor boActor)
        {
            base.Initialize(boActor);

            boMonster = boActor as BoMonster;

            SetStats();
            InitPatrolWaitTime();
            SetAnimParam(boActor.actorType);

            // -> 목적지 위치를 몬스터의 현재 위치로 설정
            // -> 이렇게 하면 바로 몬스터가 새로운 목적지를 설정함
            destPos = transform.position;
        }

        protected override void Start()
        {
            base.Start();

            agent = GetComponent<NavMeshAgent>();
            path = new NavMeshPath();
            anim = GetComponent<Animator>();
        }

        /// <summary>
        /// => 몬스터 스탯을 설정하는 메서드
        /// </summary>
        public override void SetStats()
        {
            if (boMonster == null)
            {
                return;
            }

            // 몬스터 스텟 설정
            boMonster.level = 1;
            boMonster.actorType = ActorType.Monster;

            var sdMonster = boMonster.sdMonster;
            boMonster.atkType = sdMonster.atkType;
            boMonster.moveSpeed = sdMonster.moveSpeed;
            boMonster.currentHp = boMonster.maxHp = sdMonster.maxHp;
            boMonster.atkRange = sdMonster.atkRange;
            boMonster.atkInterval = sdMonster.atkInterval;
            boMonster.atk = sdMonster.atk;
            boMonster.def = sdMonster.def;
        }

        /// <summary>
        /// => 배틀 매니저를 통해서 호출되는 메서드
        /// </summary>
        public override void ActorUpdate()
        {
            // -> 플레이어 탐지
            CheckDetection();

            base.ActorUpdate();
        }

        public override void MoveUpdate()
        {
            // -> 움직임 여부 판단
            var isMove = GetMovement();

            if (isMove)
            {
                // -> 몬스터 이동 애니메이션 실행
                SetState(ActorState.Walk);

                // -> 이동속도 설정
                agent.speed = boMonster.moveSpeed;

                // -> 목적지 설정
                agent.SetDestination(destPos);
            }
            else
            {
                SetState(ActorState.Idle);
            }
        }

        /// <summary>
        /// => 에이전트의 움직임 여부를 판단해주는 메서드
        /// </summary>
        /// <returns></returns>
        private bool GetMovement()
        {
            // -> 타겟이 존재한다면
            if (attackController.hasTarget)
            {
                // -> 평소에는 False이다가 대상을 만나면 True
                return !attackController.canAtk;
            }

            // -> 멈춰있다면
            if (State == ActorState.Idle)
            {
                currentPatrolWaitTime += Time.deltaTime;

                // -> 최대 대기시간보다 크거나 같다면
                if (currentPatrolWaitTime >= patrolWaitTime)
                {
                    InitPatrolWaitTime();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// => Vector3.magnitude : 벡터의 길이를 반환한다
            // -> 목적지와 현재 오브젝트의 사이거리 값
            var distance = (destPos - transform.position).magnitude;

            /// => stoppingDistance : 목표 위치에 가까워졌을 시, 정지하는 거리 값
            // -> 사이거리 값이 정지하는 거리 값보다 작거나 같다면
            if (distance <= agent.stoppingDistance)
            {
                // -> 목적지에 거의 도달 했다는 의미 이므로 목적지 재설정
                ChangeDestPos();
                return false;
            }

            return true;
        }

        /// <summary>
        /// => 에이전트의 목적지를 설정해주는 메서드
        /// </summary>
        private void ChangeDestPos()
        {
            // -> 몬스터마다 스폰 구역이 다르므로
            //    몬스터의 인덱스 값을 넘겨 해당 몬스터의 스폰 구역 내에서 랜덤한 위치를 반환 한다
            destPos = StageManager.Instance.GetRandPosInArea(boMonster.sdMonster.index);

            /// => CalculatePath : 에이전트 위치에서 목적지(destPos)까지의 거리중 최단 거리를 계산하고
            ///                    NavMeshPath 타입의 데이터를 path에 저장한다
            ///                    (정확히는 NavMeshPath 안에 있는 Vector3[] corners에 최단 경로의 코너들의 위치 벡터가 저장됨 )
            var isExist = agent.CalculatePath(destPos, path);

            // -> 경로가 존재 하지 않는다면
            if (!isExist)
            {
                // -> 다시 설정
                ChangeDestPos();
            }
            /// => NavMeshPathStatus.PathPartial : 해당 경로가 목적지에 도달 할 수 없는 상태를 나타낸다
            // -> 도달 할 수 없는 Path라면
            else if (path.status == NavMeshPathStatus.PathPartial)
            {
                // -> 다시 설정
                ChangeDestPos();
            }
        }

        /// <summary>
        /// => 대기 시간을 초기화 해주는 메서드
        /// </summary>
        private void InitPatrolWaitTime()
        {
            // -> 현재 대기시간을 다시 초기화
            currentPatrolWaitTime = 0;

            // -> 최대 대기시간을 랜덤 범위로 설정
            patrolWaitTime = Random.Range(Define.Monster.MinPatrolWaitTime, Define.Monster.MaxPatrolWaitTime);
        }

        /// <summary>
        /// => 몬스터가 플레이어를 감지하도록 하는 메서드
        /// </summary>
        private void CheckDetection()
        {
            // -> 감지범위 설정
            var extentsValue = boMonster.sdMonster.detectionRange;
            var halfExtents = new Vector3(extentsValue, extentsValue, extentsValue);

            /// => OverlapBox : 센터(transform.position)에서 사이즈(halfExtents)에 transform.rotation만큼 회전한 상자에 충돌한
            ///                 콜라이더를 전부 반환한다
            // -> 오버랩박스를 이용하여 플레이어를 감지
            var colls = Physics.OverlapBox(transform.position, halfExtents, transform.rotation,
                1 << LayerMask.NameToLayer("Player"));

            // -> 감지한 플레이어가 없다면
            if (colls.Length == 0)
            {
                attackController.hasTarget = false;
                return;
            }

            // -> 타겟 존재
            attackController.hasTarget = true;

            // -> 에이전트의 목적지(destPos)를 제일 처음으로 충돌한 콜라이더의 객체 포지션으로 설정
            destPos = colls[0].transform.position;

            var distance = destPos - transform.position;

            /// => Vector3.sqrMagnitude : 벡터의 길이를 제곱한 값을 반환함
            ///                           (크기의 제곱을 구하는 것이 Vector3.magnitude보다 훨씬 빠르다)
            // -> 타겟과의 거리가 공격범위보다 작거나 같다면
            if (distance.sqrMagnitude <= boActor.atkRange)
            {
                // -> 범위 안에 존재하므로 공격이 가능하고
                //    현재 오브젝트가 타겟을 바라보도록 방향벡터(distance.normalized)를 이용하여 설정
                attackController.canAtk = true;
                transform.rotation = Quaternion.LookRotation(distance.normalized);
            }
            else
            {
                attackController.canAtk = false;
            }
        }

        #region 애니메이션 이벤트

        public override void OnAttackHit()
        {
            // -> 공격 실행
            attackController.OnAttack();
        }

        public override void OnAttackEnd()
        {
            // -> 공격이 끝났으므로 쿨타임 체크 가능여부는 true
            attackController.canCheckCoolTime = true;
            anim.SetBool(monAnim.isAttack, false);
            SetState(ActorState.Idle);
        }

        /// <summary>
        /// => 몬스터가 죽었을때 애니메이션에서 실행할 이벤트 함수
        /// </summary>
        public override void OnDeadEnd()
        {
            destPos = transform.position;

            StageManager.Instance.huntedMon++;

            #region 퀘스트 디테일 작업

            // -> 현재 진행중인 퀘스트 목록을 가져 옵니다!
            var progressQuests = GameManager.User.boQuest.progressQuests;

            // -> 현재 몬스터를 타겟으로 하는 진행중인 퀘스트의 인덱스 값을 담을 변수 입니다!
            var questIndex = -1;

            // -> 현재 몬스터를 타겟으로 하는 진행 퀘스트 찾기 작업 입니다!
            for (int i = 0; i < progressQuests.Count; i++)
            {
                // -> 진행중인 퀘스트의 타겟 인덱스와 현재 몬스터의 인덱스가 같다면!
                if (progressQuests[i].sdQuest.target.Where(obj => obj == boMonster.sdMonster.index)?.SingleOrDefault()
                    == boMonster.sdMonster.index)
                {
                    // -> 현재 몬스터를 타겟으로 하는 퀘스트의 위치 값 입니다!
                    questIndex = i;
                    break;
                }
            }

            if (questIndex != -1)
            {
                var boProgressQuest = progressQuests[questIndex];

                // -> 진행중인 퀘스트를 찾았으니 현재 몬스터의 디테일 값을 찾는 작업
                for (int i = 0; i < boProgressQuest.sdQuest.target.Length; i++)
                {
                    if (boProgressQuest.sdQuest.target[i] == boMonster.sdMonster.index)
                    {
                        // -> 디테일 값이 퀘스트의 디테일 값을 넘어가지 않도록 막아 줍니다!
                        boProgressQuest.details[i] = boProgressQuest.details[i] >= boProgressQuest.sdQuest.questDetail[i] ?
                            boProgressQuest.sdQuest.questDetail[i] : boProgressQuest.details[i] += 1;

                        // -> Bo데이터 변했으므로 Dto데이터에 다시 저장하는 작업
                        var dummyServer = DummyServer.Instance;
                        //dummyServer.userData.dtoQuest.progressQuests[questIndex].details = 
                        //    (int[])boProgressQuest.details.Clone();
                        dummyServer.userData.dtoQuest.progressQuests[questIndex].details =
                            boProgressQuest.details;

                        // -> 어차피 Dto에 있는 데이터를 Bo에 저장했었기 때문에 현재 퀘스트가 존재하는 인덱스는 둘다 같다고 생각
                        dummyServer.Save();
                        break;
                    }
                }
            }

            #endregion

            #region 몬스터 아이템 작업

            var itemIndex = Random.Range(0, boMonster.sdMonster.dropItemPer.Length);
            var itemNumber = boMonster.sdMonster.dropItemRef[itemIndex];
            var sdItem = GameManager.SD.sdItems.Where(obj => obj.index == itemNumber)?.SingleOrDefault();

            var itemPool = ObjectPoolManager.Instance.GetPool<Item>(PoolType.Item);

            // -> 아이템의 부모를 찾는 작업
            if (GameObject.Find("ItemHolder"))
            {
                itemHolder = GameObject.Find("ItemHolder");
            }
            else
            {
                itemHolder = new GameObject("ItemHolder");
            }

            for (int i = 0; i < boMonster.sdMonster.dropItemRef.Length; i++)
            {
                if (Random.Range(0, boMonster.sdMonster.dropItemPer[itemIndex]) < .15f)
                {
                    var item = itemPool.GetPoolableObject();
                    item.transform.SetParent(itemHolder.transform);
                    item.transform.localScale = Vector3.one;
                    item.transform.position = this.transform.position - new Vector3(0, .4f, 0);
                    item.gameObject.SetActive(true);
                    item.Initialize(itemNumber);
                    break;
                }
            }

            #endregion

            ObjectPoolManager.Instance.GetPool<Monster>(PoolType.Monster).ReturnPoolableObject(this);
        }

        #endregion

        /*
        private void OnDrawGizmos()
        {
            var extentsValue = boMonster.sdMonster.detectionRange;
            var halfExtents = new Vector3(extentsValue, extentsValue, extentsValue);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, halfExtents);
        }
        */
    }
}