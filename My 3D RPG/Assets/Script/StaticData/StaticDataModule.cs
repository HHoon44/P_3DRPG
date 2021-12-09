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
    [Serializable]
    public class StaticDataModule
    {
        public List<SDCharacter> sdCharacters;
        public List<SDStage> sdStages;
        public List<SDNovel> sdNovels;
        public List<SDOriginInfo> sdOriginInfos;
        public List<SDFormInfo> sdFormInfos;
        public List<SDWeapon> sdWeapons;
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
            loader.Load(out sdWeapons);
            loader.Load(out sdMonsters);
            loader.Load(out sdItems);
            loader.Load(out sdNPCs);
            loader.Load(out sdQuests);
            loader.Load(out sdQuestSpeechs);
        }

        private class StaticDataLoader
        {
            private string path;

            public StaticDataLoader()
            {
                path = $"{Application.dataPath}/StaticData/Json";
            }

            public void Load<T>(out List<T> data)
                where T : StaticData
            {
                var fileName = typeof(T).Name.Remove(0, "SD".Length);
                var json = File.ReadAllText($"{path}/{fileName}.json");
                data = SerializationUtil.FromJson<T>(json);
            }
        }
    }
}