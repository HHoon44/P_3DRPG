using ProjectChan.DB;
using ProjectChan.Novel;
using ProjectChan.Resource;
using ProjectChan.SD;
using ProjectChan.UI;
using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ProjectChan.Define.Resource;

namespace ProjectChan.Novel
{
    public class NovelController : MonoBehaviour
    {
        public Image novelGround;           // -> Novel의 뒷 배경
        private UINovelSet uiNovelSet;      // -> UINovelSet 스크립트
        private SDNovel sdNovel;            // -> 현재 진행할 이야기 데이터

        /// <summary>
        /// => 이야기의 시작이 되는 메서드
        /// </summary>
        private void OnTalse()
        {
            var gameManager = GameManager.Instance;
            var uiWindowManager = UIWindowManager.Instance;
            uiNovelSet = uiWindowManager.GetWindow<UINovelSet>();

            // -> 게임매니저가 들고있는 이야기 인덱스에 이야기진행인덱스를 더한 값과
            //    같은 인덱스를 지닌 Talse기획 데이터를 가져옴
            sdNovel = GameManager.SD.sdNovels.Where(obj => obj.index ==
            gameManager.currentTalseIndex + gameManager.speechIndex)?.SingleOrDefault();

            // -> 이야기 하는 캐릭터가 없다면 다음 스테이지로
            if (sdNovel.charType == Define.Actor.CharType.None)
            {
                NextStageSet();
                return;
            }

            // -> 가져온 SD데이터로 Bo데이터에 세팅
            var boNovel = new BoNovel(sdNovel);

            // -> 다음 이야기 진행되기 위해서 진행인덱스 증가
            SetNovelGround(boNovel.stagePath);
            uiNovelSet.SetNovel(boNovel);
            gameManager.speechIndex++;
        }

        /// <summary>
        /// => 배경을 설정해주는 메서드
        /// </summary>
        /// <param name="path"> 배경 스프라이트 경로 </param>
        private void SetNovelGround(string path)
        {
            if (path.Length > 1)
            {
                var stage = SpriteLoader.GetSprite(AtlasType.SchoolStage, path);
                novelGround.sprite = stage;
            }
        }

        /// <summary>
        /// => 이야기가 끝나고 다음 스테이지로 넘어가기 위한 세팅을 하는 메서드
        /// </summary>
        private void NextStageSet()
        {
            uiNovelSet.EndNovel();

            var uiInfoSet = UIWindowManager.Instance.GetWindow<UIInfoSet>();

            if (uiInfoSet.isOpen)
            {
                return;
            }

            uiInfoSet.Open();
            uiInfoSet.Initialize(sdNovel);
            SetNovelGround(sdNovel.stagePath);
        }

        private void Update()
        {
            // -> 이게 노벨게임 씬에서만 작동하게 설정해야할듯
            if (Input.GetButtonDown(Define.Input.Interaction))
            {
                OnTalse();
            }

            // 테스트입니다
            if (Input.GetButtonDown(Define.Input.Quest.ToString()))
            {
                OnTalse();
                GameManager.Instance.speechIndex = 16;
            }
        }
    }
}