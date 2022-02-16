using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectChan.DB;
using ProjectChan.Define;
using ProjectChan.Dummy;
using ProjectChan.SD;
using ProjectChan.UI;
using ProjectChan.Util;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ProjectChan.Define.Actor;

namespace ProjectChan
{
    /// <summary>
    /// => 게임에 사용하는 모든 데이터를 관리하는 클래스
    /// => 추가로 게임의 씬 변경등과 같은 큰 흐름들을 컨트롤하기도 함
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        public bool useDummyServer;                             // -> DummyServer을 사용하고 있는가?
        public float loadProgress;                              // -> 다음씬이 얼마나 준비되었는지에 대한 값

        /// <summary>
        /// => 기획 데이터를 관리함
        /// </summary>
        [SerializeField]
        private StaticDataModule sd = new StaticDataModule();
        public static StaticDataModule SD => Instance.sd;       

        /// <summary>
        /// => 유저 데이터를 관리함
        /// </summary>
        private BoUser boUser = new BoUser();
        public static BoUser User => Instance.boUser;          

        protected override void Awake()
        {
            base.Awake();

            if (gameObject == null)
            {
                return;
            }

            // -> 씬이 변경되어도 객체가 파괴되지 않도록 합니다!
            DontDestroyOnLoad(this);

            var StartController = FindObjectOfType<StartController>();
            StartController?.Initialize();
        }

        /// <summary>
        /// => 앱의 기본 설정입니다!
        /// </summary>
        public void OnApplicationSetting()
        {
            // -> 수직 동기화를 꺼줍니다!
            QualitySettings.vSyncCount = 0;

            // -> 랜덤 프래임을 60으로 설정합니다!
            Application.targetFrameRate = 60;

            // -> 앱 실행 중 장시간 대기 시에도 화면이 꺼지지 않게합니다!
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        /// <summary>
        /// => 씬을 이동하는 메서드
        /// </summary>
        /// <param name="sceneName"> 이동할 씬의 이름 </param>
        /// <param name="loadCoroutine"> StageManager.OnChangeStage </param>
        /// <param name="loadComplete"> StageManager.OnChangeStageComplete </param>
        public void LoadScene(SceneType sceneName, IEnumerator loadCoroutine = null, Action loadComplete = null)
        { 
            StartCoroutine(WaitForLoad());

            IEnumerator WaitForLoad()
            {
                // -> 로딩 진행 상태를 나타냅니다!
                loadProgress = 0;

                // -> 비동기를 이용해서 로딩 씬을 전환 합니다!
                yield return SceneManager.LoadSceneAsync(SceneType.Loading.ToString());

                // -> 변경하고자 하는 씬을 담습니다!
                var asyncOper = SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Additive);

                // -> 방금 추가한 씬을 비활성화 합니다!
                asyncOper.allowSceneActivation = false;

                // -> 변경하고자 하는 씬에 필요한 작업이 있다면!
                if (loadCoroutine != null)
                {
                    // -> 해당 작업이 완료될 때 까지 대기합니다!
                    yield return StartCoroutine(loadCoroutine);
                }

                // -> 비동기로 로드한 씬이 활성화가 완료되지 않았따면 특정 작업을 반복합니다!
                while (!asyncOper.isDone)
                {
                    if (loadProgress >= .9f)
                    {
                        loadProgress = 1f;

                        yield return new WaitForSeconds(1f);

                        asyncOper.allowSceneActivation = true;
                    }
                    else
                    {
                        // -> loadProgress 값을 이용해서 사용자한테 로딩바를 통해 진행 상태를 알려줍니다!
                        loadProgress = asyncOper.progress;
                    }

                    // -> 코루틴 내에서 반복문 사용 시 로직을 한 번 실행 후 메인 로직을 실행할 수 있게 yield return을 해줍니다!
                    yield return null;
                }

                // -> 로딩 씬에서 다음 씬에 필요한 작업을 전부 수행 했으므로 로딩씬을 비활성화 시킵니다!
                yield return SceneManager.UnloadSceneAsync(SceneType.Loading.ToString());

                loadComplete?.Invoke();
                UIWindowManager.Instance.GetWindow<UIBattle>().Open();
            }
        }

        /// <summary>
        /// => 로딩씬을 이용하여 실제 씬을 이동하는 것처럼 보이게 해주는 메서드
        /// => 로딩씬이 실행되는 동안 필요한 리소스들을 불러오는 작업을 함
        /// </summary>
        /// <param name="loadCoroutine"> StageManager.ChangeStage </param>
        /// <param name="loadComplete"> StageManager.OnChangeStageComplete </param>
        public void OnAddtiveLoadingScene(IEnumerator loadCoroutine = null, Action loadComplete = null)
        {
            StartCoroutine(WaitForLoad());

            IEnumerator WaitForLoad()
            {
                loadProgress = 0;

                var asyncOper = SceneManager.LoadSceneAsync(SceneType.Loading.ToString(), LoadSceneMode.Additive);

                #region 로딩바 진행상태 처리

                while (!asyncOper.isDone)
                {
                    loadProgress = asyncOper.progress;
                    yield return null;
                }

                UILoading uiLoading = null;

                while (uiLoading == null)
                {
                    uiLoading = UIWindowManager.Instance.GetWindow<UILoading>();
                    yield return null;
                }

                // -> 인게임씬에서 활성화된 상태로 카메라가 존재하기 때문에 로딩씬 카메라를 비활성화 해줍니다!
                uiLoading.cam.enabled = false;
                loadProgress = 1f;

                #endregion

                yield return new WaitForSeconds(.5f);

                #region 스테이지 전환 시 필요한 작업

                if (loadCoroutine != null)
                {
                    yield return StartCoroutine(loadCoroutine);
                }

                #endregion

                yield return new WaitForSeconds(.5f);

                #region 스테이지 전환 완료 후 실행할 작업

                yield return SceneManager.UnloadSceneAsync(SceneType.Loading.ToString());
                loadComplete?.Invoke();

                #endregion
            }
        }
    }
}