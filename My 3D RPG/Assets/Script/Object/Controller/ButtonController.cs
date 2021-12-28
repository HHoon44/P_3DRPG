﻿using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ProjectChan
{
    /// <summary>
    /// => 스타트 씬에 존재하는 버튼들을 관리하는 클래스
    /// </summary>
    public class ButtonController : MonoBehaviour
    {
        public Transform OptionFrame;
        public Slider audioSlider;


        private void Start()
        {
            audioSlider.value = AudioManager.Instance.audio.volume;
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
                    OptionFrame.gameObject.SetActive(true);
                    gameObject.SetActive(false);
                    break;

                // -> 도움말 버튼
                case 3:
                    break;
            }
        }

        public void SetAudioVolume()
        {
            audioSlider.value = audioSlider.value;
        }
    }
}