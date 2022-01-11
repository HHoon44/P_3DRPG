using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// => 클라이언트 내에사 사용할 Character 데이터
    /// => 작업과정에서 데이터를 확인하기 위해서 Serializable
    /// </summary>
    [Serializable]
    public class BoCharacter : BoActor
    {
        public SDCharacter sdCharacter;     // -> 캐릭터 기본 정보 및 레벨에 영향을 받지 않는 스텟
        public SDOriginInfo sdOriginInfo;   // -> 레벨에 영향을 받는 기본 캐릭터 스텟
        public SDFormInfo sdFormInfo;       // -> 레벨에 영향을 받는 변신 캐릭터 스텟

        /// <summary>
        /// => 서버를 통해 받은 DtoCharacter 데이터를 BoCharacter 데이터로 변환하는 메서드
        /// </summary>
        /// <param name="dtoCharacter"></param>
        public BoCharacter(DtoCharacter dtoCharacter)
        {
            sdCharacter = GameManager.SD.sdCharacters.Where(obj => obj.index == dtoCharacter.index).SingleOrDefault();
            sdOriginInfo = GameManager.SD.sdOriginInfos.Where(obj => obj.index == dtoCharacter.index).SingleOrDefault();
            sdFormInfo = GameManager.SD.sdFormInfos.Where(obj => obj.index == dtoCharacter.index).SingleOrDefault();

            level = dtoCharacter.level;
        }
    }
}
