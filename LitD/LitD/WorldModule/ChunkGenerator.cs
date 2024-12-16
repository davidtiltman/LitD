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
            Chunk chunk = new Chunk(new Vector2(0, 0));
            Entity entity;

         
            float noiseScale = 0.01f;

            for (int i = 0; i < WorldConstants.CHUNK_SIZE; i++)
            {
                for (int j = 0; j < WorldConstants.CHUNK_SIZE; j++)
                {

                    /* эта хуйня нужна чтобы учитывать окружение, я знаю, что ты сказал, что это не должно быть внутри чанка но мне похуй, я не понял как это по другому сделать
                     типо шум работает со всей картой и эти координаты нужны, чтобы учитывать что сгенерировалось в предыдущих чанках*/
                    float globalX = j + worldChunkPosition.X * WorldConstants.CHUNK_SIZE; 
                    float globalY = i + worldChunkPosition.Y * WorldConstants.CHUNK_SIZE;

                    /*это уровни шума, ну типо первый то, что чаще встречаеться, второй то что реже и т.д. Коэф у каждого уровня влияет на частоту(эти значения связаны
                     с DetermineTexture*/
                    float noiseValue = (
                        0.9f * PerlinNoise.Generate(globalX * noiseScale, globalY * noiseScale) +
                        0.6f * PerlinNoise.Generate(globalX * noiseScale * 2, globalY * noiseScale * 2) +
                        0.3f * PerlinNoise.Generate(globalX * noiseScale * 4, globalY * noiseScale * 4)
);                  //нормализация,пушто шум генерит от -1 до 1
                    noiseValue = (noiseValue + 1) / 2f; 

                 
                    string texture = DetermineTexture(noiseValue);

                    // Создание тайла
                    entity = new TileEntity(
                        texture,
                        new Vector2(
                            j * WorldConstants.DEFAULT_TILE_SIZE + WorldConstants.CHUNK_SIZE_IN_PIXELS * worldChunkPosition.X + 400,
                            i * WorldConstants.DEFAULT_TILE_SIZE + WorldConstants.CHUNK_SIZE_IN_PIXELS * worldChunkPosition.Y + 300
                        )
                    );

                    chunk.SetTile(entity, new Vector2(j, i));
                }
            }
            return chunk;
        }

        /// <summary>
        /// Определяет текстуру на основе значения шума.
        /// </summary>
        private static string DetermineTexture(float noiseValue)
        {
            //ну ты понял
            if (noiseValue < 0.5f)
                return "w"; 
            else if (noiseValue < 0.55f)
                return "Grass"; 
            else
                return "Dirt";  
        }
    }
}
