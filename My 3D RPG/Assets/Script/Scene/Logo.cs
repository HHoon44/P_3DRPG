using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviour
{
    public void LoadStartScene()    /// -> �ΰ� Anim�� ������ ����
    {
        SceneManager.LoadScene("StartScene");
    }
}