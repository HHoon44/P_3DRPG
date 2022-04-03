using ProjectChan.NetWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// 서버와 통신에 사용될 Item 데이터
    /// </summary>
    [Serializable]
    public class DtoItem : DtoBase
    {
        /// <summary>
        /// 소지한 아이템 정보
        /// </summary>
        public List<DtoItemElement> dtoItems;  

        public DtoItem() { }

        /// <summary>
        /// 아이템들을 DB에 저장할 때 사용할 메서드
        /// </summary>
        /// <param name="boItems"> 담아놓을 BoItem 데이터들 </param>
        public DtoItem(List<BoItem> boItems)
        {
            dtoItems = new List<DtoItemElement>();

            for (int i = 0; i < boItems.Count; i++)
            {
                dtoItems.Add(new DtoItemElement(boItems[i]));
            }
        }
    }

    /// <summary>
    /// 아이템의 정보를 필드로 지닌 클래스
    /// </summary>
    [Serializable]
    public class DtoItemElement
    {
        public int index;               // 아이템의 기획데이터 인덱스
        public int slotIndex;           // 아이템이 존재하는 슬롯 인덱스
        public int amount;              // 아이템의 개수
        public int reinforceValue;      // 장비 아이템의 강화 수치
        public bool isEquip;            // 장비 아이템의 착용 여부

        public DtoItemElement() { }

        public DtoItemElement(BoItem boItem)
        {
            index = boItem.sdItem.index;
            slotIndex = boItem.slotIndex;
            amount = boItem.amount;

            // 만약 전달받은 아이템이 장비 아이템이라면
            if (boItem is BoEquipment)
            {
                var boEquipment = boItem as BoEquipment;
                reinforceValue =  boEquipment.reinforceValue;
                isEquip = boEquipment.isEquip;
            }
        }
    }
}