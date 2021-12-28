using ProjectChan.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectChan
{
    public class AudioManager : Singleton<AudioManager>
    {
        public AudioSource audio { get; private set; }


        void Start()
        {
            audio = GetComponent<AudioSource>();
            AudioPlay();
        }

        public void AudioPlay()
        {
            audio.Play();
        }

        public void AudioStop()
        {
            audio.Stop();
        }
    }
}
