using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectChan
{
    /// <summary>
    /// => 로고씬을 관리하는 클래스
    /// </summary>
    public class Logo : MonoBehaviour
    {
        /// <summary>
        /// => 로고 이벤트 메서드
        /// </summary>
        public void LoadStartScene()
        {
            SceneManager.LoadScene("StartScene");
        }
    }
}