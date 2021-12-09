using ProjectChan.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static ProjectChan.Define.Resource;

namespace ProjectChan.UI
{
    public class BubbleGauge : MonoBehaviour
    {
        private Image hpBar;    // -> 버블 게이지의 이미지 컴포넌트

        private void Start()
        {
            hpBar = GetComponent<Image>();
            hpBar.sprite = SpriteLoader.GetSprite(AtlasType.UIAtlase, "HpBar");
        }

        /// <summary>
        /// => 파라미터로 받는 값을 이용하여 HP게이지를 관리하는 메서드
        /// </summary>
        /// <param name="value"></param>
        public void SetGauge(float value)
        {
            hpBar.fillAmount = value;
        }
    }
}