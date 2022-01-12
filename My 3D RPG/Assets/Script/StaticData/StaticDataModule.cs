using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectChan.Util;
using UnityEngine;

namespace ProjectChan.SD
{
    /// <summary>
    /// => 모든 기획데이터를 들고 있는 클래스
    /// => 데이터를 로드하고 들고 있기만 하는 것이므로 모노를 상속받을 필요가 없음
    /// </summary>
    [Serializable]
    public class StaticDataModule
    {
        public List<SDCharacter> sdCharacters;
        public List<SDStage> sdStages;
        public List<SDNovel> sdNovels;
        public List<SDOriginInfo> sdOriginInfos;
        public List<SDFormInfo> sdFormInfos;
        public List<SDMonster> sdMonsters;
        public List<SDItem> sdItems;
        public List<SDNPC> sdNPCs;
        public List<SDQuest> sdQuests;
        public List<SDQuestSpeech> sdQuestSpeechs;

        public void Initialize()
        {
            var loader = new StaticDataLoader();

            loader.Load(out sdCharacters);
            loader.Load(out sdStages);
            loader.Load(out sdNovels);
            loader.Load(out sdOriginInfos);
            loader.Load(out sdFormInfos);
            loader.Load(out sdMonsters);
            loader.Load(out sdItems);
            loader.Load(out sdNPCs);
            loader.Load(out sdQuests);
            loader.Load(out sdQuestSpeechs);
        }

        /// <summary>
        /// => 기획 데이터를 불러올 로더 클래스
        /// </summary>
        private class StaticDataLoader
        {
            private string path;

            public StaticDataLoader()
            {
                path = $"{Application.dataPath}/StaticData/Json";
            }

            /// <summary>
            /// => 지정된 경로에 파일을 작성하는 메서드
            /// </summary>
            /// <typeparam name="T"> 기획 데이터 타입 </typeparam>
            /// <param name="data"> 기획 데이터 </param>
            public void Load<T>(out List<T> data)
                where T : StaticData
            {
                // -> 파일이름이 타입이름에서 SD만 제거하면 동일하다는 규칙을 이용합니다!
                var fileName = typeof(T).Name.Remove(0, "SD".Length);
                var json = File.ReadAllText($"{path}/{fileName}.json");
                data = SerializationUtil.FromJson<T>(json);
            }
        }
    }
}