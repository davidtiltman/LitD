using Microsoft.Xna.Framework;
using System;
using System.Text.RegularExpressions;
using System.IO;
using ProtoBuf;
using LitD.WorldModule.Entities.Placeable;

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
                string worldFile = $"Saves/{forbiddenChars.Replace(DateTime.Now.ToString(), "_")}.dat";

                File.Create(worldFile).Close();

                Chunk firstChunk = ChunkGenerator.GenerateChunk(
                    new Vector2(0, 0)
                );

                World world = new World("test world");
                for (int y = -1; y <= 1; y++)  
                {
                    for (int x = -3; x <= 3; x++)  
                    {
                        Chunk chunk = ChunkGenerator.GenerateChunk(new Vector2(x, y));
                        world.AddChunk(chunk);
                    }
                }
                using (FileStream writer = new FileStream(worldFile, FileMode.Open))
                {
                    Serializer.Serialize<World>(writer, world);
                }

                return worldFile;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create new world!\n{e}");
            }
        }

        /// <summary> Загрузка существующего мира из файла. </summary>
        public static void LoadWorld(string worldFile, out World world)
        {
            using (FileStream fileStream = new FileStream(worldFile, FileMode.Open))
            {
                world = Serializer.Deserialize<World>(fileStream);
            }

            foreach (Chunk chunk in world.GetChunks())
            {
                foreach (TileEntity tile in chunk.GetTiles())
                {
                    tile.InitializeSprite();
                }
            }
        }
    }
}
