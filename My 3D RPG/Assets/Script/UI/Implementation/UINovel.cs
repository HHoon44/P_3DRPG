 using ProjectChan.Resource;
using ProjectChan.SD;
using ProjectChan.UI;
using ProjectChan.Util;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ProjectChan.Define.Actor;

namespace ProjectChan.UI
{
    /// <summary>
    /// �뺧 UI�� �����ϴ� Ŭ����
    /// </summary>
    public class UINovel : UIWindow
    {
        // public
        public TextMeshProUGUI speakerName;     // ���� ���ϴ� ĳ���� �̸�
        public TextMeshProUGUI dialogue;        // ���� ���ϴ� ĳ������ ���
        public Image portrait;                  // ���� ���ϴ� ĳ������ �̹���

        // private
        private AspectRatioFitter arf;          // �ʻ�ȭ ũ�⸦ ����

        public override void Start()
        {
            base.Start();
            arf = transform.Find("PortraitHolder").GetChild(0).GetComponent<AspectRatioFitter>();
        }

        /// <summary>
        /// �Ķ���ͷ� ���� Bo�����ͷ� ���� UI�� �����ϴ� �޼���
        /// </summary>
        /// <param name="boNovel"> �뺧 ��ȹ �����Ͱ� ��� ������ </param>
        public void SetNovel(BoNovel boNovel)
        {
            // �̸�, ��ȭ ����
            speakerName.text = boNovel.sdNovel.name;
            dialogue.text = boNovel.sdNovel.kr;

            switch (boNovel.sdNovel.charType)
            {
                case CharType.NovelChar:
                case CharType.NPC:
                    // ��ȭ�ϴ� ĳ���� Ÿ�Ժ��� aspectRatio ����� ����
                    SetImageSize(boNovel.sdNovel.charType);

                    // �ʻ�ȭ�� �����Ѵٸ�
                    if (boNovel.sdNovel.portraitPath.Length > 1)
                    {
                        // �����̼� ������ ���� �̹����� ���İ��� 0���� ���� �Ǿ��ִٸ�
                        if (portrait.color.a == 0)
                        {
                            portrait.color = Color.white;
                        }
                        portrait.sprite = SpriteLoader.GetSprite
                            (Define.Resource.AtlasType.Portrait, boNovel.sdNovel.portraitPath);
                    }
                    break;
                case CharType.Narration:
                    portrait.color = new Color(1, 1, 1, 0);
                    break;
            }

            Open();
        }

        /// <summary>
        /// AspectRatioFitter�� Ratio ����� �����ϴ� �޼���
        /// </summary>
        /// <param name="charType"> Ÿ�Ժ��� ������ ����� Ÿ�Ե����� </param>
        private void SetImageSize(CharType charType)
        {
            switch (charType)
            {
                case CharType.NovelChar:
                    arf.aspectRatio = Define.StaticData.BaseNovelSize;
                    break;
                case CharType.NPC:
                    arf.aspectRatio = Define.StaticData.NPCNovelSize;
                    break;
            }
        }
    }
}