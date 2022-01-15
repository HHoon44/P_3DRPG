using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.UI
{
    /// <summary>
    /// => 조그마한 팝업, UI내의 UIElement를 제외한 모든 UI의 베이스 클래스
    /// => 모든 UI들은 이 클래스를 갖는다
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UIWindow : MonoBehaviour
    {
        public bool isOpen;     // -> 해당 UI의 활성화 상태

        private CanvasGroup cachedCanvasGroup;

        /// <summary>
        /// => 캔버스 그룹을 통해 UI를 활성 or 비활성화 하는 효과를 준다(알파값을 설정)
        /// => 비활성화 시 실제로 객체가 비활성화되는 것이 아니므로 UI입력을 차단 한다(블록 레이캐스트)
        /// </summary>
        public CanvasGroup CachedCanvasGroup
        {
            get
            {
                if (cachedCanvasGroup == null)
                    cachedCanvasGroup = GetComponent<CanvasGroup>();

                return cachedCanvasGroup;
            }
        }

        public virtual void Start()
        {
            InitWindow();
        }

        /// <summary>
        /// => UI 초기화 기능 메서드
        /// </summary>
        public virtual void InitWindow()
        {
            UIWindowManager.Instance.AddTotalWindow(this);

            if (isOpen)
            {
                Open(true);
            }
            else
            {
                Close(true);
            }
        }

        /// <summary>
        /// => UI를 활성화 하는 기능 메서드
        /// </summary>
        /// <param name="force"> 활성화 여부 </param>
        public virtual void Open(bool force = false)
        {
            if (!isOpen || force)
            {
                isOpen = true;
                UIWindowManager.Instance.AddOpenWindow(this);
                SetCanvasGroup(true);
            }
        }

        /// <summary>
        /// => UI를 비활성화 하는 기능 메서드
        /// </summary>
        /// <param name="force"> 비활성화 여부 </param>
        public virtual void Close(bool force = false)
        {
            if (isOpen || force)
            {
                isOpen = false;
                UIWindowManager.Instance.RemoveOpenWindow(this);
                SetCanvasGroup(false);
            }
        }

        /// <summary>
        /// => 활성화 상태에 따라 캔버스 그룹 내의 필드를 설정
        /// </summary>
        /// <param name="isActive"></param>
        private void SetCanvasGroup(bool isActive)
        {
            CachedCanvasGroup.alpha = Convert.ToInt32(isActive);
            cachedCanvasGroup.interactable = isActive;
            cachedCanvasGroup.blocksRaycasts = isActive;
        }
    }
}
