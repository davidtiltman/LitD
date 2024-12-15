using LitD.WorldModule.Entities;
using Microsoft.Xna.Framework;
using System;

namespace LitD.WorldModule
{
    /// <summary> Отвечает за генерацию чанков. </summary>
    internal class ChunkGenerator
    {
        /// <summary>
        /// Генерирует новый чанк.<br/>
        /// Чанк генерируется из левого верхнего угла вправо вниз.
        /// </summary>
        /// <param name="worldChunkPosition">
        /// Позиция чанка в мире.<br/>
        /// Чанки имеют собственные координаты с шагом в единицу.
        /// </param>
        public static Chunk GenerateChunk(Vector2 worldChunkPosition)
        {
            Chunk chunk = new Chunk();
            Entity entity;

            for (int i = 0; i < WorldConstants.CHUNK_SIZE; i++)
            {
                for (int j = 0; j < WorldConstants.CHUNK_SIZE; j++)
                {
                    Vector2 tilePos = new Vector2(j, i);

                    // тупо рандомные тайлы генерируются
                    int r = Random.Shared.Next(3);
                    string texture = string.Empty;
                    switch (r)
                    {
                        case 0:
                            texture = "Grass";
                            break;
                        case 1:
                            texture = "Dirt";
                            break;
                        case 2:
                            texture = "w";
                            break;
                    }

                    entity = new TileEntity(
                         texture,
                         new Vector2(
                             j * WorldConstants.DEFAULT_TILE_SIZE + WorldConstants.CHUNK_SIZE_IN_PIXELS * worldChunkPosition.X + 300,
                             i * WorldConstants.DEFAULT_TILE_SIZE + WorldConstants.CHUNK_SIZE_IN_PIXELS * worldChunkPosition.Y + 150
                         )
                     );
                    // =========================================

                    chunk.SetTile(entity, new Vector2(j, i));
                }
            }
            return chunk;
        }
    }
}
