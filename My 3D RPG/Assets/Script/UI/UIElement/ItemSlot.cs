using ProjectChan.DB;
using ProjectChan.Define;
using ProjectChan.Resource;
using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectChan.UI
{
    /// <summary>
    /// => 아이템 슬롯이 가지는 데이터
    /// </summary>
    public class ItemSlot : MonoBehaviour
    {
        public SDItem sdItem { get; private set; }                 // -> 상점 슬롯이 지닌 아이템 기획 데이터

        public BoItem BoItem { get; private set; }          // -> 슬롯에 존재하는 아이템 정보

        public Image ItemImage { get; private set; }        // -> 슬롯에 이미지 컴포넌트

        public TMP_Text ItemAmount { get; private set; }    // -> 슬롯에 존재하는 텍스트 컴포넌트

        public void Initialize()
        {
            ItemAmount ??= GetComponentInChildren<TMP_Text>();
            ItemImage ??= transform.GetChild(0).GetComponentInChildren<Image>();

            SetSlot();
        }

        /// <summary>
        /// => 아이템 슬롯 세팅
        /// </summary>
        /// <param name="boItem"> 세팅에 사용할 BoItem 데이터 </param>
        public void SetSlot(BoItem boItem = null)
        {
            BoItem = boItem;

            if (boItem == null)
            {
                ItemAmount.text = " ";
                ItemImage.sprite = null;
                ItemImage.color = new Color(1, 1, 1, 0);
            }
            else
            {
                ItemAmount.text = boItem.amount.ToString();

                var qwe = boItem.sdItem.resourcePath.Remove(0, boItem.sdItem.resourcePath.LastIndexOf('/') + 1);

                ItemImage.sprite = SpriteLoader.GetSprite(Define.Resource.AtlasType.ItemAtlase, qwe);
                ItemImage.color = Color.white;
            }
        }

        public void SetSlot(SDItem sdItem)
        {
            this.sdItem = sdItem;

            ItemAmount.text = sdItem.price.ToString();
            ItemImage.sprite = SpriteLoader.GetSprite(Define.Resource.AtlasType.ItemAtlase, sdItem.resourcePath);
            ItemImage.color = Color.white;
        }

        /// <summary>
        /// => 동일한 아이템을 습득했을 시 텍스트에 아이템 개수를 재설정하는 메서드
        /// </summary>
        /// <param name="boItem"> 개수가 증가된 아이템 데이터 </param>
        public void AmountUpdate(BoItem boItem)
        {
            if (boItem == null)
            {
                return;
            }

            ItemAmount.text = boItem.amount.ToString();
        }
    }
}