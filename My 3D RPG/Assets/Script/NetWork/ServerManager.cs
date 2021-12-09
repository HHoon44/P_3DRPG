using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.NetWork
{
    public class ServerManager : Singleton<ServerManager>
    {
        private INetworkClient netClient;
        public static INetworkClient Server => Instance.netClient;

        protected override void Awake()
        {
            base.Awake();

            if (gameObject != null)
                DontDestroyOnLoad(this);
        }

        public void Initialize()
        {
            netClient = ServerModuleFactory.NewNetworkClientModule();
        }
    }
}