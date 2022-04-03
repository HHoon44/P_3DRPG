using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 *  상황?
 *  지금처럼 더미서버를 사용할 경우 더미서버 모듈을 생성
 *  만약 라이브서버 사용할 경우 라이브서버 모듈을 생성
 */

namespace ProjectChan.NetWork
{
    /// <summary>
    /// 클라이언트 내 전체적인 서버 통신을 관리하는 매니저 클래스
    /// 상황에 따라 서버 모듈을 생성하여 통신을 처리
    /// </summary>
    public class ServerManager : Singleton<ServerManager>
    {
        /// <summary>
        /// 서버 통신에 필요한 프로토콜 메서드를 갖는 인터페이스
        /// </summary>
        private INetworkClient netClient;

        public static INetworkClient Server => Instance.netClient;

        protected override void Awake()
        {
            base.Awake();

            if (gameObject != null)
            { 
                DontDestroyOnLoad(this);
            }
        }

        /// <summary>
        /// 서버 매니저 초기화 메서드
        /// </summary>
        public void Initialize()
        {
            // 서버 모듈 팩토리를 사용해 서버 모듈을 생성
            netClient = ServerModuleFactory.NewNetworkClientModule();
        }
    }
}