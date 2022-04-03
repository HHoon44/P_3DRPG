using ProjectChan.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.DB
{
    /// <summary>
    /// 캐릭터와 몬스터의 공통된 데이터를 정의하는 클래스
    /// </summary>
    [Serializable]
    public class BoActor
    {
        public int level;                       // 레벨
        public Actor.ActorType actorType;       // 액터 타입
        public Actor.AttackType atkType;        // 공격타입
        public Vector3 moveDir;                 // 이동값
        public Vector3 rotDir;                  // 회전값
        public float moveSpeed;                 // 이동속도
        public float jumpForce;                 // 점프력
        public float currentHp;                 // 현재 체력
        public float maxHp;                     // 최대 체력
        public float currentEnergy;             // 현재 기력
        public float maxEnergy;                 // 최대 기력
        public float atk;                       // 공격력
        public float def;                       // 방어력
        public float atkRange;                  // 공격 범위
        public float atkInterval;               // 공격 쿨타임
        public bool isGround;                   // 현재 땅에 닿은 상태인지?
    }
}
