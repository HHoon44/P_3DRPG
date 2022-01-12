using ProjectChan.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.Dummy
{
    /// <summary>
    /// => 데이터 베이스가 없으므로 해당 클래스가 데이터 베이스
    /// </summary>
    [CreateAssetMenu(menuName ="ProjectChan/UserData")]
    public class UserDataSo : ScriptableObject
    {
        public DtoAccount dtoAccount;           // -> 데이터 베이스에 저장되어있는 유저 데이터
        public DtoCharacter dtoCharacter;       // -> 데이터 베이스에 저장되어있는 캐릭터 데이터
        public DtoStage dtoStage;               // -> 데이터 베이스에 저장되어있는 스테이지 데이터
        public DtoItem dtoItem;                 // -> 데이터 베이스에 저장되어있는 아이템 데이터
        public DtoQuest dtoQuest;               // -> 데이터 베이스에 저장되어있는 퀘스트 데이터
    }
}