using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;


namespace LitD.Core.Textures
{
    /// <summary>
    /// Текстурный менеджер, предназначенный для упрощения работы с текстурами.<br />
    /// Перед началом работы следует его инициализировать, использовав метод Init.
    /// </summary>
    internal class TextureManager
    {
        /*
         * планируется, что этот менеджер полностью избавит меня от любого гемора, связанного с текстурами.
         * схема работы такая: я загружаю текстурку в контент-менеджер, а потом получаю ее по названию файла.
         */

        // служебные сущности
        private static ContentManager ContentManager { get; set; }
        private static GraphicsDevice GraphicsDevice { get; set; }
        private static bool IsInit { get; set; } = false;
        private static string SpritesFolder = "Sprites/";

        // хранилище текстур
        private static Dictionary<string, Texture2D> _textureDictionary = new Dictionary<string, Texture2D>();

        #region вызываемое извне

        /// <summary> Инициализирует менеджер. </summary>
        /// <param name="contentManager"> Контент-менеджер Monogame. </param>
        /// <param name="graphicsDevice"> Графическое устройство Monogame. </param>
        public static void Init(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            ContentManager = contentManager;
            GraphicsDevice = graphicsDevice;

            IsInit = true;
        }

        /// <summary> Загружает текстуры. </summary>
        /// <param name="Content"> Контент-менеджер. Передавать прямо из LitDGame. </param>
        public static void LoadTextures()
        {
            CheckInit();

            List<string> _textureNames;
            GetTextureNames(out _textureNames);

            foreach (var textureName in _textureNames)
            {
                _textureDictionary.Add(textureName, ContentManager.Load<Texture2D>(String.Concat(SpritesFolder, textureName)));
            }

            _textureNames.Clear();
        }

        /// <summary> Возвращает текстуру по ее названию. </summary>
        /// <param name="textureName"> Название текстуры. </param>
        /// <returns> Текстура </returns>
        public static Texture2D GetTexture(string textureName)
        {
            CheckInit();

            if (_textureDictionary.ContainsKey(textureName))
            {
                return _textureDictionary[textureName];
            }
            else
            {
                Texture2D _emptyTexture = new Texture2D(GraphicsDevice, 32, 32);

                Color[] colorData = new Color[32 * 32];
                for (int i = 0; i < colorData.Length; i++) colorData[i] = Color.Purple;

                _emptyTexture.SetData(colorData);

                return _emptyTexture;
            }
        }

        #endregion

        #region служебное

        /// <summary> Проверяет менеджер на инициализацию. </summary>
        /// <exception cref="InvalidOperationException"> Если не инициализирован, то бросатеся исключение. </exception>
        private static void CheckInit()
        {
            if (!IsInit)
            {
                throw new InvalidOperationException("TextureManager is not initialized");
            }
        }

        /// <summary> Подгружает имена текстур в соответствие с именами файлов. </summary>
        /// <param name="texRelativeDir"> Относительный путь к контенту. Извлекается из ContentManager. </param>
        private static void GetTextureNames(out List<string> textureNames)
        {
            textureNames = new List<string>();

            string _texDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ContentManager.RootDirectory, SpritesFolder);
            FileInfo[] _files = new DirectoryInfo(_texDir).GetFiles();

            foreach (FileInfo file in _files)
            {
                string trimmedName = file.Name.Split('.')[0]; // расширение необходимо убрать, иначе Content.Load не найдет файл
                textureNames.Add(trimmedName);
            }
        }

        #endregion
    }
}
