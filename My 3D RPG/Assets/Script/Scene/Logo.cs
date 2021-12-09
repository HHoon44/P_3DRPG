using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviour
{
    public void LoadStartScene()    /// -> 로고 Anim이 끝나고 실행
    {
        SceneManager.LoadScene("StartScene");
    }
}