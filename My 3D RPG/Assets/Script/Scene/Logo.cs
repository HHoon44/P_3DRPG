using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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