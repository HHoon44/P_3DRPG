using ProjectChan.Define;
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
    /// <summary>
    /// => 플레이어의 체력, 에너지 바를 관리하는 클래스
    /// </summary>
    public class BubbleGauge : MonoBehaviour
    {
        public UIType uiType;   // -> 현재 UI의 타입

        private Image Bar;      // -> 버블 게이지의 이미지 컴포넌트

        private void Start()
        {
            Bar = GetComponent<Image>();
            Bar.sprite = SpriteLoader.GetSprite(AtlasType.UIAtlase, uiType.ToString());
            Bar.type = Image.Type.Filled;
        }

        /// <summary>
        /// => 파라미터로 받는 값을 이용하여 HP게이지를 관리하는 메서드
        /// </summary>
        /// <param name="value"> 게이지에 업데이트 할 값 </param>
        public void SetGauge(float value)
        {
            Bar.fillAmount = value;
        }
    }
}