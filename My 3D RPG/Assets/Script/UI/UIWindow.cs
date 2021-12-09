using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIWindow : MonoBehaviour
    {
        public bool canCloseESC;
        public bool isOpen;

        private CanvasGroup cachedCanvasGroup;
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

        public virtual void Open(bool force = false)
        {
            if (!isOpen || force)
            {
                isOpen = true;
                UIWindowManager.Instance.AddOpenWindow(this);
                SetCanvasGroup(true);
            }
        }

        public virtual void Close(bool force = false)
        {
            if (isOpen || force)
            {
                isOpen = false;
                UIWindowManager.Instance.RemoveOpenWindow(this);
                SetCanvasGroup(false);
            }
        }

        private void SetCanvasGroup(bool isActive)
        {
            CachedCanvasGroup.alpha = Convert.ToInt32(isActive);
            cachedCanvasGroup.interactable = isActive;
            cachedCanvasGroup.blocksRaycasts = isActive;
        }
    }
}
