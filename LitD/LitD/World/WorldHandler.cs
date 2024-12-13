using Microsoft.Xna.Framework;
using System;
using System.Text.RegularExpressions;


namespace LitD.World
{
    internal class WorldHandler
    {
        private const float DefaultTileSize = 32;
        /*
         * todos
         * 1. после генерации записать мир в файл
         * 2. в файл должны записываться чанки по мере генерации
         */

        /// <summary> Создает новый мир. </summary>
        /// <returns> Путь к файлу созданного мира. </returns>
        public static string CreateNew()
        {
            /*
             * 1. создание файла мира
             * 2. генерация первых чанков
             */
            try
            {
                Regex forbiddenChars = new Regex("[/:]");
                string worldFile = $"Saves/{forbiddenChars.Replace(DateTime.Now.ToString(), "_")}.dat";
                System.IO.File.Create(worldFile).Close();

                WorldGenerator.GenerateChunk(
                    worldFile,
                    ConvertScreenToWorldPosition(new Vector2(0, 0))
                );

                return worldFile;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create new world!\n{e}");
            }
        }

        /// <summary> Загрузка существующего мира. </summary>
        public static void LoadWorld()
        {
            // чтение файла мира
        }

        /// <summary> Проверяет существует ли чанк на координатах игрока. </summary>
        public static void IsChunkExists()
        {
            // если нет, то GenerateChunk
        }

        /// <summary> Преобразовывает координаты на экране в мировые координаты. </summary>
        /// <param name="screenPosition"> Координаты на экране. </param>
        /// <returns> Мировые координаты </returns>
        public static Vector2 ConvertScreenToWorldPosition(Vector2 screenPosition)
        {
            return new Vector2(screenPosition.X / DefaultTileSize, screenPosition.Y / DefaultTileSize);
        }
    }
}
