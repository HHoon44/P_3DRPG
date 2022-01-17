using ProjectChan.Define;
using ProjectChan.Dummy;
using ProjectChan.NetWork;
using ProjectChan.Resource;
using ProjectChan.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectChan
{
    /// <summary>
    /// => 시작 씬에서 게임 시작 전에 필요한 전반적인 초기화 및 데이터 로드 등을 수행하는 클래스
    /// </summary>
    public class StartController : MonoBehaviour
    {
        public UIStart uiStart;

        private bool allLoaded;                                 // -> 로드가 모두 되었는지
        private IntroPhase introPhase = IntroPhase.Start;       // -> 현재 페이즈
        private Coroutine loadGaugeUpdateCoroutine;             // -> 로드 하는 동안 로드바 게이지를 채울 코루틴
        private bool loadComplete;                              // -> 현재 페이즈의 로드가 되었는지

        /// <summary>
        /// => 외부에서 loadComplete에 접근하기 위한 프로퍼티
        /// => 추가로 현재 페이즈 완료 시 조건에 따라 다음 페이즈로 변경
        /// </summary>
        public bool LoadComplete
        {
            get => loadComplete;
            set
            {
                loadComplete = value;

                // -> 현재 페이즈가 완료 되었고지만 아직 모든 페이즈가 완료가 아니라면!
                if (loadComplete && !allLoaded)
                {
                    NextPhase();
                }
            }
        }

        /// <summary>
        /// => 첫 페이즈를 불러오는 메서드
        /// </summary>
        public void Initialize()
        {
            // -> 유저 데이터를 초기 설정합니다!
            UserManager.Instance.Initialize();
            OnPhase(introPhase);
        }

        /// <summary>
        /// => 현재 페이즈에 대한 로직을 실행하는 메서드
        /// </summary>
        /// <param name="phase"> 진행시키고자 하는 현재 페이즈 </param>
        private void OnPhase(IntroPhase phase)
        {
            // -> 현재 진행 상태를 띄웁니다!
            uiStart.SetLoadStateDescription(phase.ToString());

            // -> 이미 실행중인 코루틴을 또 실행시킨다면?
            if (loadGaugeUpdateCoroutine != null)
            {
                // -> 오류가 발생하므로 
                //    일단 멈춘 후에 새로 변경된 로딩 게이지 퍼센트를 넘겨 코루틴을 다시 시작하게 합니다!
                StopCoroutine(loadGaugeUpdateCoroutine);
                loadGaugeUpdateCoroutine = null;
            }

            // -> 아직 페이즈가 모두 완료되지 않았다면!
            if (phase != IntroPhase.Complete)
            {
                // -> 현재 로딩 게이지 퍼센트를 구해서 코루틴에 보내줍니다!
                var loadPer = (float)phase / (float)IntroPhase.Complete;

                loadGaugeUpdateCoroutine = StartCoroutine(uiStart.LoadGaugeUpdate(loadPer));
            }
            else
            {
                // -> 완료되었다면 로딩 게이지를 완료 게이지 퍼센트로 설정해줍니다!
                uiStart.loadFillGauge.fillAmount = 1f;
            }

            switch (phase)
            {
                case IntroPhase.Start:
                    LoadComplete = true;
                    break;

                case IntroPhase.ApplicationSetting:
                    GameManager.Instance.OnApplicationSetting();
                    LoadComplete = true;
                    break;

                case IntroPhase.Server:
                    DummyServer.Instance.Initialize();
                    ServerManager.Instance.Initialize();
                    LoadComplete = true;
                    break;

                case IntroPhase.StaticData:
                    GameManager.SD.Initialize();
                    LoadComplete = true;
                    break;

                case IntroPhase.UserData:
                    new LoginHandler().Connect();
                    break;

                case IntroPhase.Resource:
                    ResourceManager.Instance.Initialize();
                    LoadComplete = true;
                    break;

                case IntroPhase.UI:
                    UIWindowManager.Instance.Initialize();
                    LoadComplete = true;
                    break;

                case IntroPhase.Complete:
                    switch (UserManager.Instance.currentGameType)
                    {
                        case NewPlayerData.GameType.New:
                            SceneManager.LoadScene(SceneType.NovelGame.ToString());
                            break;

                        case NewPlayerData.GameType.Continue:
                            GameManager.Instance.LoadScene
                                (SceneType.InGame, StageManager.Instance.ChangeStage(), StageManager.Instance.OnChangeStageComplete);
                            break;
                    }

                    allLoaded = true;
                    LoadComplete = true;
                    break;
            }
        }

        /// <summary>
        /// => 페이즈를 다음 페이즈로 변경하는 메서드
        /// </summary>
        private void NextPhase()
        {
            StartCoroutine(WatiForSeconds());

            IEnumerator WatiForSeconds()
            {
                yield return new WaitForSeconds(.5f);
                LoadComplete = false;
                OnPhase(++introPhase);
            }
        }
    }
}