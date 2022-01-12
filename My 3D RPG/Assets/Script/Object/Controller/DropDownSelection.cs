using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectChan.Object
{
    /// <summary>
    /// => ����Ƽ ��ӹڽ��� Ŭ����
    /// </summary>
    public class DropDownSelection : MonoBehaviour
    {
        private Dropdown dropDown;      // -> ��Ӵٿ� ������Ʈ

        void Start()
        {
            dropDown = GetComponent<Dropdown>();
        }

        /// <summary>
        /// => ������ �ڽ��� ���� ����Ƽ�� �����ϴ� �޼���
        /// </summary>
        public void OnChange()
        {
            QualitySettings.SetQualityLevel(dropDown.value, false);
        }
    }
}
