using ProjectChan.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.NetWork
{
    /// <summary>
    /// => 서버와 통신하는 프로토콜(서버와 클라이언트 사이의 통신에 사용되는 API) 메서드를 갖는 인터페이스
    /// </summary>
    public interface INetworkClient
    {
        /// <summary>
        /// => 서버에 계정 정보를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"> 서버에 계정 정보를 요청하면서 보내는 각 계정마다 부여된 고유 아이디 </param>
        /// <param name="responsHandler"> 서버에 요청한 데이터를 받아 처리하는 핸들러 </param>
        public void GetAccount(int uniqueId, ResponsHandler<DtoAccount> responsHandler);

        /// <summary>
        /// => 서버에 캐릭터 정보를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"> 서버에 계정 정보를 요청하면서 보내는 각 계정마다 부여된 고유 아이디 </param>
        /// <param name="responsHandler"> 서버에 요청한 데이터를 받아 처리하는 핸들러 </param>
        public void GetCharacter(int uniqueId, ResponsHandler<DtoCharacter> responsHandler);

        /// <summary>
        /// => 서버에 스테이지 정보를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"> 서버에 계정 정보를 요청하면서 보내는 각 계정마다 부여된 고유 아이디 </param>
        /// <param name="responsHandler"> 서버에 요청한 데이터를 받아 처리하는 핸들러 </param>
        public void GetStage(int uniqueId, ResponsHandler<DtoStage> responsHandler);

        /// <summary>
        /// => 서버에 아이템 정보를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"> 서버에 계정 정보를 요청하면서 보내는 각 계정마다 부여된 고유 아이디 </param>
        /// <param name="responsHandler"> 서버에 요청한 데이터를 받아 처리하는 핸들러 </param>
        public void GetItem(int uniqueId, ResponsHandler<DtoItem> responsHandler);

        /// <summary>
        /// => 서버에 퀘스트 정보를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"> 서버에 계정 정보를 요청하면서 보내는 각 계정마다 부여된 고유 아이디 </param>
        /// <param name="responsHandler"> 서버에 요청한 데이터를 받아 처리하는 핸들러 </param>
        public void GetQuest(int uniqueId, ResponsHandler<DtoQuest> responsHandler);

        /// <summary>
        /// => 서버에 유저에 대한 퀘스트 DB에 새로운 퀘스트 정보 추가를 요청하는 메서드
        /// </summary>
        /// <param name="uniqueId"> 서버에 계정 정보를 요청하면서 보내는 각 계정마다 부여된 고유 아이디 </param>
        /// <param name="questIndex"> DB에 추가할 새로운 퀘스트 인덱스 </param>
        /// <param name="responsHandler"> 서버에 요청한 데이터를 받아 처리하는 핸들러 </param>
        public void AddQuest(int uniqueId, int questIndex, ResponsHandler<DtoQuestProgress> responsHandler);
    }
}