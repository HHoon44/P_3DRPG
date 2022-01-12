using ProjectChan.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.NetWork
{
    /// <summary>
    /// => 로그인 시 필요한 데이터를 서버에 요청기능을 하는 클래스
    /// </summary>
    public class LoginHandler
    {
        // -> 리스폰스 핸들러를 이용하여 데이터를 받아 처리합니다!
        public ResponsHandler<DtoAccount> accountHandler;
        public ResponsHandler<DtoCharacter> characterHandler;
        public ResponsHandler<DtoStage> stageHandler;
        public ResponsHandler<DtoItem> itemHandler;
        public ResponsHandler<DtoQuest> questHandler;

        public LoginHandler()
        {
            accountHandler = new ResponsHandler<DtoAccount>(GetAccountSuccess, OnFailed);
            characterHandler = new ResponsHandler<DtoCharacter>(GetCharacterSuccess, OnFailed);
            stageHandler = new ResponsHandler<DtoStage>(GetStageSuccess, OnFailed);
            itemHandler = new ResponsHandler<DtoItem>(GetItemSuccess, OnFailed);
            questHandler = new ResponsHandler<DtoQuest>(GetQuestSuccess, OnFailed);
        }

        /// <summary>
        /// => 서버와 연결 시작
        /// => INetworkClient를 지닌 Module에 접근하여 로그인 작업 메서드
        /// </summary>
        public void Connect()
        {
            // -> 계정 정보를 요청합니다!
            ServerManager.Server.GetAccount(0, accountHandler);
        }

        /// <summary>
        /// => 계정 정보 받아오기를 성공 했을 때 실행할 메서드
        /// </summary>
        /// <param name="dtoAccount"> 서버에서 받은 계정 데이터 </param>
        public void GetAccountSuccess(DtoAccount dtoAccount)
        {
            // -> 서버에서 받은 DtoAccount 데이터를 Bo 데이터로 변환 후
            //    GM이 BoAccount를 들고 있게 합니다!
            GameManager.User.boAccount = new BoAccount(dtoAccount);

            // -> 다음으로 스테이지 정보를 요청합니다!
            ServerManager.Server.GetStage(0, stageHandler);
        }

        /// <summary>
        /// => 스테이지 정보 받아오기를 성공 했을 때 실행할 메서드
        /// </summary>
        /// <param name="dtoStage"> 서버에서 받은 계정 데이터 </param>
        public void GetStageSuccess(DtoStage dtoStage)
        {
            // -> 서버에서 받은 DtoStage 데이터를 Bo 데이터로 변환 후
            //    GM이 BoStage를 들고 있게 합니다!
            GameManager.User.boStage = new BoStage(dtoStage);

            // -> 다음으로 아이템 정보를 요청합니다!
            ServerManager.Server.GetItem(0, itemHandler);
        }

        /// <summary>
        /// => 아이템 정보 받아오기를 성공 했을 때 실행할 메서드
        /// </summary>
        /// <param name="dtoItem"> 서버에서 받은 아이템 데이터 </param>
        public void GetItemSuccess(DtoItem dtoItem)
        {
            // -> 새로운 BoItem 리스트를 생성해서 GM이 들고 있게 합니다!
            GameManager.User.boItems = new List<BoItem>();

            var boItems = GameManager.User.boItems;

            // -> DB에 저장된 아이템 개수만큼 반복합니다!
            for (int i = 0; i < dtoItem.dtoItems.Count; i++)
            {
                var dtoItemElement = dtoItem.dtoItems[i];

                // -> DB에 저장할 Item 정보를 지닐 새로운 공간 입니다!
                BoItem boItem = null;

                // -> i번째 아이템 데이터의 인덱스와 일치한 SDItem 데이터를 가져 옵니다!
                var sdItem = GameManager.SD.sdItems.Where(obj => obj.index == dtoItemElement.index)?.SingleOrDefault();

                // -> 장비 아이템 이라면!
                if (sdItem.itemType == Define.ItemType.Equipment)
                {
                    boItem = new BoEquipment(sdItem);

                    // -> 부모 클래스를 자식 클래스로 형변환 합니다!
                    var boEquipment = boItem as BoEquipment;
                    boEquipment.reinforceValue = dtoItemElement.reinforceValue;
                    boEquipment.isEquip = dtoItemElement.isEquip;
                }
                else
                {
                    boItem = new BoItem(sdItem);
                }

                SetBoItem(boItem, dtoItemElement);

                // -> Bo 아이템 리스트에 저장합니다!
                boItems.Add(boItem);
            }

            void SetBoItem(BoItem boItem, DtoItemElement dtoItemEquipment)
            {
                boItem.slotIndex = dtoItemEquipment.slotIndex;
                boItem.amount = dtoItemEquipment.amount;
            }

            // -> 다음으로 캐릭터 정보를 요청합니다!
            ServerManager.Server.GetCharacter(0, characterHandler);
        }

        /// <summary>
        /// => 캐릭터 정보 받아오기를 성공 했을 때 실행할 메서드
        /// </summary>
        /// <param name="dtoCharacter"> 서버에서 받은 캐릭터 데이터 </param>
        public void GetCharacterSuccess(DtoCharacter dtoCharacter)
        {
            // -> 서버에서 받은 DtoCharacter 데이터를 Bo 데이터로 변환 후
            //    GM이 BoCharacter를 들고 있게 합니다!
            GameManager.User.boCharacter = new BoCharacter(dtoCharacter);

            // -> 다음으로 퀘스트 정보를 요청합니다!
            ServerManager.Server.GetQuest(0, questHandler);
        }

        /// <summary>
        /// => 퀘스트 정보 요청 성공시 실행할 메서드
        /// </summary>
        /// <param name="dtoQuest"> 서버에서 보내준 퀘스트 정보 </param>
        public void GetQuestSuccess(DtoQuest dtoQuest)
        {
            // -> 서버에서 받은 DtoQuest 데이터를 Bo 데이터로 변환 후
            //    GM이 BoQuest를 들고 있게 합니다!
            GameManager.User.boQuest = new BoQuest(dtoQuest);

            OnLoginFinished();
        }

        /// <summary>
        /// => 모든 로그인 절차가 끝나고 실행할 메서드
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

            // -> 로그인 성공후 다음으로 넘어 갑니다!
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