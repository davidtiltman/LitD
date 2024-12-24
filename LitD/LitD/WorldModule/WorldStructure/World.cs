using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;
using LitD.System.Interfaces;
using System;
using System.Linq;

namespace LitD.WorldModule.WorldStructure
{
    /// <summary> Игровой мир, состоящий из чанков. </summary>
    [ProtoContract]
    internal class World : IDebugInfo
    {
        /// <summary> Название мира. </summary>
        [ProtoMember(1)]
        public string Name { get; private set; }

        /// <summary> Все загруженные регионы. </summary>
        private List<Region> _loadedRegions = new List<Region>();

        /// <summary> Все видимые чанки вокруг игрока. </summary>
        private List<Chunk> _visibleChunks = new List<Chunk>();

        [ProtoMember(2)]
        private string _selfDirectory;

        public World(string name, string selfDirectory)
        {
            Name = name;
            _selfDirectory = selfDirectory;
        }

        /// <summary> Пустой конструктор нужен для десериализации. </summary>
        private World()
        { }

        /// <summary>
        /// Проверяет, существует ли регион на указанных коориданах. </summary>
        /// <param name="position"> Кооридинаты региона. </param>
        /// <returns> Регион, если он существует. Null, если нет. </returns>
        private Region IsRegionExists(int xPosition)
        {
            // сначала ищем в памяти
            foreach(Region region in _loadedRegions)
            {
                if (region.Position == xPosition) return region;
            }

            // потом на диске
            try
            {
                Region region = null;
                using (FileStream file = new FileStream(Region.GetRegionFilePath(_selfDirectory, xPosition), FileMode.Open))
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
        public void AddRegion(Region region)
        {
            _loadedRegions.Add(region);
            region.InitializeEntitySprites();
            region.SaveRegion(_selfDirectory);
        }

        /// <summary> Возвращает видимые чанки. </summary>
        /// <returns> Список видимых чанков. </returns>
        public List<Chunk> GetVisibleChunks()
        {
            return _visibleChunks;
        }

        #region Update/Draw

        /// <summary> Выделенная логика обновления видимых чанков. </summary>
        /// <param name="observerPosition"></param>
        private void UpdateVisibleChunks(Vector2 observerPosition)
        {
            // конвертируем координаты наблюдателя в координаты чкнка
            var observerChunkLocation = new Vector2(
                (float)Math.Floor(observerPosition.X / WorldConstants.CHUNK_SIZE_IN_PIXELS),
                (float)Math.Floor(observerPosition.Y / WorldConstants.CHUNK_SIZE_IN_PIXELS)
            );

            // убираем все ранее записанные чанки
            _visibleChunks.Clear();

            foreach (Region region in _loadedRegions)
            {
                foreach (Chunk chunk in region.GetChunks())
                {
                    if (Vector2.Distance(chunk.Position, observerChunkLocation) <= WorldConstants.CHUNK_DRAW_DISTANCE)
                    {
                        chunk.SetVisibility(true);
                        _visibleChunks.Add(chunk);
                    }
                    else
                    {
                        chunk.SetVisibility(false);
                    }
                }
            }
        }

        /// <summary> Выделенная логика отгрузки лишних регионов из памяти. </summary>
        /// <param name="observerPosition"> Координаты наблюдателя. </param>
        private void UnloadRegions(Vector2 observerPosition)
        {
            int observerX = (int)(observerPosition.X / (WorldConstants.REGION_WIDTH * WorldConstants.CHUNK_SIZE_IN_PIXELS));

            for (int i = 0; i < _loadedRegions.Count; i++)
            {
                int regionX = _loadedRegions[i].Position;

                int loadDistanceMin = observerX - WorldConstants.NEAR_REGIONS_LOAD_DISTANCE;
                int loadDistanceMax = observerX + WorldConstants.NEAR_REGIONS_LOAD_DISTANCE;

                if (regionX < loadDistanceMin || regionX > loadDistanceMax)
                {
                    // если регион находится за пределами дальности подгрузки, то он выгружается
                    //_loadedRegions[i].SaveRegion(_selfDirectory); // пока регионы чанки (следовательно регионы) нельзя никак изменять, поэтому нет нужды их сохранять
                    _loadedRegions.RemoveAt(i);
                }
            }
        }

        /// <summary> Выделенная логика подгрузки регионов. </summary>
        /// <param name="observerPosition"></param>
        private void LoadRegions(Vector2 observerPosition)
        {
            // проверяем, есть ли на координатах наблюдателя регион
            // конвертируем координаты наблюдателя в координаты региона
            int observerRegionLocation = (int)observerPosition.X / (WorldConstants.REGION_WIDTH * WorldConstants.CHUNK_SIZE_IN_PIXELS);

            // подгружаем соседние от наблюдателя регионы
            for (
                    int regionLocation = observerRegionLocation - WorldConstants.NEAR_REGIONS_LOAD_DISTANCE;
                    regionLocation < observerRegionLocation + WorldConstants.NEAR_REGIONS_LOAD_DISTANCE;
                    regionLocation++
                )
            {
                Region region = IsRegionExists(regionLocation);
                if (region != null)
                {
                    // регион существует И не загружен, то добавляем его в список загруженных
                    if (!_loadedRegions.Contains(region))
                    {
                        AddRegion(region);
                    }
                }
                else
                {
                    // регион НЕ существует
                    AddRegion(new Region(regionLocation));
                }
            }
        }

        /// <summary>
        /// Обновляет состояние мира.
        /// </summary>
        /// <param name="gameTime"> Игровое время. </param>
        /// <param name="observerRegionPosition"> Координаты наблюдателя. </param>
        public void Update(GameTime gameTime, Vector2 observerPosition)
        {
            LoadRegions(observerPosition);
            UpdateVisibleChunks(observerPosition);

            if ((int)(gameTime.TotalGameTime.TotalSeconds) % WorldConstants.REGION_UNLOAD_FREQUENCY == 0)
            {
                UnloadRegions(observerPosition);
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 observerPosition)
        {
            foreach (var chunk in GetVisibleChunks())
            {
                chunk.Draw(spriteBatch, gameTime);
            }
        }

        #endregion

        #region IDebugInfo

        public void GetDebugInfo(ref string debugInfo)
        {
            if (!string.IsNullOrEmpty(debugInfo))
            {
                debugInfo += "\n";
            }

            debugInfo += "World:\n";
            if (_loadedRegions.Count > 0)
            {
                int regionHeight = Math.Abs(WorldConstants.WORLD_LOWEST_CHUNK) + Math.Abs(WorldConstants.WORLD_HIGHEST_CHUNK); 
                debugInfo += $"\tLoaded regions:{_loadedRegions.Count} (A region contains {WorldConstants.REGION_WIDTH * regionHeight} chunks)\n";
            }
            else
            {
                debugInfo += "\tNo chunks loaded\n";
            }

            if (_visibleChunks.Count > 0)
            {
                debugInfo += $"\tVisible chunks: {_visibleChunks.Count}";
            }
            else
            {
                debugInfo += "\tNo chunks visible";
            }
        }

        #endregion
    }
}
