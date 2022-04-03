using ProjectChan.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.NetWork
{
    /// <summary>
    /// 로그인 시 필요한 데이터들을 서버에 요청하는 기능을 가진 클래스
    /// </summary>
    public class LoginHandler
    {
        // 리스폰스 핸들러를 이용해서 데이터를 받아 처리
        public ResponsHandler<DtoAccount> accountHandler;
        public ResponsHandler<DtoCharacter> characterHandler;
        public ResponsHandler<DtoStage> stageHandler;
        public ResponsHandler<DtoItem> itemHandler;
        public ResponsHandler<DtoQuest> questHandler;

        public LoginHandler()
        {
            // 성공 / 실패 시 실행할 메서드를 전달
            accountHandler = new ResponsHandler<DtoAccount>(GetAccountSuccess, OnFailed);
            characterHandler = new ResponsHandler<DtoCharacter>(GetCharacterSuccess, OnFailed);
            stageHandler = new ResponsHandler<DtoStage>(GetStageSuccess, OnFailed);
            itemHandler = new ResponsHandler<DtoItem>(GetItemSuccess, OnFailed);
            questHandler = new ResponsHandler<DtoQuest>(GetQuestSuccess, OnFailed);
        }

        /// <summary>
        /// 서버와 연결 시작을 시작하는 메서드
        /// </summary>
        public void Connect()
        {
            // ServerModuleDummy에 데이터 요청 메서드 실행

            // 계정 정보 요청
            ServerManager.Server.GetAccount(0, accountHandler);
        }

        /// <summary>
        /// 계정 정보 요청 성공 시 실행할 메서드
        /// </summary>
        /// <param name="dtoAccount"> 서버에서 보내준 계정 데이터 </param>
        public void GetAccountSuccess(DtoAccount dtoAccount)
        {
            // 서버에서 받은 Dto 데이터를 Bo 데이터로 변환 후
            // 게임 매니저가 모든 Bo 데이터 관리 객체( User )를 들고 있도록 함
            GameManager.User.boAccount = new BoAccount(dtoAccount);

            // 다음으로 스테이지 정보를 요청
            ServerManager.Server.GetStage(0, stageHandler);
        }

        /// <summary>
        /// 스테이지 정보 요청 성공 시 실행할 메서드
        /// </summary>
        /// <param name="dtoStage"> 서버에서 보내준 스테이지 데이터 </param>
        public void GetStageSuccess(DtoStage dtoStage)
        {
            GameManager.User.boStage = new BoStage(dtoStage);

            // 다음으로 아이템 정보를 요청
            ServerManager.Server.GetItem(0, itemHandler);
        }

        /// <summary>
        /// 아이템 정보 요청 성공 시 실행할 메서드
        /// </summary>
        /// <param name="dtoItem"> 서버에서 받은 아이템 데이터 </param>
        public void GetItemSuccess(DtoItem dtoItem)
        {
            GameManager.User.boItems = new List<BoItem>();

            var boItems = GameManager.User.boItems;

            for (int i = 0; i < dtoItem.dtoItems.Count; i++)
            {
                // 소지한 아이템을 담을 공간
                var dtoItemElement = dtoItem.dtoItems[i];

                // Dto 데이터를 전환해서 담아둘 공간
                BoItem boItem = null;

                // 소지한 아이템의 인덱스와 같은 인덱스의 기획 데이터를 가져옴
                var sdItem = GameManager.SD.sdItems.Where(obj => obj.index == dtoItemElement.index)?.SingleOrDefault();

                // 장비 아이템이라면
                if (sdItem.itemType == Define.ItemType.Equipment)
                {
                    boItem = new BoEquipment(sdItem);

                    var boEquipment = boItem as BoEquipment;
                    boEquipment.reinforceValue = dtoItemElement.reinforceValue;
                    boEquipment.isEquip = dtoItemElement.isEquip;
                }
                else
                {
                    boItem = new BoItem(sdItem);
                }

                SetBoItem(boItem, dtoItemElement);

                // BoItem 목록에 저장
                boItems.Add(boItem);
            }

            void SetBoItem(BoItem boItem, DtoItemElement dtoItemEquipment)
            {
                boItem.slotIndex = dtoItemEquipment.slotIndex;
                boItem.amount = dtoItemEquipment.amount;
            }

            // 다음으로 캐릭터 정보 요청
            ServerManager.Server.GetCharacter(0, characterHandler);
        }

        /// <summary>
        /// 캐릭터 정보 요청 성공 시 실행할 메서드
        /// </summary>
        /// <param name="dtoCharacter"> 서버에서 받은 캐릭터 데이터 </param>
        public void GetCharacterSuccess(DtoCharacter dtoCharacter)
        {
            GameManager.User.boCharacter = new BoCharacter(dtoCharacter);

            // 다음으로 퀘스트 정보 요청
            ServerManager.Server.GetQuest(0, questHandler);
        }

        /// <summary>
        /// => 퀘스트 정보 요청 성공시 실행할 메서드
        /// </summary>
        /// <param name="dtoQuest"> 서버에서 보내준 퀘스트 정보 </param>
        public void GetQuestSuccess(DtoQuest dtoQuest)
        {
            GameManager.User.boQuest = new BoQuest(dtoQuest);

            OnLoginFinished();
        }

        /// <summary>
        /// 모든 로그인 절차가 끝나고 실행할 메서드
        /// </summary>
        private void OnLoginFinished()
        {
            /*
             * 모노를 갖지 않는 클래스에서 FindOfType 같은 메서드를 사용 하려면
             * 모노를 갖는 객체로 접근하여 해당 메서드를 사용해야 합니다
             */

            var startController = GameManager.FindObjectOfType<StartController>();

            if (startController == null)
            {
                return;
            }

            startController.LoadComplete = true;
        }

        /// <summary>
        /// => 서버에서 특정 요청 실패 시 실행될 메서드
        /// </summary>
        /// <param name="dtoBase"> 에러에 대한 코드 및 메세지 </param>
        public void OnFailed(DtoBase dtoBase)
        {

        }
    }
}