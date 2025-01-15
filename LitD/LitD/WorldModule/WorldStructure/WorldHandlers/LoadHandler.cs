using Microsoft.Xna.Framework;
using ProtoBuf;
using System.Collections.Generic;
using System.IO;

namespace LitD.WorldModule.WorldStructure.WorldServices
{
    /// <summary> Загрзузчик/отгрузчик регионов. </summary>
    internal class LoadHandler
    {
        #region Load logic

        /// <summary> Загружает регионы рядом с наблюдателем. </summary>
        /// <param name="observerPosition"></param>
        public static void LoadRegions(Vector2 observerPosition, ref List<Region> regions, string worldDirectory)
        {
            // проверяем, есть ли на координатах наблюдателя регион
            // конвертируем координаты наблюдателя в координаты региона
            int observerRegionLocation = (int)observerPosition.X / (WorldConstants.REGION_WIDTH * WorldConstants.CHUNK_SIZE_IN_PIXELS);

            // подгружаем соседние от наблюдателя регионы
            int regionX = observerRegionLocation - WorldConstants.NEAR_REGIONS_LOAD_DISTANCE;
            int distance = observerRegionLocation + WorldConstants.NEAR_REGIONS_LOAD_DISTANCE;

            for (; regionX < distance; regionX++)
            {
                Region region = IsRegionExists(regionX, regions, worldDirectory);

                if (region != null)
                {
                    // регион существует И не загружен, добавляем его в список загруженных
                    if (!regions.Contains(region))
                    {
                        AddRegion(region, regions, worldDirectory);
                    }
                }
                else
                {
                    // регион НЕ существует
                    AddRegion(new Region(regionX), regions, worldDirectory);
                }
            }
        }

        /// <summary>
        /// Проверяет, существует ли регион на указанных коориданах. </summary>
        /// <param name="position"> Кооридинаты региона. </param>
        /// <returns> Регион, если он существует. Null, если нет. </returns>
        private static Region IsRegionExists(int xPosition, List<Region> regions, string worldDirectory)
        {
            // сначала ищем в памяти
            foreach (Region region in regions)
            {
                if (region.Position == xPosition) return region;
            }

            // потом на диске
            try
            {
                Region region = null;
                using (FileStream file = new FileStream(Region.GetRegionFilePath(worldDirectory, xPosition), FileMode.Open))
                {
                    region = Serializer.Deserialize<Region>(file, region);
                }
                return region;
            }
            catch
            {
                return null;
            }
        }

        /// <summary> Записывает в мир новый регион. </summary>
        /// <param name="region"> Новый регион. </param>
        private static void AddRegion(Region region, List<Region> regions, string worldDirectory)
        {
            regions.Add(region);
            region.InitializeEntitySprites();
            region.SaveRegion(worldDirectory);
        }

        #endregion

        #region Unload logic

        /// <summary> Отгружает регионы. </summary>
        /// <param name="observerPosition"> Координаты наблюдателя. </param>
        public static void UnloadRegions(GameTime gameTime, Vector2 observerPosition, ref List<Region> regions, string worldDirectory)
        {
            int unloadTimer = (int)(gameTime.TotalGameTime.TotalSeconds) % WorldConstants.REGION_UNLOAD_FREQUENCY;
            if (unloadTimer == 0)
            {
                int observerX = (int)(observerPosition.X / (WorldConstants.REGION_WIDTH * WorldConstants.CHUNK_SIZE_IN_PIXELS));

                for (int i = 0; i < regions.Count; i++)
                {
                    int regionX = regions[i].Position;

                    int loadDistanceMin = observerX - WorldConstants.NEAR_REGIONS_LOAD_DISTANCE;
                    int loadDistanceMax = observerX + WorldConstants.NEAR_REGIONS_LOAD_DISTANCE;

                    if (regionX < loadDistanceMin || regionX > loadDistanceMax)
                    {
                        // если регион находится за пределами дальности подгрузки, то он выгружается
                        regions[i].SaveRegion(worldDirectory);
                        regions.RemoveAt(i);
                    }
                }
            }
        }

        #endregion
    }
}
