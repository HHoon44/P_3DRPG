using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectChan
{
    /// <summary>
    /// => �ΰ���� �����ϴ� Ŭ����
    /// </summary>
    public class Logo : MonoBehaviour
    {
        /// <summary>
        /// => �ΰ� �̺�Ʈ �޼���
        /// </summary>
        public void LoadStartScene()
        {
            SceneManager.LoadScene("StartScene");
        }
    }
}