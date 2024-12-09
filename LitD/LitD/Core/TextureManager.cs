using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace LitD.Core
{
    /// <summary> Текстурный менеджер, предназначенный для упрощения работы с текстурами. </summary>
    internal class TextureManager
    {
        /*
         * планируется, что этот менеджер полностью избавит меня от любого гемора, связанного с текстурами.
         * схема работы такая: я загружаю текстурку в контент-менеджер, а потом получаю ее по названию файла.
         */

        private static Dictionary<string, Texture2D> _textureDictionary = new Dictionary<string, Texture2D>();
        private static List<string> _textureNames = new List<string>();

        #region вызываемое извне

        /// <summary> Загружает текстуры. </summary>
        /// <param name="Content"> Контент-менеджер. Передавать прямо из LitDGame. </param>
        public static void LoadTextures(ContentManager Content)
        {
            GetTextureNames(Content.RootDirectory);

            foreach (var textureName in _textureNames)
            {
                _textureDictionary.Add(textureName, Content.Load<Texture2D>(textureName));
            }

            _textureNames.Clear();
        }

        /// <summary> Возвращает текстуру по ее названию. </summary>
        /// <param name="textureName"> Название текстуры. </param>
        /// <returns> Текстура </returns>
        public static Texture2D GetTexture(string textureName)
        {
            return _textureDictionary[textureName];
        }

        #endregion

        #region служебное

        /// <summary> Подгружает имена текстур в соответствие с именами файлов. </summary>
        /// <param name="texRelativeDir"> Относительный путь к контенту. Извлекается из ContentManager. </param>
        private static void GetTextureNames(string texRelativeDir)
        {
            string _texDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, texRelativeDir);

            FileInfo[] files = new DirectoryInfo(_texDir).GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Extension != "png") continue;

                string trimmedName = file.Name.Split('.')[0]; // расширение необходимо убрать, иначе Content.Load не найдет файл
                _textureNames.Add(trimmedName);
            }
        }

        #endregion
    }
}
