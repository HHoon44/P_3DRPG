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
    /// 모든 기획데이터를 들고 있는 클래스
    /// </summary>
    [Serializable]
    public class StaticDataModule
    {
        // public 
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

        /// <summary>
        /// 기획 데이터 초기화 메서드
        /// </summary>
        public void Initialize()
        {
            // 기획 데이터를 불러오는 로더를 생성
            var loader = new StaticDataLoader();

            // out 키워드를 이용해서 데이터를 저장
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
        /// 기획 데이터를 불러올 로더 클래스
        /// </summary>
        private class StaticDataLoader
        {
            // private
            private string path;    // 읽어올 파일이 존재하는 경로

            public StaticDataLoader()
            {
                path = $"{Application.dataPath}/StaticData/Json";
            }

            /// <summary>
            /// 작성해놓은 기획 데이터를 가져오는 메서드
            /// </summary>
            /// <typeparam name="T">    기획 데이터 타입 </typeparam>
            /// <param name="data">     기획 데이터 </param>
            public void Load<T>(out List<T> data) where T : StaticData
            {
                // T타입의 이름에서 SD를 제거하면, 파일이름과 동일하다는 규칙을 이용
                var fileName = typeof(T).Name.Remove(0, "SD".Length);

                // 경로에 존재하는 fileName의 Json 파일을 모두 읽음
                var json = File.ReadAllText($"{path}/{fileName}.json");

                // 읽어온 Json 파일을 역직렬화 하여 저장
                data = SerializationUtil.FromJson<T>(json);
            }
        }
    }
}