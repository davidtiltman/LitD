using System;
using LitD.WorldModule.Entities;
using LitD.WorldModule.Entities.Placeable;
using LitD.WorldModule.WorldStructure;
using Microsoft.Xna.Framework;

namespace LitD.WorldModule.WorldGeneration
{
   
    internal class ChunkGenerator
    {
     
            public static int WorldSeed;

            public static void Initialize()
            {
                Random rnd = new Random();
                WorldSeed = rnd.Next(int.MinValue, int.MaxValue);
            }
            // да я сделал метод на хульярд строк, да я знаю, что его надо разделить
            public static Chunk GenerateChunk(Vector2 worldChunkPosition) {
                Chunk chunk = new Chunk(worldChunkPosition);
                Entity entity;

        
                // ОСНОВНЫЕ НАСТРОЙКИ ШУМА

                // Масштаб шума для рельефа > значение > больше искажений
                float terrainNoiseScale = 0.01f;

                // Масштаб шума для биома, > значение чаще сменяется биом
                float biomeNoiseScale = 0.1f;

                // Настройки пещер (второй шум)
                float caveNoiseScale = 0.05f;
                float caveThreshold = 0.7f;

                // Сдвиги по X/Y зависящие от сида
                float seedOffsetX = WorldSeed * 10.0f;
                float seedOffsetY = WorldSeed * 10.0f;

         
                // Генерация чанка
       
                for (int i = 0; i < WorldConstants.CHUNK_SIZE; i++)
                {
                    for (int j = 0; j < WorldConstants.CHUNK_SIZE; j++)
                    {
                        float globalX = j + worldChunkPosition.X * WorldConstants.CHUNK_SIZE;
                        float globalY = i + worldChunkPosition.Y * WorldConstants.CHUNK_SIZE;

                 
                        float biomeNoiseX = (globalX + seedOffsetX) * biomeNoiseScale;
                        float biomeNoiseY = (globalY + seedOffsetY) * biomeNoiseScale;
                        float biomeValue = PerlinNoise.Generate(biomeNoiseX, biomeNoiseY);
                        biomeValue = (biomeValue + 1f) / 2f;  

                 
                        Biome currentBiome;
                        if (biomeValue < 0.33f) currentBiome = Biome.Forest;
                        else if (biomeValue < 0.66f) currentBiome = Biome.Plains;
                        else currentBiome = Biome.Desert;

                 
                        float biomeBaseHeight = 50f;        
                        float biomeHeightVariation = 10f;  
                
                        switch (currentBiome)
                        {
                            case Biome.Forest:
                                biomeBaseHeight = 50f;       
                                biomeHeightVariation = 10f;  
                                break;
                            case Biome.Plains:
                                biomeBaseHeight = 45f;        
                                biomeHeightVariation = 5f;   
                                break;
                            case Biome.Desert:
                                biomeBaseHeight = 50f;
                                biomeHeightVariation = 30f;  
                                break;
                        }

                        float terrainNoiseX = (globalX + seedOffsetX) * terrainNoiseScale;
                        float terrainNoiseY = (0 + seedOffsetY) * terrainNoiseScale; 
                 

                        float terrainNoiseVal = PerlinNoise.Generate(terrainNoiseX, terrainNoiseY);
                        float groundHeight = biomeBaseHeight + terrainNoiseVal * biomeHeightVariation;

                
                        string texture = "skyBlock";

                 
                        if (globalY > groundHeight + 1)
                        {
                    
                            float caveNoiseX = (globalX + seedOffsetX) * caveNoiseScale;
                            float caveNoiseY = (globalY + seedOffsetY) * caveNoiseScale;
                            float caveValue = PerlinNoise.Generate(caveNoiseX, caveNoiseY);
                            caveValue = (caveValue + 1f) / 2f;

                            bool isCave = (caveValue > caveThreshold);
                            if (!isCave)
                            {
                                  
                                float depth = globalY - groundHeight;
                                if (depth < 5) texture = "Dirt";
                                else if (depth < 20) texture = "dirtBlock";
                                else texture = "iceBlock";
                            }
                            else
                            {
                                texture = "skyBlock";
                            }
                        }
                        
                        else if (Math.Abs(globalY - groundHeight) <= 1)
                        {
                    
                            switch (currentBiome)
                            {
                                case Biome.Forest:
                                    texture = "forestBlock";
                                    break;
                                case Biome.Plains:
                                    texture = "Grass";
                                    break;
                                case Biome.Desert:
                                    texture = "sandBlock";
                                    break;
                            }
                        }
                        else
                        {
                             
                            texture = "skyBlock";
                        }

                        
                          entity = new TileEntity(
                            texture,
                            new Vector2(
                                j * WorldConstants.DEFAULT_TILE_SIZE + 
                                    WorldConstants.CHUNK_SIZE_IN_PIXELS * worldChunkPosition.X,
                                i * WorldConstants.DEFAULT_TILE_SIZE + 
                                    WorldConstants.CHUNK_SIZE_IN_PIXELS * worldChunkPosition.Y
                            )
                        );

                        chunk.SetTile(entity, new Vector2(j, i));
                    }
                }
                return chunk;
    }

        }
    }
