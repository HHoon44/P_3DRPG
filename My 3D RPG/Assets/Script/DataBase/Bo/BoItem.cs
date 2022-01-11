using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// => 클라이언트 내에서 사용될 Item 데이터
    /// => 작업과정에서 데이터를 확인하기 위해서 Serializable
    /// </summary>
    [Serializable]
    public class BoItem
    {
        public int slotIndex;       // -> 아이템이 존재하는 슬롯 인덱스
        public int amount;          // -> 아이템의 현재 개수
        public SDItem sdItem;       // -> 아이템이 지닌 기획 데이터 정보

        /// <summary>
        /// => 서버에서 아이템 정보를 받아와 아이템을 세팅할 때 
        /// => 플레이어가 처음 습득하는 아이템일 때 
        /// => 플레이어가 소지한 아이템을 습득 했을 때 사용하는 메서드
        /// </summary>
        /// <param name="sdItem"> 저장할 아이템의 데이터 정보 </param>
        public BoItem(SDItem sdItem)
        {
            slotIndex = -1;
            amount = 1;
            this.sdItem = sdItem;
        }

        /// <summary>
        /// => 객체 복사를 해주는 메서드
        /// </summary>
        /// <returns></returns>
        public BoItem ObjcetCopy()
        {
            /// => MemberwiseClone : Object 클래스의 protected 멤버 함수이며 객체 복사를 위한 함수
            var clone = (BoItem)this.MemberwiseClone();

            return clone;
        }
    }

    /// <summary>
    /// => 장비 아이템의 옵션을 지닌 클래스
    /// </summary>
    [Serializable]
    public class BoEquipment : BoItem
    {
        public int reinforceValue;      // -> 장비 아이템의 강화 수치
        public bool isEquip;            // -> 장비 아이템 착용 여부

        /// <summary>
        /// => 아이템 정보가 장비 아이템일 때 장비 아이템 정보를 담아놓는 메서드
        /// </summary>
        /// <param name="sdItem"> 저장할 아이템의 데이터 정보 </param>
        public BoEquipment(SDItem sdItem) : base(sdItem)
        {
            reinforceValue = 0;
            isEquip = false;    
        }
    }
}