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
            OnPhase(introPhase);
        }

        /// <summary>
        /// => 현재 페이즈에 대한 로직 실행
        /// </summary>
        /// <param name="phase"> 진행시키고자 하는 현재 페이즈 </param>
        private void OnPhase(IntroPhase phase)
        {
            uiStart.SetLoadStateDescription(phase.ToString());

            if (loadGaugeUpdateCoroutine != null)
            {
                StopCoroutine(loadGaugeUpdateCoroutine);
                loadGaugeUpdateCoroutine = null;
            }

            if (phase != IntroPhase.Complete)
            {
                var loadPer = (float)phase / (float)IntroPhase.Complete;

                loadGaugeUpdateCoroutine = StartCoroutine(uiStart.LoadGaugeUpdate(loadPer));
            }
            else
            {
                /// 완료 되었다면 Image의 fillAmount값을 1로 설정해준다
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
                    SceneManager.LoadScene(SceneType.NovelGame.ToString());
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