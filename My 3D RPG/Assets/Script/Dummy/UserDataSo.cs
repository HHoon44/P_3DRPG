using ProjectChan.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.Dummy
{
    /*
     *  ScriptableObject
     *  유니티에서 지원하는 데이터 또는 정적 메서드( 툴 같은 기능 )만을 갖는 클래스
     *  인스턴스가 불가능
     */

    /// <summary>
    /// 데이터 베이스 역할을 하는 클래스( 현재 DB가 없으므로 해당 클래스가 DB 역할 )
    /// </summary>
    [CreateAssetMenu(menuName ="ProjectChan/UserData")]
    public class UserDataSo : ScriptableObject
    {
        public DtoAccount dtoAccount;           // 데이터 베이스의 유저 데이터
        public DtoCharacter dtoCharacter;       // 데이터 베이스의 캐릭터 데이터
        public DtoStage dtoStage;               // 데이터 베이스의 스테이지 데이터
        public DtoItem dtoItem;                 // 데이터 베이스의 아이템 데이터
        public DtoQuest dtoQuest;               // 데이터 베이스의 퀘스트 데이터
    }
}