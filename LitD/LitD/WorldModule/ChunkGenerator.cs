using LitD.WorldModule.Entities;
using LitD.WorldModule.Entities.Placeable;
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

            public static int WorldSeed;

            public static void Initialize()
            {
                 
                Random rnd = new Random();
                WorldSeed = rnd.Next(int.MinValue, int.MaxValue);
            }

        public static Chunk GenerateChunk(Vector2 worldChunkPosition)
        {
            Chunk chunk = new Chunk(worldChunkPosition);
            Entity entity;

            float noiseScale = 0.01f;
            float baseHeight = 50f;
            float heightVariation = 20f;

            float seedOffsetX = WorldSeed * 10.0f;
            float seedOffsetY = WorldSeed * 10.0f;

            
            float caveNoiseScale = 0.05f;
            float caveThreshold = 0.7f; // Чем ближе к 1, тем реже пещеры

            for (int i = 0; i < WorldConstants.CHUNK_SIZE; i++)
            {
                for (int j = 0; j < WorldConstants.CHUNK_SIZE; j++)
                {
                    float globalX = j + worldChunkPosition.X * WorldConstants.CHUNK_SIZE;
                    float globalY = i + worldChunkPosition.Y * WorldConstants.CHUNK_SIZE;

                    float noiseX = (globalX + seedOffsetX) * noiseScale;
                    float noiseY = (0 + seedOffsetY) * noiseScale;
                    float groundHeight = baseHeight + PerlinNoise.Generate(noiseX, noiseY) * heightVariation;

                    string texture;

                    if (globalY > groundHeight + 1)
                    {
                     
                        float caveNoiseX = (globalX + seedOffsetX) * caveNoiseScale;
                        float caveNoiseY = (globalY + seedOffsetY) * caveNoiseScale;
                        float caveValue = PerlinNoise.Generate(caveNoiseX, caveNoiseY);
                        caveValue = (caveValue + 1f) / 2f;  

                        if (caveValue > caveThreshold)
                        {
                             
                            texture = "Air";
                        }
                        else
                        {
                             
                            texture = "Dirt";
                        }
                    }
                    else if (Math.Abs(globalY - groundHeight) <= 1)
                    {
                        texture = "Grass";
                    }
                    else
                    {
                        texture = "Air";
                    }

                    entity = new TileEntity(
                        texture,
                        new Vector2(
                            j * WorldConstants.DEFAULT_TILE_SIZE + WorldConstants.CHUNK_SIZE_IN_PIXELS * worldChunkPosition.X,
                            i * WorldConstants.DEFAULT_TILE_SIZE + WorldConstants.CHUNK_SIZE_IN_PIXELS * worldChunkPosition.Y
                        )
                    );

                    chunk.SetTile(entity, new Vector2(j, i));
                }
            }

            return chunk;
        }

    }


}
