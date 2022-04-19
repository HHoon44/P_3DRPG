using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// 인 게임 로직에서 사용할 Character 데이터
    /// </summary>
    [Serializable]
    public class BoCharacter : BoActor
    {
        public SDCharacter sdCharacter;     // 캐릭터 기본 정보 및 레벨에 영향을 받지 않는 스탯
        public SDOriginInfo sdOriginInfo;   // 레벨에 영향을 받는 기본 캐릭터 스탯
        public SDFormInfo sdFormInfo;       // 레벨에 영향을 받는 변신 캐릭터 스탯

        /// <summary>
        /// 서버에서 보내준 통신 데이터( Dto )를 유저 데이터( Bo )로 변환
        /// </summary>
        /// <param name="dtoCharacter"> 서버에서 보낸 통신 데이터 </param>
        public BoCharacter(DtoCharacter dtoCharacter)
        {
            sdCharacter = GameManager.SD.sdCharacters.Where
                (obj => obj.index == dtoCharacter.index).SingleOrDefault();
            sdOriginInfo = GameManager.SD.sdOriginInfos.Where
                (obj => obj.index == dtoCharacter.index).SingleOrDefault();
            sdFormInfo = GameManager.SD.sdFormInfos.Where
                (obj => obj.index == dtoCharacter.index).SingleOrDefault();

            level = dtoCharacter.level;
        }
    }
}
