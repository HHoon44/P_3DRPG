using ProjectChan;
using ProjectChan.Resource;
using ProjectChan.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ProjectChan.Define.Resource;

namespace ProjectChan.UI
{
    /// <summary>
    /// => 로딩씬의 UI를 컨트롤하는 클래스
    /// </summary>
    public class UILoading : UIWindow
    {
        // public
        public TextMeshProUGUI loadStateDesc;        
        public Image loadGauge;                     
        public Image loadImage;                     
        public Camera cam;                      // -> 로딩씬 카메라          

        // private
        private static string dot = string.Empty;
        private static string loadStateDescription = "로딩 중입니다";

        public override void Start()
        {
            base.Start();

            // -> 뒷 배경을 세팅 해줍니다!
            var backIndex = Random.Range(1, SpriteLoader.atlasIndex + 1);
            var selectSprite = SpriteLoader.GetSprite(AtlasType.BackGround, $"Back Ground{backIndex}");
            loadImage.sprite = selectSprite;
        }

        private void Update()
        {
            if (loadGauge.fillAmount >= .88f)
            {
                loadGauge.fillAmount = 1f;
            }

            loadGauge.fillAmount = Mathf.Lerp(loadGauge.fillAmount, 1f, Time.deltaTime * 2f);

            if (Time.frameCount % 20 == 0)
            {
                if (dot.Length >= 3)
                {
                    dot = string.Empty;
                }
                else
                {
                    dot = string.Concat(dot, ".");
                }

                loadStateDesc.text = $"{loadStateDescription}{dot}";
            }
        }
    }
}