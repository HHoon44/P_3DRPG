using ProjectChan.Define;
using ProjectChan.Object;
using ProjectChan.UI;
using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

namespace ProjectChan.Resource
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        public void Initialize()
        {
            LoadAllAtlas();
            LoadAllPrefabs();
        }

        public GameObject LoadObject(string path)
        {
            return Resources.Load<GameObject>(path);
        }

        private void LoadAllAtlas()
        {
            var portraitAtlase = Resources.LoadAll<SpriteAtlas>("Atlase/PortraitAtlase");
            SpriteLoader.SetAtlas(portraitAtlase);

            var uiAtlase = Resources.LoadAll<SpriteAtlas>("Atlase/UIAtlase");
            SpriteLoader.SetAtlas(uiAtlase);

            var itemAtlase = Resources.LoadAll<SpriteAtlas>("Atlase/ItemAtlase");
            SpriteLoader.SetAtlas(itemAtlase);
        }

        public void LoadAllPrefabs()
        {
            LoadPoolableObject<MonHpBar>(PoolType.MonHpBar, "Prefabs/UI/MonHpBar", 10);
            LoadPoolableObject<DialogueButton>(PoolType.DialogueButton, "Prefabs/UI/DialogueButton", 10);
            LoadPoolableObject<QuestSlot>(PoolType.QuestSlot, "Prefabs/UI/QuestSlot", 10);
            LoadPoolableObject<Object.Item>(PoolType.Item, "Prefabs/Item/Potion", 10);
        }

        public void LoadBackGround()
        {
            var backAtlase = Resources.LoadAll<SpriteAtlas>("Atlase/BackAtlase");
            SpriteLoader.SetAtlas(backAtlase);
        }

        public void LoadPoolableObject<T>(PoolType poolType, string path, int poolCount = 1, Action loadComplete = null)
            where T : MonoBehaviour, IPoolableObject
        {
            var obj = LoadObject(path);

            var tComponent = obj.GetComponent<T>();

            ObjectPoolManager.Instance.RegistPool<T>(poolType, tComponent, poolCount);

            loadComplete?.Invoke();
        }
    }
}