using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ProtoBuf;
using LitD.System.Interfaces;
using System;
using LitD.WorldModule.WorldStructure.WorldServices;
using System.Threading;
using System.Diagnostics;

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

        #region Update/Draw

        /// <summary>
        /// Обновляет состояние мира.
        /// </summary>
        /// <param name="gameTime"> Игровое время. </param>
        /// <param name="observerRegionPosition"> Координаты наблюдателя. </param>
        public void Update(GameTime gameTime, Vector2 observerPosition)
        {
            LoadHandler.LoadRegions(observerPosition, ref _loadedRegions, _selfDirectory);
            VisibilityHandler.UpdateVisibleChunks(observerPosition, ref _loadedRegions, ref _visibleChunks);

            LoadHandler.UnloadRegions(gameTime, observerPosition, ref _loadedRegions, _selfDirectory);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 observerPosition)
        {
            foreach (var chunk in _visibleChunks)
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
                debugInfo += "\tNo regions loaded\n";
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
