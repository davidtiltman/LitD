using Microsoft.Xna.Framework;
using System;
using System.Text.RegularExpressions;

namespace LitD.World
{
    /// <summary> Отвечает за загрузку и создание миров. </summary>
    internal class WorldLoader
    {
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
                    InWorldOperations.ConvertPixelToWorldPosition(new Vector2(0, 0))
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
    }
}
