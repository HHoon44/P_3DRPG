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
    public class GameManager : Singleton<GameManager>
    {
        public bool useDummyServer;                             // -> DummyServer을 사용하고 있는가?
        public float loadProgress;                              // -> 다음씬이 얼마나 준비되었는지에 대한 값

        [SerializeField]
        private StaticDataModule sd = new StaticDataModule();
        public static StaticDataModule SD => Instance.sd;       // -> SD데이터 연결고리가 되어주는 프로퍼티

        private BoUser boUser = new BoUser();
        public static BoUser User => Instance.boUser;           // -> BO데이터의 연결고리가 되어주는 프로퍼티

        protected override void Awake()
        {
            base.Awake();

            if (gameObject == null)
            {
                return;
            }

            DontDestroyOnLoad(this);

            var StartController = FindObjectOfType<StartController>();
            StartController?.Initialize();

            /*
            // -> 처음 시작하는 유저라면 유저의 정보를 담을 공간생성
            if (AssetDatabase.LoadAssetAtPath("Assets/Dummy/UserDataSo", typeof(UserDataSo)) == null)
            {
                UserDataSo UserDataSo = ScriptableObject.CreateInstance<UserDataSo>();
                AssetDatabase.CreateAsset(UserDataSo, "Assets/Dummy/UserDataSo.asset");
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(UserDataSo));

                UserDataSo.dtoStage = new DtoStage();
                UserDataSo.dtoStage.lastStageIndex = StartStageInfo.StartStage;
                UserDataSo.dtoStage.lastPosX = StartStageInfo.StartPosX;
                UserDataSo.dtoStage.lastPosY = StartStageInfo.StartPosY;
                UserDataSo.dtoStage.lastPosZ = StartStageInfo.StartPosZ;

                UserDataSo.dtoAccount = new DtoAccount();
                UserDataSo.dtoAccount.nickName = StartStageInfo.CharName;
                UserDataSo.dtoAccount.gold = StartStageInfo.StartGold;

                UserDataSo.dtoCharacter = new DtoCharacter();
                UserDataSo.dtoCharacter.index = StartStageInfo.StartChar;
                UserDataSo.dtoCharacter.level = StartStageInfo.StartLevel;

                DummyServer.Instance.userData = UserDataSo;
            }
             */

        }

        public void OnApplicationSetting()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
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
                loadProgress = 0;

                yield return SceneManager.LoadSceneAsync(SceneType.Loading.ToString());

                var asyncOper = SceneManager.LoadSceneAsync(sceneName.ToString(), LoadSceneMode.Additive);
                asyncOper.allowSceneActivation = false;

                if (loadCoroutine != null)
                {
                    yield return StartCoroutine(loadCoroutine);
                }

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
                        loadProgress = asyncOper.progress;
                    }

                    yield return null;
                }

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

                #region Loading Bar

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

                uiLoading.cam.enabled = false;
                loadProgress = 1f;

                #endregion

                yield return new WaitForSeconds(.5f);

                #region Stage Change

                if (loadCoroutine != null)
                {
                    yield return StartCoroutine(loadCoroutine);
                }

                #endregion

                yield return new WaitForSeconds(.5f);

                #region Stage Complete

                yield return SceneManager.UnloadSceneAsync(SceneType.Loading.ToString());
                loadComplete?.Invoke();

                #endregion
            }
        }
    }
}