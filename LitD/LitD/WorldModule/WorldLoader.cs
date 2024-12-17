using Microsoft.Xna.Framework;
using System;
using System.Text.RegularExpressions;
using System.IO;
using ProtoBuf;
using LitD.WorldModule.Entities.Placeable;
using System.Collections.Generic;

namespace LitD.WorldModule
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
            try
            {
                Regex forbiddenChars = new Regex("[/:]");
                string worldDirectory = $"Saves/{forbiddenChars.Replace(DateTime.Now.ToString(), "_")}";
                string worldFile = $"{worldDirectory}/data.dat";
                string chunkFile = $"{worldDirectory}/chunks.dat";

                Directory.CreateDirectory(worldDirectory);
                File.Create(worldFile).Close();
                File.Create(chunkFile).Close();

                Chunk firstChunk = ChunkGenerator.GenerateChunk(
                    new Vector2(0, 0)
                );

                World world = new World("test world", worldDirectory);
                world.AddChunk(firstChunk);

                using (FileStream writer = new FileStream(worldFile, FileMode.Open))
                {
                    Serializer.Serialize<World>(writer, world);
                }
                using (FileStream writer = new FileStream(chunkFile, FileMode.Open))
                {
                    Serializer.Serialize<List<Chunk>>(writer, world.GetLoadedChunks());
                }

                return worldDirectory;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create new world!\n{e}");
            }
        }

        /// <summary> Загрузка существующего мира из файла. </summary>
        public static void LoadWorld(string worldDirectory, out World world)
        {
            using (FileStream fileStream = new FileStream(Path.Combine(worldDirectory, "data.dat"), FileMode.Open))
            {
                world = Serializer.Deserialize<World>(fileStream);
            }

            using (FileStream fileStream = new FileStream(Path.Combine(worldDirectory, "chunks.dat"), FileMode.Open))
            {
                List<Chunk> chunks = Serializer.Deserialize<List<Chunk>>(fileStream);
            
                foreach (Chunk chunk in chunks)
                {
                    world.AddChunk(chunk);
                    chunk.InitializeEntitySprites();
                }
            }

            world.InitializeStream(); // открываем файл с чанками, чтобы динамически их подгружать
        }
    }
}
