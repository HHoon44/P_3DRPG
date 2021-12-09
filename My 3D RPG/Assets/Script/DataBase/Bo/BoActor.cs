using ProjectChan.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.DB
{
    [Serializable]
    public class BoActor
    {
        public int level;                       // -> 플레이어의 레벨
        public Actor.ActorType actorType;       // -> 플레이어의 타입
        public Actor.AttackType atkType;        // -> 플레이어의 공격타입
        public Vector3 moveDir;                 // -> 플레이어의 이동값
        public Vector3 rotDir;                  // -> 플레이어의 회전값
        public float moveSpeed;                 // -> 플레이어의 이동속도
        public float jumpForce;                 // -> 플레이어의 점프력
        public float currentHp;                 // -> 플레이어의 현재 체력
        public float maxHp;                     // -> 플레이어의 최대 체력
        public float currentEnergy;             // -> 플레이어의 현재 기력
        public float maxEnergy;                   // -> 플레이어의 최대 기력
        public float atk;                       // -> 플레이어의 공격력
        public float def;                       // -> 플레이어의 방어력
        public float atkRange;                  // -> 플레이어의 공격 범위
        public float atkInterval;               // -> 플레이어의 공격 쿨타임
        public bool isGround;                   // -> 플레이어가 현재 땅에 닿은 상태인지?
    }
}
