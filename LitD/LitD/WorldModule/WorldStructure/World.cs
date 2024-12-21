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
        private Region IsRegionExists(Vector2 position)
        {
            // сначала ищем в памяти
            foreach(Region region in _loadedRegions)
            {
                if (region.Position == position) return region;
            }

            // потом на диске
            try
            {
                Region region = null;
                using (FileStream file = new FileStream(Region.GetRegionFilePath(_selfDirectory, position), FileMode.Open))
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

        /// <summary> Выделение видимых чанков из всех загруженных. </summary>
        public List<Chunk> FilterVisibleChunks(Vector2 observerPosition)
        {
            _visibleChunks.Clear();
            List<Chunk> visibleChunks = new List<Chunk>();

            foreach (Region region in _loadedRegions)
            {
                foreach (Chunk chunk in region.GetChunks())
                {
                    if (Vector2.Distance(chunk.Position, observerPosition) <= WorldConstants.CHUNK_DRAW_DISTANCE)
                    {
                        chunk.SetVisibility(true);
                        visibleChunks.Add(chunk);
                    }
                    else
                    {
                        chunk.SetVisibility(false);
                    }
                }
            }

            List<Chunk> t = new List<Chunk>();
            foreach (Region region in _loadedRegions)
            {
                foreach (Chunk chunk in region.GetChunks())
                {
                    t.Add(chunk);
                }
            }

            return visibleChunks;
        }

        public List<Chunk> GetVisibleChunks()
        {
            return _visibleChunks;
        }

        #region Update/Draw

        /// <summary> Выделенная логика обновления видимых чанков. </summary>
        /// <param name="observerPosition"></param>
        private void UpdateVisibleChunks(Vector2 observerPosition)
        {
            // проверяем, есть ли на координатах наблюдателя регион
            // конвертируем координаты наблюдателя в координаты региона
            Vector2 regionLocation = new Vector2(
                (float)Math.Floor(observerPosition.X / (WorldConstants.REGION_SIZE * WorldConstants.CHUNK_SIZE_IN_PIXELS)),
                (float)Math.Floor(observerPosition.Y / (WorldConstants.REGION_SIZE * WorldConstants.CHUNK_SIZE_IN_PIXELS))
            );

            for (int i = (int)regionLocation.X - 1; i < regionLocation.X + 1; i++)
            {
                for (int j = (int)regionLocation.Y - 1; j < regionLocation.Y + 1; j++)
                {
                    Vector2 nearPositions = new Vector2(i, j);

                    Region region = IsRegionExists(nearPositions);
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
                        AddRegion(new Region(nearPositions));
                    }
                }
            }

            // обновляем видимые чанки
            // конвертируем координаты наблюдателя в координаты чкнка
            var chunkLocation = new Vector2(
                (float)Math.Floor(observerPosition.X / WorldConstants.CHUNK_SIZE_IN_PIXELS),
                (float)Math.Floor(observerPosition.Y / WorldConstants.CHUNK_SIZE_IN_PIXELS)
            );
            _visibleChunks = FilterVisibleChunks(chunkLocation);
        }

        /// <summary> Выделенная логика отгрузки лишних регионов из памяти. </summary>
        private void UnloadRegions()
        {
            for (int i = 0; i < _loadedRegions.Count; i++)
            {
                if (!_loadedRegions[i].IsVisible())
                {
                    // если в регионе нет видимых чанков, то регион сохраняется на диск и выгружается из памяти.
                    _loadedRegions[i].SaveRegion(_selfDirectory);
                    _loadedRegions.RemoveAt(i);
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
            UpdateVisibleChunks(observerPosition);

            if ((int)(gameTime.TotalGameTime.TotalSeconds) % WorldConstants.REGION_UNLOAD_FREQUENCY == 0)
            {
                UnloadRegions();
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
                debugInfo += $"\tLoaded regions:{_loadedRegions.Count} (A region contains {Math.Pow(WorldConstants.REGION_SIZE, 2)} chunks)\n";
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
