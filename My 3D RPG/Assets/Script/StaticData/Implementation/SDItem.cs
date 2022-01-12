using ProjectChan.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.SD
{
    /// <summary>
    /// => 아이템 기본 세팅에 사용할 기획 데이터
    /// </summary>
    [Serializable]
    public class SDItem : StaticData
    {
        public string name;                     // -> 아이템의 이름
        public ItemType itemType;               // -> 아이템의 타입
        public string[] affectingStats;         // -> 아이템이 영향을 주는 스텟이름
        public string description;              // -> 아이템의 성능 설명
        public string resourcePath;             // -> 아이템의 저장 경로
        public int price;                       // -> 아이템 가격
        public float[] affectingStatsValue;     // -> 아이템이 스텟에 영향을 주는 값
    }
}