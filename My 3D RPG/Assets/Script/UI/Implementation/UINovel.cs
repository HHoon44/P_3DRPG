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
    public class UINovel : UIWindow
    {
        private AspectRatioFitter arf;          // -> 초상화 크기 설정

        public TextMeshProUGUI speakerName;     // -> 현재 말하는 캐릭터 이름
        public TextMeshProUGUI dialogue;        // -> 현재 말하는 캐릭터의 대사
        public Image portrait;                  // -> 현재 말하는 캐릭터의 이미지

        public override void Start()
        {
            base.Start();
            arf = transform.Find("PortraitHolder").GetChild(0).GetComponent<AspectRatioFitter>();
        }

        /// <summary>
        /// => 파라미터로 받은 Bo데이터로 현재 UI를 세팅하는 메서드
        /// </summary>
        /// <param name="boNovel"> 노벨 기획 데이터가 담긴 데이터 </param>
        public void SetNovel(BoNovel boNovel)
        {
            // -> 이름, 대화 세팅
            speakerName.text = boNovel.sdNovel.name;
            dialogue.text = boNovel.sdNovel.kr;

            switch (boNovel.sdNovel.charType)
            {
                case CharType.NovelChar:
                case CharType.NPC:
                    // -> 대화하는 캐릭터 타입별로 이미지 사이즈를 정합니다!
                    SetImageSize(boNovel.sdNovel.charType);

                    // -> 초상화가 존재하는 경로가 있다면 !
                    if (boNovel.sdNovel.portraitPath.Length > 1 )
                    {
                        // -> 이전 나레이션 때문에 현재 알파값이 0이라면!
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
        /// => AspectRatioFitter의 Ratio 사이즈를 설정하는 메서드
        /// </summary>
        /// <param name="charType"> 타입별로 나눌때 사용할 타입데이터 </param>
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

        /// <summary>
        /// => 노벨 창을 닫아주는 메서드 (컽)
        /// </summary>
        public void EndNovel()
        {
            Close();
        }
    }
}