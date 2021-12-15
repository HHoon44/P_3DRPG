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
        public TextMeshProUGUI speakerName;     // -> 현재 말하는 캐릭터 이름
        public TextMeshProUGUI dialogue;        // -> 현재 말하는 캐릭터의 대사
        public Image portrait;                  // -> 현재 말하는 캐릭터의 이미지
        public Animation portraitAnim;          // -> 초상화 애니메이션
        private AspectRatioFitter arf;

        public override void Start()
        {
            base.Start();
            arf = transform.Find("PortraitHolder").GetChild(0).GetComponent<AspectRatioFitter>();
        }

        /// <summary>
        /// => 받은 Bo데이터로 NovelSet을 세팅하는 메서드
        /// </summary>
        /// <param name="boNovel"> SDNovel 데이터를 지닌 Bo데이터 </param>
        public void SetNovel(BoNovel boNovel)
        { 
            speakerName.text = boNovel.name;
            dialogue.text = boNovel.speeches;

            // -> CharType별로 이미지 사이즈 설정
            SetImageSize(boNovel.charType);

            // -> 초상화가 존재하는 경로가 있다면 
            if (boNovel.portraitPath.Length > 1)
            {
                portrait.sprite = SpriteLoader.GetSprite
                    (boNovel.atlasType, boNovel.portraitPath + boNovel.currentPortrait.ToString());
            }

            Open();
        }

        /// <summary>
        /// => AspectRatioFitter의 Ratio 사이즈를 설정하는 메서드
        /// </summary>
        /// <param name="charType"> 타입별로 나눌때 사용할 타입데이터 </param>
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
        /// => 씬에 존재하는 UINovelSet을 닫아줌
        /// </summary>
        public void EndNovel()
        {
            Close();
        }
    }
}