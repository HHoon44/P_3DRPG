using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.UI
{
    public class UIWindowManager : Singleton<UIWindowManager>
    {
        private List<UIWindow> totalUIWindows = new List<UIWindow>();
        private Dictionary<string, UIWindow> cachedTotalUIWindowDic = new Dictionary<string, UIWindow>();

        private List<UIWindow> totalOpenWindows = new List<UIWindow>();
        private Dictionary<string, object> cachedInstanceDic = new Dictionary<string, object>();

        public void Initialize()
        {
            InitAllWindow();
        }

        private void Update()
        {
            // -> ESC키를 눌럿을때 꺼지는가?
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                var targetWindow = GetTopWindow();

                if (targetWindow != null && targetWindow.canCloseESC)
                { 
                    targetWindow.Close();
                }
            }
        }

        public void AddTotalWindow(UIWindow uiWindow)
        {
            var key = uiWindow.GetType().Name;

            bool hasKey = false;

            if (totalUIWindows.Contains(uiWindow) || cachedTotalUIWindowDic.ContainsKey(key))
            {
                if (cachedTotalUIWindowDic[key] != null)
                {
                    return;
                }
                else
                {
                    hasKey = true;

                    for (int i = 0; i < totalUIWindows.Count; i++)
                    {
                        if (totalUIWindows[i] == null)
                        { 
                            totalUIWindows.RemoveAt(i);
                        }
                    }
                }
            }

            totalUIWindows.Add(uiWindow);

            if (hasKey)
            {
                cachedInstanceDic[key] = uiWindow;
            }
            else
            {
                cachedTotalUIWindowDic.Add(key, uiWindow);
            }
        }

        public void AddOpenWindow(UIWindow uiWindow)
        {
            if (!totalOpenWindows.Contains(uiWindow))
            { 
                totalOpenWindows.Add(uiWindow);
            }
        }

        public void RemoveOpenWindow(UIWindow uiWindow)
        {
            if (totalOpenWindows.Contains(uiWindow))
            { 
                totalOpenWindows.Remove(uiWindow);
            }
        }

        public T GetWindow<T>()
            where T : UIWindow
        {
            string key = typeof(T).Name;

            if (!cachedTotalUIWindowDic.ContainsKey(key))
            { 
                return null;
            }

            if (!cachedInstanceDic.ContainsKey(key))
            {
                cachedInstanceDic.Add(key, (T)Convert.ChangeType(cachedTotalUIWindowDic[key], typeof(T)));
            }
            else if (cachedInstanceDic[key].Equals(null))
            {
                cachedInstanceDic[key] = (T)Convert.ChangeType(cachedTotalUIWindowDic[key], typeof(T));
            }

            return (T)cachedInstanceDic[key];
        }

        public UIWindow GetTopWindow()
        {
            for (int i = 0; i < totalUIWindows.Count - 1; i++)
            {
                if (totalUIWindows[i] != null)
                { 
                    return totalUIWindows[i];
                }
            }

            return null;
        }

        public void CloseAllUI()
        {
            for (int i = 0; i < totalUIWindows.Count; i++)
            {
                if (totalUIWindows[i] != null)
                { 
                    totalUIWindows[i].Close(true);
                }
            }

            //totalUIWindows.Clear();
        }

        public void InitAllWindow()
        {
            for (int i = 0; i < totalUIWindows.Count; i++)
            {
                if (totalUIWindows[i] != null)
                {
                    totalUIWindows[i].InitWindow();
                }
            }
        }
    }
}