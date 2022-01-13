using ProjectChan.Resource;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectChan.UI
{
    /// <summary>
    /// => 게임을 시작할 때 UI들을 관리할 클래스
    /// </summary>
    public class UIStart : MonoBehaviour
    {
        public Text loadStateDesc;          // -> 로딩바 텍스트
        public Image loadFillGauge;         // -> 로딩바 이미지
        public Image backImage;             // -> 설정할 백 이미지

        private void Start()
        {
            // -> 백 이미지를 설정하는 작업 입니다!
            ResourceManager.Instance.LoadBackGround();

            var backIndex = Random.Range(1, SpriteLoader.atlasIndex + 1);
            var selectSprite = SpriteLoader.GetSprite(Define.Resource.AtlasType.BackGround, $"Back Ground{backIndex}");

            backImage.sprite = selectSprite;
        }

        public void SetLoadStateDescription(string loadState)
        {
            loadStateDesc.text = $"Load{loadState}...";
        }

        public IEnumerator LoadGaugeUpdate(float loadPer)
        {
            // -> FillAmount값과 파라미터로 받은 loadPer이 같아질 때 까지 반복합니다!
            while (!Mathf.Approximately(loadFillGauge.fillAmount, loadPer))
            {
                loadFillGauge.fillAmount = Mathf.Lerp(loadFillGauge.fillAmount, loadPer, Time.deltaTime * 2f);
                yield return null;
            }
        }
    }
}