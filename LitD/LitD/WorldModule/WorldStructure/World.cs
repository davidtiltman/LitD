using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;
using LitD.System.Interfaces;
using System;

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
                    region = Serializer.Deserialize(file, region);
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
            List<Chunk> allChunks = new List<Chunk>();
            foreach (Region region in _loadedRegions)
            {
                foreach (Chunk chunk in region.GetChunks())
                {
                    allChunks.Add(chunk);
                }
            }

            _visibleChunks.Clear();
            List<Chunk> visibleChunks = new List<Chunk>();
            foreach(Chunk chunk in allChunks)
            {
                if (Vector2.Distance(chunk.Position, observerPosition) <= WorldConstants.CHUNK_DRAW_DISTANCE)
                {
                    visibleChunks.Add(chunk);
                }
            }

            return visibleChunks;
        }

        public List<Chunk> GetVisibleChunks()
        {
            return _visibleChunks;
        }

        #region Update/Draw

        /// <summary>
        /// Обновляет состояние мира.
        /// </summary>
        /// <param name="gameTime"> Игровое время. </param>
        /// <param name="observerRegionPosition"> Координаты наблюдателя. </param>
        public void Update(GameTime gameTime, Vector2 observerPosition)
        {
            // проверяем, есть ли на координатах наблюдателя регион
            // конвертируем координаты наблюдателя в координаты региона
            Vector2 regionLocation = new Vector2(
                (float)Math.Floor(observerPosition.X / (WorldConstants.REGION_SIZE * WorldConstants.CHUNK_SIZE_IN_PIXELS)),
                (float)Math.Floor(observerPosition.Y / (WorldConstants.REGION_SIZE * WorldConstants.CHUNK_SIZE_IN_PIXELS))
            );

            // проверяем квадрат 3x3 вокруг наблюдателя, чтобы подргузить соседние регионы
            Vector2[] nearRegions = new Vector2[]
            {
                new Vector2(regionLocation.X - regionLocation.Y - 1),
                new Vector2(regionLocation.X, regionLocation.Y - 1),
                new Vector2(regionLocation.X + 1, regionLocation.Y - 1),

                new Vector2(regionLocation.X - 1, regionLocation.Y),
                new Vector2(regionLocation.X, regionLocation.Y),
                new Vector2(regionLocation.X + 1, regionLocation.Y),

                new Vector2(regionLocation.X - 1, regionLocation.Y + 1),
                new Vector2(regionLocation.X, regionLocation.Y + 1),
                new Vector2(regionLocation.X + 1, regionLocation.Y + 1)
            };

            foreach (Vector2 nearRegion in nearRegions)
            {
                Region region = IsRegionExists(nearRegion);
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
                    AddRegion(new Region(nearRegion));
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
