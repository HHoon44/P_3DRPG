using ProjectChan.Resource;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ProjectChan.Define.Resource;

public class UIBackGround : MonoBehaviour
{
    private Image image;

    private void Start()
    {
        ResourceManager.Instance.LoadBackGround();

        image = GetComponent<Image>();

        var backIndex = Random.Range(1, SpriteLoader.atlasIndex + 1);
        var selectSprite = SpriteLoader.GetSprite(AtlasType.BackGround, $"Back Ground{backIndex}");

        image.sprite = selectSprite;
    }
}