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
    /// <summary>
    /// => 게임에 사용되는 모든 아틀라스를 관리하는 클래스
    /// => 런타임에 생성되는 스프라이트는 모두 해당 클래스를 통해서 가져온다
    /// </summary>
    public static class SpriteLoader
    {
        /// <summary>
        /// => UIStart, UILoading에서 뒷배경을 가져올 때 사용할 인덱스 프로퍼티
        /// </summary>
        public static int atlasIndex { get; private set; }

        /// <summary>
        /// => 모든 아틀라스들을 관리할 딕셔너리
        /// </summary>
        private static Dictionary<AtlasType, SpriteAtlas> atlasDic = new Dictionary<AtlasType, SpriteAtlas>();

        /// <summary>
        /// => 매개변수로 받은 아틀라스 목록의 아틀라스들을 딕셔너리에 등록하는 메서드
        /// </summary>
        /// <param name="atlases"> 등록하고자 하는 아틀라스 목록 </param>
        public static void SetAtlas(SpriteAtlas[] atlases)
        {
            for (int i = 0; i < atlases.Length; i++)
            {
                // -> 딕셔너리의 Key값으로 사용하기 위해서 형변환
                var key = (AtlasType)Enum.Parse(typeof(AtlasType), atlases[i].name);

                // -> 만약 Key값이 백그라운드 라면!
                if (key == AtlasType.BackGround)
                { 
                    // -> 아틀라스의 스프라이트 개수를 저장한다
                    atlasIndex = atlases[i].spriteCount;
                }

                atlasDic.Add(key, atlases[i]);
            }
        }

        /// <summary>
        /// => 특정 아틀라스에서 원하는 스프라이트를 찾아서 반환하는 메서드
        /// </summary>
        /// <param name="type"> 찾고자 하는 스프라이트가 들어있는 아틀라스의 키 값 </param>
        /// <param name="spriteName"> 찾고자 하는 스프라이트의 이름 </param>
        /// <returns></returns>
        public static Sprite GetSprite(AtlasType type, string spriteName)
        {
            // -> 원하는 스프라이트가 없다면!
            if (!atlasDic.ContainsKey(type))
            {
                return null;
            }

            // -> Key값을 통해 선언한 Dictionary에 접근
            // -> Key값 안에 있는 SpriteAtlas에 접근하여 SpriteAtlas안에 있는 GetSprite에 접근
            // -> GetSprite의 인자로 spriteName을 전달하여 원하는 Sprite를 반환
            return atlasDic[type].GetSprite(spriteName);
        }
    }
}