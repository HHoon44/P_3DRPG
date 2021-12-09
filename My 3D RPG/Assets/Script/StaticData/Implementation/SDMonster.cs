using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectChan.Define.Actor;

namespace ProjectChan.SD
{
    /// <summary>
    /// => 몬스터의 SD데이터 클래스
    /// </summary>
    [Serializable]
    public class SDMonster : StaticData
    {
        public string name;             // -> 몬스터 이름
        public string resourcePath;     // -> 몬스터 프리팹 경로
        public AttackType atkType;      // -> 몬스터 공격 타입
        public int[] dropItemRef;       // -> 몬스터 드랍 아이템 인덱스
        public float[] dropItemPer;     // -> 몬스터 아이템 드랍 확률
        public float moveSpeed;         // -> 몬스터 이동 속도
        public float detectionRange;    // -> 타겟 감지 범위
        public float atkRange;          // -> 몬스터 공격 범위
        public float atkInterval;       // -> 몬스터 공격 쿨타임
        public float maxHp;             // -> 몬스터 최대 Hp
        public float maxMana;           // -> 몬스터 최대 Mana
        public float atk;               // -> 몬스터 공격력
        public float def;               // -> 몬스터 방어력
    }
}