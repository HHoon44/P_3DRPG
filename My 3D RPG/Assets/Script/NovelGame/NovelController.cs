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
    /// <summary>
    /// => 노벨 게임씬을 관리하는 컨트롤러 메서드
    /// </summary>
    public class NovelController : MonoBehaviour
    {
        // public
        public Image novelGround;           // -> 뒷 배경

        // private
        private int speechIndex;            // -> 현재 대화 진행도 인덱스
        private int currentNovelIndex;      // -> 현재 대화 인덱스
        private SDNovel sdNovel;            // -> 현재 진행할 대화 데이터

        private void Awake()
        {
            currentNovelIndex = Define.Novel.firstNovelIndex;
            speechIndex = 0;
        }

        private void Start()
        {
            // -> 오디오를 노벨 게임 오디오로 변경
            AudioManager.Instance.ChangeAudioClip(Define.Audio.ClipType.NovelGame);

            // -> 노벨 시작
            OnTalse();
        }

        /// <summary>
        /// => 노벨을 시작하는 메서드
        /// </summary>
        private void OnTalse()
        {
            // -> 다음 스테이지로 넘어가기 위한 값보다 작다면!
            if (currentNovelIndex + speechIndex < Define.Novel.nextStageLoadIndex)
            {
                sdNovel = GameManager.SD.sdNovels.Where(obj => obj.index == currentNovelIndex + speechIndex)?.SingleOrDefault();

                // -> 노벨 기획 데이터로 새로운 Bo데이터를 만듭니다!
                BoNovel boNovel = new BoNovel(sdNovel);

                // -> 대화를 하기 위한 세팅을 합니다! (배경설정, 노벨 세팅, 대화 인덱스 증가)
                SetNovelGround(boNovel.sdNovel.stagePath);

                UIWindowManager.Instance.GetWindow<UINovel>().SetNovel(boNovel);
                speechIndex++;
            }
            else
            {
                // -> 다음 스테이지로 넘어갑니다!
                NextStageLoad();
            }
        }

        /// <summary>
        /// => 노벨 배경을 설정하는 메서드
        /// </summary>
        /// <param name="path"> 배경 경로 </param>
        private void SetNovelGround(string path)
        {
            // -> 경로가 존재한다면!
            if (path.Length > 1)
            {
                var stage = SpriteLoader.GetSprite(AtlasType.SchoolImage, path);
                novelGround.sprite = stage;
            }
        }

        /// <summary>
        /// => 노벨 캐릭터가 더이상 없을 때 다음 스테이지로 넘어갈 세팅을 하는 메서드
        /// </summary>
        private void NextStageLoad()
        {
            // -> 노벨 창을 꺼줍니다!
            UIWindowManager.Instance.GetWindow<UINovel>().Close();

            var stageManager = StageManager.Instance;
            
            // -> 다음 씬을 불러옵니다!
            GameManager.Instance.LoadScene
                (Define.SceneType.InGame, stageManager.ChangeStage(), stageManager.OnChangeStageComplete);
        }

        private void Update()
        {
            if (Input.GetButtonDown(Define.Input.Interaction))
            {
                OnTalse();
            }
        }
    }
}