using ProjectChan.Resource;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectChan.UI
{
    /// <summary>
    /// => ������ ������ �� UI���� ������ Ŭ����
    /// </summary>
    public class UIStart : MonoBehaviour
    {
        // pulbic
        public Text loadStateDesc;          // -> �ε��� �ؽ�Ʈ
        public Image loadFillGauge;         // -> �ε��� �̹���
        public Image backImage;             // -> ������ �� �̹���
        private void Start()
        {
            // -> �� �̹����� �����ϴ� �۾� �Դϴ�!
            ResourceManager.Instance.LoadBackGround();

            var backIndex = Random.Range(1, SpriteLoader.atlasIndex + 1);
            var selectSprite = SpriteLoader.GetSprite(Define.Resource.AtlasType.BackGround, $"Back Ground{backIndex}");

            backImage.sprite = selectSprite;
        }

        #region Public Method

        /// <summary>
        /// => ���� ���� ���¸� �˷��� ��� �ڷ�ƾ
        /// </summary>
        /// <param name="loadState"></param>
        public void SetLoadStateDescription(string loadState)
        {
            loadStateDesc.text = $"Load{loadState}...";
        }

        /// <summary>
        /// => ���� ���� ���¸� �������� ǥ���� �ڷ�ƾ
        /// </summary>
        /// <param name="loadPer"> ���� ���� ���� </param>
        /// <returns></returns>
        public IEnumerator LoadGaugeUpdate(float loadPer)
        {
            // -> FillAmount���� �Ķ���ͷ� ���� loadPer�� ������ �� ���� �ݺ��մϴ�!
            while (!Mathf.Approximately(loadFillGauge.fillAmount, loadPer))
            {
                loadFillGauge.fillAmount = Mathf.Lerp(loadFillGauge.fillAmount, loadPer, Time.deltaTime * 2f);
                yield return null;
            }
        }

        #endregion
    }
}