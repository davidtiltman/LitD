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
        public static void UpdateVisibleChunks(Vector2 observerPosition, ref List<Region> regions, ref List<Chunk> visibleChunks)
        {
            visibleChunks.Clear();

            // конвертируем координаты наблюдателя в координаты чкнка
            Vector2 observerChunkPosition = new Vector2(
                (float)Math.Floor(observerPosition.X / WorldConstants.CHUNK_SIZE_IN_PIXELS),
                (float)Math.Floor(observerPosition.Y / WorldConstants.CHUNK_SIZE_IN_PIXELS)
            );

            int observerRegionX = GetRegionXByChunkGlobalX((int)observerChunkPosition.X);
            int drawDistanceInRegions = (int)Math.Ceiling((decimal)WorldConstants.CHUNK_DRAW_DISTANCE / (decimal)WorldConstants.REGION_WIDTH);

            /*
             * NOT IMPLEMENTED
             * TODO: разработать алгоритм, который будет подтягивать ТОЛЬКО НОВЫЕ видимые чанки,
             * никак не взаимодействуя с чанками, которые по-прежнему видны.
            
            Vector2 previousObserverChunkLocation = new Vector2(
                (float)Math.Floor(previousObserverPosition.X / WorldConstants.CHUNK_SIZE_IN_PIXELS),
                (float)Math.Floor(previousObserverPosition.Y / WorldConstants.CHUNK_SIZE_IN_PIXELS)
            );
            

            // направление движения наблюдателя. <0 = влево/вниз, >0 = вправо/вверх
            int dx = (int)(observerChunkPosition.X - previousObserverChunkLocation.X);
            int dy = (int)(observerChunkPosition.Y - previousObserverChunkLocation.Y);

            if (dx == 0) return; // игрок не сдвинулся. чанки не обновляем.
            */

            // вертикальные границы прорисовки
            int topY = (int)(observerChunkPosition.Y + WorldConstants.CHUNK_DRAW_DISTANCE);
            int bottomY = (int)(observerChunkPosition.Y - WorldConstants.CHUNK_DRAW_DISTANCE);

            // границы по регионам
            int leftRegion = observerRegionX - drawDistanceInRegions;
            int rightRegion = observerRegionX + drawDistanceInRegions;

            List<Chunk> nowVisible = new List<Chunk>();

            // проходимся по всем регионам в пределах видимости
            for (int regPos = leftRegion; regPos <= rightRegion; regPos++)
            {
                Region selectedRegion = regions.FirstOrDefault(region => region?.Position == regPos);
                if (selectedRegion != null)
                {
                    try
                    {
                        // и берем из этих регионов все чанки, попадающие в отрезок видимости по Y
                        Chunk[] inRangeY = selectedRegion.GetChunkArrayInYRange(topY, bottomY);

                        foreach (Chunk chunk in inRangeY)
                        {
                            if (Vector2.Distance(observerChunkPosition, chunk.Position) <= WorldConstants.CHUNK_DRAW_DISTANCE)
                                visibleChunks.Add(chunk);
                        }

                    }
                    catch (Exception e)
                    {
                        throw new InvalidOperationException($"Failed to get visible chunks\n{e}");
                    }
                }
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
