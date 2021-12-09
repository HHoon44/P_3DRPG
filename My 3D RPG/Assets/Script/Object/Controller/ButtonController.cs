using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace ProjectChan
{
    public class ButtonController : MonoBehaviour
    {
        public void ButtonOption()  /// -> StartScene에 있는 버튼 UI를 관리
        {
            switch (EventSystem.current.currentSelectedGameObject.transform.GetSiblingIndex())
            {
                case 0:
                    SceneManager.LoadScene("StartLoading");
                    break;

                case 1:
                    break;

                case 2:
                    break;

                case 3:
                    break;
            }
        }
    }
}