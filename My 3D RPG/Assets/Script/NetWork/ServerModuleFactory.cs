using ProjectChan.Dummy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.NetWork
{
    /// <summary>
    /// 특정 조건에 따라 서버모듈을 생성할 팩토리 클래스
    /// </summary>
    public class ServerModuleFactory
    {
        /// <summary>
        /// 서버 모듈이 존재한다면 해당 모듈에 구현된 프로토콜을 갖는 인터페이스를 반환
        /// </summary>
        /// <returns></returns>
        public static INetworkClient NewNetworkClientModule()
        {
            // 더미 서버를 사용하고 더미 서버 인스턴스가 존재한다면
            if (GameManager.Instance.useDummyServer && DummyServer.Instance != null)
            {
                // 더미 서버가 지닌 서버 모듈을 반환
                return DummyServer.Instance.dummyModule;
            }

            return null;
        }
    }
}
