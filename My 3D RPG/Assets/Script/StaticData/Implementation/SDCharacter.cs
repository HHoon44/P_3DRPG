using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectChan.Define;
using static ProjectChan.Define.Actor;

namespace ProjectChan.SD
{
    /// <summary>
    /// => 기본 캐릭터의 SD 데이터 클래스
    /// </summary>
    [Serializable]
    public class SDCharacter : StaticData
    {
        public string name;             // -> 캐릭터의 이름
        public string resourcePath;     // -> 캐릭터 프리팹이 저장된 위치
        public AttackType atkType;      // -> 캐릭터의 공격 타입
        public float moveSpeed;         // -> 캐릭터의 이동속도
        public float jumpForce;         // -> 캐릭터의 점프력
        public float atkRange;          // -> 캐릭터의 공격범위
        public float atkInterval;       // -> 캐릭터의 공격 쿨타임
    }
}