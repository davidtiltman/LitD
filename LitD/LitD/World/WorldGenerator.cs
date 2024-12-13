using Microsoft.Xna.Framework;
using System.IO;

namespace LitD.World
{
    internal class WorldGenerator
    {
        /// <summary>
        /// Генерация нового чанка.<br/>
        /// Чанк генерируется из левого верхнего угла вправо вниз.
        /// </summary>
        public static void GenerateChunk(string worldFile, Vector2 worldChunkPosition)
        {
            const int CHUNK_SIZE = 16;

            using (StreamWriter output = new StreamWriter(worldFile))
            {
                for (int i = 0; i < CHUNK_SIZE; i++)
                {
                    for (int j = 0; j < CHUNK_SIZE; j++)
                    {
                        output.Write($"[{j};{i}]\t");
                    }
                    output.Write("\n\n\n");
                }
            }
        }
    }
}
