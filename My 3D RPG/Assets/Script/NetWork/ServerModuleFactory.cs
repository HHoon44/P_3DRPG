using ProjectChan.Dummy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.NetWork
{
    public class ServerModuleFactory
    {
        public static INetworkClient NewNetworkClientModule()
        {
            if (GameManager.Instance.useDummyServer && DummyServer.Instance != null)
            {
                return DummyServer.Instance.dummyModule;
            }

            return null;
        }
    }
}
