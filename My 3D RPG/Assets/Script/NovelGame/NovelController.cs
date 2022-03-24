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
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static ProjectChan.Define.Resource;

namespace ProjectChan.Novel
{
    /// <summary>
    /// 노벨 게임을 관리하는 컨트롤러 메서드
    /// </summary>
    public class NovelController : MonoBehaviour
    {
        // public
        public Image novelGround;           // 노벨 씬 배경

        // private
        private int currentNovelIndex;      // 현재 대화 인덱스
        private int speechIndex;            // 현재 대화 진행도 인덱스
        private SDNovel sdNovel;            // 현재 대화 기획 데이터

        private void Awake()
        {
            currentNovelIndex = Define.Novel.firstNovelIndex;
            speechIndex = 0;
        }

        private void Start()
        {
            // 노벨 씬에 맞는 오디오 클립으로 설정
            AudioManager.Instance.ChangeAudioClip(SceneManager.GetActiveScene().name);

            // 대화를 시작
            OnTalse();
        }

        /// <summary>
        /// 대화를 시작하는 메서드
        /// </summary>
        private void OnTalse()
        {
            // 모든 대화가 안 끝났다면
            if (currentNovelIndex + speechIndex < Define.Novel.nextStageLoadIndex)
            {
                // 현재 대화 인덱스와 대화 진행 인덱스를 더한 값과 같은 인덱스의 기획 데이터를 가져옴
                sdNovel = GameManager.SD.sdNovels.Where(obj => obj.index == currentNovelIndex + speechIndex)?.SingleOrDefault();

                // 노벨 기획 데이터로 새로운 Bo데이터를 만듬
                BoNovel boNovel = new BoNovel(sdNovel);

                // 노벨 씬 배경을 설정한다
                if (boNovel.sdNovel.stagePath.Length > 1)
                {
                    var stage = SpriteLoader.GetSprite(AtlasType.SchoolImage, boNovel.sdNovel.stagePath);
                    novelGround.sprite = stage;
                }

                // 노벨 씬의 노벨 UI를 세팅
                UIWindowManager.Instance.GetWindow<UINovel>().SetNovel(boNovel);
                speechIndex++;
            }
            else
            {
                // 다음 스테이지로
                NextStageLoad();
            }
        }

        /// <summary>
        /// 대화가 끝났을 때, 다음 스테이지로 넘어가도록 하는 메서드
        /// </summary>
        private void NextStageLoad()
        {
            // 현재 노벨 UI를 닫음
            UIWindowManager.Instance.GetWindow<UINovel>().Close();

            var stageManager = StageManager.Instance;
            
            // 게임 매니저를 이용해서 다음 씬을 불러옴
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