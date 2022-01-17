using ProjectChan.Define;
using ProjectChan.Resource;
using ProjectChan.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ProjectChan.Define.Audio;

namespace ProjectChan
{
    public class AudioManager : Singleton<AudioManager>
    {
        /// <summary>
        /// => 오디오 소스 컴포넌트를 담아 놓을 프로퍼티
        /// </summary>
        public AudioSource audio { get; private set; }

        protected override void Awake()
        {
            audio = GetComponent<AudioSource>();

            // -> 씬 변경 떄 파괴되지 않도록!
            if (gameObject != null)
            {
                DontDestroyOnLoad(this);
            }
        }

        #region 오디오 관련

        /// <summary>
        /// => 오디오 소스의 볼륨 값을 설정하는 메서드
        /// </summary>
        /// <param name="value"> 설정할 값 </param>
        public void SetVolume(float value)
        {
            audio.volume = value;
        }


        /// <summary>
        /// => 오디오를 정지 하는 메서드
        /// </summary>
        public void AudioStop()
        {
            audio.Stop();
        }

        /// <summary>
        /// => 오디오 소스 클립을 세팅하는 메서드
        /// </summary>
        /// <param name="clipType"> 키고자 하는 클립 타입 </param>
        public void ChangeAudioClip(ClipType clipType)
        {
            switch (clipType)
            {
                case ClipType.StartScene:
                    audio.clip = Resources.Load<AudioClip>(StartSceneClipPath);
                    break;

                case ClipType.NovelGame:
                    audio.clip = Resources.Load<AudioClip>(NovelGameClipPath);
                    break;

                case ClipType.Village:
                    audio.clip = Resources.Load<AudioClip>(VillageClipPath);
                    break;

                case ClipType.DunGeon:
                    audio.clip = Resources.Load<AudioClip>(DunGeonClipPath);
                    break;
            }

            audio.Play();
        }

        #endregion
    }
}
