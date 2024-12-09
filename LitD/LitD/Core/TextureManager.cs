using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace LitD.Core
{
    internal class TextureManager
    {
        private static Dictionary<string, Texture2D> _textureDictionary = new Dictionary<string, Texture2D>();

        /// <summary> Загружает текстуры </summary>
        /// <param name="Content"> Контент-менеджер. Передавать прямо из LitDGame </param>
        public static void LoadTextures(ContentManager Content)
        {
            _textureDictionary.Add("Grass", Content.Load<Texture2D>("Grass"));
        }

        /// <summary> Возвращает текстуру по ее названию </summary>
        /// <param name="textureName"> Название текстуры </param>
        /// <returns> Текстура </returns>
        public static Texture2D GetTexture(string textureName)
        {
            return _textureDictionary[textureName];
        }
    }
}
