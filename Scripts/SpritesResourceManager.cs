using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace zm
{
    public class SpritesResourceManager : SingleBase<SpritesResourceManager>
    {
        public struct SpritesResourceInfo
        {
            public string Key;

            public string ResourcePath;
        }

        public Sprite[] TechnicalPointSprites, TechnicalPointHighlightSprites;

        public Dictionary<string, Sprite[]> LoadSpritesDictionary = new Dictionary<string, Sprite[]>();

        public void Init(params SpritesResourceInfo[] spritesResourceInfos)
        {
            foreach (var info in spritesResourceInfos)
            {
                var sprites = Resources.LoadAll<Sprite>(info.ResourcePath);
                LoadSpritesDictionary.Add(info.Key, sprites);
            }
        }


        public Sprite GetLoadSprite(string key, string spriteName)
        {
            foreach (var sprite in LoadSpritesDictionary[key])
            {
                if (sprite.name == spriteName)
                {
                    return sprite;
                }
            }
            return default(Sprite);
        }
    }
}