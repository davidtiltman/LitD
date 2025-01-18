using Microsoft.Xna.Framework;
using ProtoBuf;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace LitD.WorldModule.WorldStructure.WorldHandlers
{
    /// <summary> Обработчик подгрузки регионов. </summary>
    internal class LoadHandler
    {
        private static Vector2 _observerPosition;
        private static string _worldDirectory;
        private static bool _isRunning = false;

        private static Thread _loaderThread;

        public static void Start()
        {
            if (!_isRunning)
            {
                _isRunning = true;

                _loaderThread = new Thread(() => LoadRegions());
                _loaderThread.Start();
            }
        }

        public static void Stop()
        {
            _isRunning = false;
        }

        public static void UpdateData(Vector2 observerPosition, string worldDirectory)
        {
            _observerPosition = observerPosition;
            _worldDirectory = worldDirectory;
        }

        /// <summary> Загружает регионы рядом с наблюдателем. </summary>
        /// <param name="observerPosition"></param>
        private static void LoadRegions()
        {
            while (_isRunning)
            {
                // проверяем, есть ли на координатах наблюдателя регион
                // конвертируем координаты наблюдателя в координаты региона
                int observerRegionLocation = (int)_observerPosition.X / (WorldConstants.REGION_WIDTH * WorldConstants.CHUNK_SIZE_IN_PIXELS);

                // подгружаем соседние от наблюдателя регионы
                int regionX = observerRegionLocation - WorldConstants.NEAR_REGIONS_LOAD_DISTANCE;
                int distance = observerRegionLocation + WorldConstants.NEAR_REGIONS_LOAD_DISTANCE;

                for (; regionX < distance; regionX++)
                {
                    Region region = IsRegionExistsOnDisk(regionX, _worldDirectory);

                    if (region != null)
                    {
                        // регион существует
                        RegionLoadQueue.Push(region);
                    }
                    else
                    {
                        // регион НЕ существует
                        Region newRegion = new Region(regionX);
                        RegionLoadQueue.Push(newRegion);
                    }
                }
            }
        }

        /// <summary>
        /// Проверяет, существует ли регион на указанных коориданах. </summary>
        /// <param name="position"> Кооридинаты региона. </param>
        /// <returns> Регион, если он существует. Null, если нет. </returns>
        private static Region IsRegionExistsOnDisk(int xPosition, string worldDirectory)
        {
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
    }
}
