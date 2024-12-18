﻿using System;
using System.Text.RegularExpressions;

namespace LitD.World
{
    internal class WorldManager
    {
        /*
         * todos
         * 1. после генерации записать мир в файл
         * 2. в файл должны записываться чанки по мере генерации
         */

        /// <summary> Создает новый мир. </summary>
        public static void CreateNewWorld()
        {
            /*
             * 1. создание файла мира
             * 2. генерация первых чанков
             */
            try
            {
                Regex forbiddenChars = new Regex("[/:]");
                string worldFile = $"Saves/{forbiddenChars.Replace(DateTime.Now.ToString(), "_")}.dat";
                System.IO.File.Create(worldFile);
            }
            catch
            {
                throw new Exception("Failed to create new world");
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

        /// <summary> Генерация нового чанка. </summary>
        public static void GenerateChunk(string worldFile)
        {
            const int CHUNK_SIZE = 32;
        }
    }
}
