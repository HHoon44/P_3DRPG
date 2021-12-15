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
    public class UINovelSet : UIWindow
    {
        public TextMeshProUGUI speakerName;     // -> ���� ���ϴ� ĳ���� �̸�
        public TextMeshProUGUI dialogue;        // -> ���� ���ϴ� ĳ������ ���
        public Image portrait;                  // -> ���� ���ϴ� ĳ������ �̹���
        public Animation portraitAnim;          // -> �ʻ�ȭ �ִϸ��̼�
        private AspectRatioFitter arf;

        public override void Start()
        {
            base.Start();
            arf = transform.Find("PortraitHolder").GetChild(0).GetComponent<AspectRatioFitter>();
        }

        /// <summary>
        /// => ���� Bo�����ͷ� NovelSet�� �����ϴ� �޼���
        /// </summary>
        /// <param name="boNovel"> SDNovel �����͸� ���� Bo������ </param>
        public void SetNovel(BoNovel boNovel)
        { 
            speakerName.text = boNovel.name;
            dialogue.text = boNovel.speeches;

            // -> CharType���� �̹��� ������ ����
            SetImageSize(boNovel.charType);

            // -> �ʻ�ȭ�� �����ϴ� ��ΰ� �ִٸ� 
            if (boNovel.portraitPath.Length > 1)
            {
                portrait.sprite = SpriteLoader.GetSprite
                    (boNovel.atlasType, boNovel.portraitPath + boNovel.currentPortrait.ToString());
            }

            Open();
        }

        /// <summary>
        /// => AspectRatioFitter�� Ratio ����� �����ϴ� �޼���
        /// </summary>
        /// <param name="charType"> Ÿ�Ժ��� ������ ����� Ÿ�Ե����� </param>
        private void SetImageSize(CharType charType)
        {
            switch (charType)
            {
                case CharType.None:
                    break;
                case CharType.Normal:
                    arf.aspectRatio = Define.StaticData.BaseNovelSize;
                    break;
                case CharType.NPC:
                    arf.aspectRatio = Define.StaticData.NPCNovelSize;
                    break;
            }
        }

        /// <summary>
        /// => ���� �����ϴ� UINovelSet�� �ݾ���
        /// </summary>
        public void EndNovel()
        {
            Close();
        }
    }
}