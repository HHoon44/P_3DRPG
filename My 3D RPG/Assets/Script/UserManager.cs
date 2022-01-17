using ProjectChan.DB;
using ProjectChan.Define;
using ProjectChan.Dummy;
using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProjectChan.Define.NewPlayerData;

namespace ProjectChan
{
    /// <summary>
    /// => 유저의 게임이 새로운 게임 or 이어하는 게임인지 관리하는 클래스
    /// </summary>
    public class UserManager : Singleton<UserManager>
    {
        public GameType currentGameType;        // -> 현재 게임 타입 EX) 새로운 게임 or 이어하기

        protected override void Awake()
        {
            if (gameObject != null)
            {
                DontDestroyOnLoad(this);
            }
        }

        public void Initialize()
        {
            // -> 현재 게임 타입에 따라서 유저 데이터 설정합니다!
            switch (currentGameType)
            {
                case GameType.New:
                    // -> 초기 DtoAccount를 생성합니다!
                    var playerData = new NewPlayerData();
                    var dummyServer = DummyServer.Instance;

                    dummyServer.userData.dtoAccount = playerData.newStartDA;
                    dummyServer.userData.dtoCharacter = playerData.newStartDC;
                    dummyServer.userData.dtoStage = playerData.newStartDS;
                    dummyServer.userData.dtoItem.dtoItems.Clear();
                    Array.Resize(ref dummyServer.userData.dtoQuest.progressQuests, 0);
                    Array.Resize(ref dummyServer.userData.dtoQuest.completeQuests, 0);
                    break;

                case GameType.Continue:
                    break;
            }
        }
    }
}