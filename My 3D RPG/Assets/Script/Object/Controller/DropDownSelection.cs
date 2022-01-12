using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectChan.Object
{
    /// <summary>
    /// => 퀄리티 드롭박스의 클래스
    /// </summary>
    public class DropDownSelection : MonoBehaviour
    {
        private Dropdown dropDown;      // -> 드롭다운 컴포넌트

        void Start()
        {
            dropDown = GetComponent<Dropdown>();
        }

        /// <summary>
        /// => 누르는 박스에 따라 퀄리티를 설정하는 메서드
        /// </summary>
        public void OnChange()
        {
            QualitySettings.SetQualityLevel(dropDown.value, false);
        }
    }
}
