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
    public class StartController : MonoBehaviour
    {
        private bool allLoaded;                                 // -> 로드가 모두 되었는지
        private IntroPhase introPhase = IntroPhase.Start;       // -> 현재 페이즈
        private Coroutine loadGaugeUpdateCoroutine;             // -> 로드 하는 동안 로드바 게이지를 채울 코루틴
        private bool loadComplete;                              // -> 현재 페이즈의 로드가 되었는지

        public UIStart uiStart;                                 

        public bool LoadComplete
        {
            get => loadComplete;
            set
            {
                loadComplete = value;

                if (loadComplete && !allLoaded)
                {
                    NextPhase();
                }
            }
        }

        /// GameManager가 FindObjectType을 이용하여 StartController를 찾아내서 Initialize를 해줌
        public void Initialize()
        {
            OnPhase(introPhase);
        }

        private void OnPhase(IntroPhase phase)
        {
            /// 현재 진행중인 phase를 text에 띄운다
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