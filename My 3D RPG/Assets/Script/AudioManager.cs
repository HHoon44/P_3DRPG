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
        /// => ����� �ҽ� ������Ʈ�� ��� ���� ������Ƽ
        /// </summary>
        public AudioSource audio { get; private set; }

        protected override void Awake()
        {
            audio = GetComponent<AudioSource>();

            // -> �� ���� �� �ı����� �ʵ���!
            if (gameObject != null)
            {
                DontDestroyOnLoad(this);
            }
        }

        #region ����� ����

        /// <summary>
        /// => ����� �ҽ��� ���� ���� �����ϴ� �޼���
        /// </summary>
        /// <param name="value"> ������ �� </param>
        public void SetVolume(float value)
        {
            audio.volume = value;
        }

        /// <summary>
        /// => ������� ���� �ϴ� �޼���
        /// </summary>
        public void AudioStop()
        {
            audio.Stop();
        }

        public void ChangeAudioClip(string clipName)
        {
            switch (clipName)
            {
                case "StartVillage":
                    ChangeAudioClip(Audio.ClipType.Village);
                    break;

                case "DunGeon":
                    ChangeAudioClip(Audio.ClipType.DunGeon);
                    break;
                case "StartScene":
                    ChangeAudioClip(Audio.ClipType.StartScene);
                    break;
                case "NovelGame":
                    ChangeAudioClip(Audio.ClipType.NovelGame);
                    break;
            }
        }

        /// <summary>
        /// => ����� �ҽ� Ŭ���� �����ϴ� �޼���
        /// </summary>
        /// <param name="clipType"> Ű���� �ϴ� Ŭ�� Ÿ�� </param>
        private void ChangeAudioClip(ClipType clipType)
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
