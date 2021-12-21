using ProjectChan;
using ProjectChan.Resource;
using ProjectChan.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ProjectChan.Define.Resource;

public class UILoading : UIWindow
{
    private static string dot = string.Empty;
    private static string loadStateDescription = "로딩 중입니다";

    public TextMeshProUGUI loadStateDesc;
    public Image loadGauge;
    public Image loadImage;
    public Camera cam;

    public override void Start()
    {
        base.Start();

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