using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.UI
{
    /// <summary>
    /// => 모든 UIWindow클래스를 관리 하는 매니저 클래스
    /// </summary>
    public class UIWindowManager : Singleton<UIWindowManager>
    {
        /// <summary>
        /// => UM에 등록된 UIWindow를 갖는 객체 리스트
        /// </summary>
        private List<UIWindow> totalUIWindows = new List<UIWindow>();

        /// <summary>
        /// => 활성화 되어있는 UIWindow를 갖는 객체 리스트
        /// </summary>
        private List<UIWindow> totalOpenWindows = new List<UIWindow>();

        /// <summary>
        /// => UM에 UIWindow 등록 시 캐싱하여 담아둘 딕셔너리
        /// </summary>
        private Dictionary<string, UIWindow> cachedTotalUIWindowDic = new Dictionary<string, UIWindow>();

        /// <summary>
        /// => UM에 등록된 UI에 인스턴스 접근 시 해당 인스턴스 들을 캐싱하여 담아둘 딕셔너리
        /// </summary>
        private Dictionary<string, object> cachedInstanceDic = new Dictionary<string, object>();

        public void Initialize()
        {
            InitAllWindow();
        }

        /// <summary>
        /// => UIWindow를 담아놓는 메서드
        /// </summary>
        /// <param name="uiWindow"> 담아둘 UIWindow </param>
        public void AddTotalWindow(UIWindow uiWindow)
        {
            // -> 키 이름은 UI의 이름으로 합니다!
            var key = uiWindow.GetType().Name;

            bool hasKey = false;

            // -> 리스트와 딕셔너리에 등록하고자 하는 인스턴스가 있다면!
            if (totalUIWindows.Contains(uiWindow) || cachedTotalUIWindowDic.ContainsKey(key))
            {
                // -> 이미 캐싱 해놓은 인스턴스가 있다면!
                if (cachedTotalUIWindowDic[key] != null)
                {
                    return;
                }
                else
                {
                    // -> 키 있습니다!
                    hasKey = true;

                    // -> 키 값은 있으나 비어있으므로 참조하고 있는 인스턴스가 없어
                    //    리스트에서 제거 합니다!
                    for (int i = 0; i < totalUIWindows.Count; i++)
                    {
                        if (totalUIWindows[i] == null)
                        {
                            totalUIWindows.RemoveAt(i);
                        }
                    }
                }
            }

            // -> 리스트에 현재 인스턴스할 UIWindow를 저장합니다!
            totalUIWindows.Add(uiWindow);

            // -> 키가 있다면!
            if (hasKey)
            {
                // -> 캐싱 딕셔너리에 저장합니다!
                cachedInstanceDic[key] = uiWindow;
            }
            else
            {
                // -> 키 값과 같이 저장합니다!
                cachedTotalUIWindowDic.Add(key, uiWindow);
            }
        }

        /// <summary>
        /// => 활성화 할 UIWindow를 리스트에 추가하는 메서드
        /// </summary>
        /// <param name="uiWindow"> 활성화 할 UIWindow </param>
        public void AddOpenWindow(UIWindow uiWindow)
        {
            if (!totalOpenWindows.Contains(uiWindow))
            {
                totalOpenWindows.Add(uiWindow);
            }
        }

        /// <summary>
        /// => 비활성화 할 UIWIndow를 리스트에서 제거하는 메서드
        /// </summary>
        /// <param name="uiWindow"> 비활성화 할 UIWindow </param>
        public void RemoveOpenWindow(UIWindow uiWindow)
        {
            if (totalOpenWindows.Contains(uiWindow))
            {
                totalOpenWindows.Remove(uiWindow);
            }
        }

        /// <summary>
        /// => UM에 등록된 UIWindow 인스턴스를 반환하는 메서드
        /// </summary>
        /// <typeparam name="T"> 반환 받을 UIWindow의 타입 </typeparam>
        /// <returns></returns>
        public T GetWindow<T>()
            where T : UIWindow
        {
            string key = typeof(T).Name;

            if (!cachedTotalUIWindowDic.ContainsKey(key))
            {
                return null;
            }

            // -> 인스턴스 시 담아둘 딕셔너리에 키 값이 없다면!
            if (!cachedInstanceDic.ContainsKey(key))
            {
                // -> 딕셔너리에 추가 해줍니다!
                cachedInstanceDic.Add(key, (T)Convert.ChangeType(cachedTotalUIWindowDic[key], typeof(T)));
            }
            else if (cachedInstanceDic[key].Equals(null))
            {
                // -> 키 값은 존재하나 Null 이라면 UIWindow 인스턴스를 T타입으로 캐스팅 후 등록합니다!
                cachedInstanceDic[key] = (T)Convert.ChangeType(cachedTotalUIWindowDic[key], typeof(T));
            }

            return (T)cachedInstanceDic[key];
        }

        /// <summary>
        /// => UM에 등록된 모든 UIWindow를 초기화 하는 기능
        /// </summary>
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

        #region 안씀

        /// <summary>
        /// => 현재 활성화 된 UIWindow중 가장 위의 UIWindow를 반환 하는 메서드
        /// </summary>
        /// <returns></returns>
        public UIWindow GetTopWindow()
        {
            // -> 최상위 UIWindow를 찾는 작업입니다!
            for (int i = 0; i < totalUIWindows.Count - 1; i++)
            {
                if (totalUIWindows[i] != null)
                {
                    return totalUIWindows[i];
                }
            }

            return null;
        }

        /// <summary>
        /// => UM에 등록된 모든 UIWindow를 닫는 메서드
        /// </summary>
        public void CloseAllUI()
        {
            for (int i = 0; i < totalUIWindows.Count; i++)
            {
                if (totalUIWindows[i] != null)
                {
                    totalUIWindows[i].Close(true);
                }
            }
        }

        #endregion
    }
}