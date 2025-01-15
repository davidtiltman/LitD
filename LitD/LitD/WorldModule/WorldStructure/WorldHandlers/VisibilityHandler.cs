using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LitD.WorldModule.WorldStructure.WorldServices
{
    internal class VisibilityHandler
    {
        /// <summary> Выделенная логика обновления видимых чанков. </summary>
        /// <param name="observerPosition"></param>
        public static void UpdateVisibleChunks(Vector2 observerPosition, ref List<Region> regions, ref List<Chunk> chunks)
        {
            chunks.Clear();

            // конвертируем координаты наблюдателя в глобальные координаты чкнка
            var observerChunkLocation = new Vector2(
                (float)Math.Floor(observerPosition.X / WorldConstants.CHUNK_SIZE_IN_PIXELS),
                (float)Math.Floor(observerPosition.Y / WorldConstants.CHUNK_SIZE_IN_PIXELS)
            );

            // регион наблюдателя
            int observerRegionLocation = GetRegionXByChunkGlobalX((int)observerChunkLocation.X);

            int drawDistanceInRegions = (int)Math.Ceiling((decimal)WorldConstants.CHUNK_DRAW_DISTANCE / (decimal)WorldConstants.REGION_WIDTH);
            int leftRegion = observerRegionLocation - drawDistanceInRegions;
            int rightRegion = observerRegionLocation + drawDistanceInRegions;

            // границы прямоугольника отрисовки
            int topY    = (int)observerChunkLocation.Y - WorldConstants.CHUNK_DRAW_DISTANCE;
            int bottomY = (int)observerChunkLocation.Y + WorldConstants.CHUNK_DRAW_DISTANCE;

            if (Math.Abs(topY) > Math.Abs(WorldConstants.WORLD_HIGHEST_CHUNK)) 
                topY = WorldConstants.WORLD_HIGHEST_CHUNK;
            if (Math.Abs(bottomY) > Math.Abs(WorldConstants.WORLD_LOWEST_CHUNK)) 
                bottomY = WorldConstants.WORLD_LOWEST_CHUNK;

            try 
            {
                for (int x = leftRegion; x <= rightRegion; x++)
                {
                    Region currentRegion = regions.FirstOrDefault(r => r.Position == x);

                    if (currentRegion != null)
                    {
                        Chunk[] chunksInYRange = currentRegion.GetChunkArrayInYRange(topY, bottomY);

                        foreach (Chunk chunk in chunksInYRange)
                            chunks.Add(chunk);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update visible chunks: {ex}");
            }
        }

        /// <summary> Возвращает X региона по глобальной X чанка. </summary>
        /// <param name="chunkGlobalX"> Глобальная X чанка. </param>
        /// <returns> X региона. </returns>
        private static int GetRegionXByChunkGlobalX(int chunkGlobalX)
        {
            int absoluteChunkX = Math.Abs(chunkGlobalX);
            int regionX = absoluteChunkX / WorldConstants.REGION_WIDTH;

            return chunkGlobalX < 0 ? -regionX : regionX;
        }
    }
}
