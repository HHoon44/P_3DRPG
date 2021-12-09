using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using static ProjectChan.Define.Resource;

namespace ProjectChan.Resource
{
    public static class SpriteLoader
    {
        private static Dictionary<AtlasType, SpriteAtlas> atlasDic = new Dictionary<AtlasType, SpriteAtlas>();

        public static int atlasIndex { get; private set; }

        public static void SetAtlas(SpriteAtlas[] atlases)
        {
            for (int i = 0; i < atlases.Length; i++)
            {
                var key = (AtlasType)Enum.Parse(typeof(AtlasType), atlases[i].name);

                if (key == AtlasType.BackGround)
                    atlasIndex = atlases[i].spriteCount;

                atlasDic.Add(key, atlases[i]);
            }
        }

        public static Sprite GetSprite(AtlasType type, string spriteName)
        {
            if (!atlasDic.ContainsKey(type))
            {
                return null;
            }

            return atlasDic[type].GetSprite(spriteName);
        }
    }
}