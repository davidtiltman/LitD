using Microsoft.Xna.Framework;
using System.IO;

namespace LitD.World
{
    /// <summary> Отвечает за генерацию мира. </summary>
    internal class WorldGenerator
    {
        /// <summary>
        /// Генерация нового чанка.<br/>
        /// Чанк генерируется из левого верхнего угла вправо вниз.
        /// </summary>
        public static void GenerateChunk(string worldFile, Vector2 worldChunkPosition)
        {
            using (StreamWriter output = new StreamWriter(worldFile))
            {
                for (int i = 0; i < WorldConstants.CHUNK_SIZE; i++)
                {
                    for (int j = 0; j < WorldConstants.CHUNK_SIZE; j++)
                    {
                        output.Write($"[{j};{i}]\t");
                    }
                    output.Write("\n\n\n");
                }
            }
        }
    }
}
