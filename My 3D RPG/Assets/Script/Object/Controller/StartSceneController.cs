using ProjectChan.Define;
using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProjectChan
{
    public class StartSceneController : MonoBehaviour
    {
        public Transform ButtonFrame;       // -> 게임에 관련 버튼이 설정 되어있는 창
        public Transform OptionFrame;       // -> 옵션 버튼을 눌렀을 때 활성화 할 설정 창
        public Transform HelpFrame;         // -> 도움말 버튼을 눌렀을 때 활성화 할 설정 창 
        public Slider slider;               // -> 오디오 볼륨을 조절할 슬라이더

        private AudioManager AM;            // -> 오디오 매니저
        private TextMeshProUGUI helpText;

        private void Start()
        {
            // -> 많이 사용할거 같으니깐 필드로 담아두기
            AM = AudioManager.Instance;

            // -> 오디오 볼륨을 조절하는 슬라이더
            slider.value = AM.audio.volume;

            AM.ChangeAudioClip(Define.Audio.ClipType.StartScene);

            helpText = HelpFrame.GetComponentInChildren<TextMeshProUGUI>();

            var manual = StaticData.Manual.Split(',');

            for (int i = 0; i < manual.Length; i++)
            {
                helpText.text += manual[i] + '\n';
            }
        }

        /// <summary>
        /// => 씬에 존재하는 버튼들을 관리함
        /// </summary>
        public void ButtonOption()
        {
            switch (EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex())
            {
                // -> 새로하기 버튼
                case 0:
                    SceneManager.LoadScene("FirstLoading");
                    break;

                // -> 이어하기 버튼
                case 1:
                    break;

                // -> 옵션 버튼
                case 2:
                    ButtonFrame.gameObject.SetActive(false);
                    OptionFrame.gameObject.SetActive(true);
                    break;

                // -> 도움말 버튼
                case 3:
                    ButtonFrame.gameObject.SetActive(false);
                    HelpFrame.gameObject.SetActive(true);
                    break;
            }
        }

        /// <summary>
        /// => 옵션 프레임을 닫는 버튼의 바인딩 메서드
        /// </summary>
        public void OptionButtoFun()
        {
            OptionFrame.gameObject.SetActive(false);
            ButtonFrame.gameObject.SetActive(true);
        }

        /// <summary>
        /// => 도움말 프레임을 닫는 버튼의 바인딩 이벤트
        /// </summary>
        public void HelpButtonFun()
        {
            HelpFrame.gameObject.SetActive(false);
            ButtonFrame.gameObject.SetActive(true);
        }


        /// <summary>
        /// => 현재 슬라이더 값으로 오디오 볼륨을 조절하는 바인딩 메서드
        /// </summary>
        public void SetAudioVolume()
        {
            AM.SetVolume(slider.value);
        }
    }
}