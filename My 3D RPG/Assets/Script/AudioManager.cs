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
        public AudioSource audio { get; private set; }

        void Awake()
        {
            audio = GetComponent<AudioSource>();

            if (gameObject != null)
            { 
                DontDestroyOnLoad(this);
            }
        }

        public void SetVolume(float value)
        {
            audio.volume = value;
        }

        #region 오디오 관련

        public void AudioStop()
        {
            audio.Stop();
        }

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
