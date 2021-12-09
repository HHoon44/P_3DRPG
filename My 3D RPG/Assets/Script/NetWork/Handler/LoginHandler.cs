using ProjectChan.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.NetWork
{
    public class LoginHandler
    {
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
        /// </summary>
        public void Connect()
        {
            // -> 계정 정보 요청 작업
            ServerManager.Server.GetAccount(0, accountHandler);
        }

        /// <summary>
        /// => 계정 정보 받아오기를 성공 했을 때 실행할 메서드
        /// </summary>
        /// <param name="dtoAccount"> 서버에서 받은 계정 데이터 </param>
        public void GetAccountSuccess(DtoAccount dtoAccount)
        {
            GameManager.User.boAccount = new BoAccount(dtoAccount);

            // -> 스테이지 정보 요청 작업
            ServerManager.Server.GetStage(0, stageHandler);
        }

        /// <summary>
        /// => 스테이지 정보 받아오기를 성공 했을 때 실행할 메서드
        /// </summary>
        /// <param name="dtoStage"> 서버에서 받은 계정 데이터 </param>
        public void GetStageSuccess(DtoStage dtoStage)
        {
            GameManager.User.boStage = new BoStage(dtoStage);

            // -> 아이템 정보 요청 작업
            ServerManager.Server.GetItem(0, itemHandler);
        }

        /// <summary>
        /// => 아이템 정보 받아오기를 성공 했을 때 실행할 메서드
        /// </summary>
        /// <param name="dtoItem"> 서버에서 받은 아이템 데이터 </param>
        public void GetItemSuccess(DtoItem dtoItem)
        {
            // -> DtoItem의 정보를 가진 BoItem들을 저장할 새로운 공간
            GameManager.User.boItems = new List<BoItem>();
            var boItems = GameManager.User.boItems;

            // -> DtoItem에 존재하는 dtoItems의 개수만큼 반복
            for (int i = 0; i < dtoItem.dtoItems.Count; i++)
            {
                var dtoItemElement = dtoItem.dtoItems[i];

                // -> DtoItem의 정보를 지닐 새로운 BoItem 공간
                BoItem boItem = null;

                // -> i번째 아이템 데이터의 인덱스와 일치한 SDItem 데이터를 가져온다
                var sdItem = GameManager.SD.sdItems.Where(obj => obj.index == dtoItemElement.index)?.SingleOrDefault();

                // -> 아이템 타입이 장비 아이템 이라면
                if (sdItem.itemType == Define.ItemType.Equipment)
                {
                    // -> 장비 아이템 클래스 할당
                    boItem = new BoEquipment(sdItem);

                    // -> 부모 클래스를 자식 클래스로 형변환
                    var boEquipment = boItem as BoEquipment;
                    boEquipment.reinforceValue = dtoItemElement.reinforceValue;
                    boEquipment.isEquip = dtoItemElement.isEquip;
                }
                // -> 일반 아이템 이라면
                else
                {
                    // -> 일반 아이템 클래스 할당
                    boItem = new BoItem(sdItem);
                }

                SetBoItem(boItem, dtoItemElement);

                // -> DtoItem의 정보를 지닌 BoItem을 추가해서 관리
                boItems.Add(boItem);
            }

            void SetBoItem(BoItem boItem, DtoItemElement dtoItemEquipment)
            {
                boItem.slotIndex = dtoItemEquipment.slotIndex;
                boItem.amount = dtoItemEquipment.amount;
            }

            // -> 캐릭터 정보 요청 작업
            ServerManager.Server.GetCharacter(0, characterHandler);
        }

        /// <summary>
        /// => 캐릭터 정보 받아오기를 성공 했을 때 실행할 메서드
        /// </summary>
        /// <param name="dtoCharacter"> 서버에서 받은 캐릭터 데이터 </param>
        public void GetCharacterSuccess(DtoCharacter dtoCharacter)
        {
            GameManager.User.boCharacter = new BoCharacter(dtoCharacter);

            ServerManager.Server.GetQuest(0, questHandler);
        }

        public void GetQuestSuccess(DtoQuest dtoQuest)
        {
            GameManager.User.boQuest = new BoQuest(dtoQuest);

            OnLoginFinished();
        }

        /// <summary>
        /// => 모든 로그인 절차가 끝나고 실행할 메서드
        /// </summary>
        private void OnLoginFinished()
        {
            var startController = GameManager.FindObjectOfType<StartController>();
            if (startController == null)
            {
                return;
            }

            startController.LoadComplete = true;
        }

        public void OnFailed(DtoBase dtoBase)
        {

        }
    }
}