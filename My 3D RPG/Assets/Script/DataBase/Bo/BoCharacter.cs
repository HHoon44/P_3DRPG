using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    [Serializable]
    public class BoCharacter : BoActor
    {
        public SDCharacter sdCharacter;
        public SDOriginInfo sdOriginInfo;
        public SDFormInfo sdFormInfo;

        public BoCharacter(DtoCharacter dtoCharacter)
        {
            sdCharacter = GameManager.SD.sdCharacters.Where(obj => obj.index == dtoCharacter.index).SingleOrDefault();
            sdOriginInfo = GameManager.SD.sdOriginInfos.Where(obj => obj.index == dtoCharacter.index).SingleOrDefault();
            sdFormInfo = GameManager.SD.sdFormInfos.Where(obj => obj.index == dtoCharacter.index).SingleOrDefault();

            level = dtoCharacter.level;
        }
    }
}
