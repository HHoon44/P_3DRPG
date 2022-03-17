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
    /// 모든 UIWindow클래스를 관리 하는 매니저 클래스
    /// </summary>
    public class UIWindowManager : Singleton<UIWindowManager>
    {
        /// <summary>
        /// 활성화 상태의 UIWindow를 갖는 객체 리스트
        /// </summary>
        private List<UIWindow> totalOpenWindows = new List<UIWindow>();

        /// <summary>
        /// UM에 등록된 UIWindow를 담아놓을 객체 리스트
        /// </summary>
        private List<UIWindow> totalUIWindows = new List<UIWindow>();

        /// <summary>
        /// UM에 UIWindow 등록 시, 캐싱하여 담아둘 딕셔너리
        /// </summary>
        private Dictionary<string, UIWindow> cachedTotalUIWindowDic = new Dictionary<string, UIWindow>();

        /// <summary>
        /// UM에 등록된 UI에 인스턴스 접근 시, 해당 인스턴스를 캐싱하여 담아둘 딕셔너리
        /// ( UM를 이용해서 특정 인스턴스 접근 메서드를 사용할 시에, 내가 코드로 직접 접근한 UIWindow들만 담김 )
        /// </summary>
        private Dictionary<string, object> cachedInstanceDic = new Dictionary<string, object>();

        /// <summary>
        /// UM 초기화 메서드
        /// </summary>
        public void Initialize()
        {
            InitAllWindow();
        }

        /// <summary>
        /// UM에 등록된 모든 UIWindow를 초기화 하는 메서드
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

        /// <summary>
        /// UIWindow를 담아놓는 메서드
        /// </summary>
        /// <param name="uiWindow"> 담아둘 UIWindow </param>
        public void AddTotalWindow(UIWindow uiWindow)
        {
            // Key값은 UI의 이름
            var key = uiWindow.GetType().Name;

            bool hasKey = false;

            // 리스트와 딕셔너리에 등록하려는 UI가 존재한다면
            if (totalUIWindows.Contains(uiWindow) || cachedTotalUIWindowDic.ContainsKey(key))
            {
                // 저장하려는 UI가 이미 캐싱이 되어있다면
                if (cachedTotalUIWindowDic[key] != null)
                {
                    return;
                }
                else
                {
                    // Key가 존재
                    hasKey = true;

                    // 키 값은 있으나, 참조하고 있는 인스턴스가 없으므로 리스트에서 제거
                    for (int i = 0; i < totalUIWindows.Count; i++)
                    {
                        if (totalUIWindows[i] == null)
                        {
                            totalUIWindows.RemoveAt(i);
                        }
                    }
                }
            }

            // 인스턴스할 UI를 저장한다
            totalUIWindows.Add(uiWindow);

            // 키가 있다면
            if (hasKey)
            {
                // 인스턴스 값을 인스턴스 딕셔너리에 저장
                cachedInstanceDic[key] = uiWindow;
            }
            else
            {
                cachedTotalUIWindowDic.Add(key, uiWindow);
            }
        }

        /// <summary>
        /// UM에 등록된 UIWindow 인스턴스를 반환하는 메서드
        /// </summary>
        /// <typeparam name="T"> 반환 받을 UIWindow의 타입 </typeparam>
        /// <returns></returns>
        public T GetWindow<T>() where T : UIWindow
        {
            // T 타입으로 Key 값을 설정
            string key = typeof(T).Name;

            // Key 값이 캐싱 Dic에 없다면
            if (!cachedTotalUIWindowDic.ContainsKey(key))
            {
                return null;
            }

            // 인스턴스 시 담아둘 딕셔너리에 키 값이 없다면
            if (!cachedInstanceDic.ContainsKey(key))
            {
                // 값이 지정된 개체와 동일한 지정된 형식의 개체를 Key값과 Dic에 저장
                cachedInstanceDic.Add(key, (T)Convert.ChangeType(cachedTotalUIWindowDic[key], typeof(T)));
            }
            else if (cachedInstanceDic[key].Equals(null))
            {
                // 값이 지정된 개체와 동일한 지정된 형식의 개체를 Dic에 저장
                cachedInstanceDic[key] = (T)Convert.ChangeType(cachedTotalUIWindowDic[key], typeof(T));
            }

            return (T)cachedInstanceDic[key];
        }

        /// <summary>
        /// 활성화 된 UIWindow를 리스트에 추가하는 메서드
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
        /// 비활성화 된 UIWIndow를 리스트에서 제거하는 메서드
        /// </summary>
        /// <param name="uiWindow"> 비활성화 할 UIWindow </param>
        public void RemoveOpenWindow(UIWindow uiWindow)
        {
            if (totalOpenWindows.Contains(uiWindow))
            {
                totalOpenWindows.Remove(uiWindow);
            }
        }
    }
}