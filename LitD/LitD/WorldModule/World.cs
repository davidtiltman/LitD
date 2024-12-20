using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;
using LitD.System.Interfaces;

namespace LitD.WorldModule
{
    /// <summary> Игровой мир, состоящий из чанков. </summary>
    [ProtoContract]
    internal class World : IDebugInfo
    {
        /// <summary> Название мира. </summary>
        [ProtoMember(1)]
        public string Name { get; private set; }

        /// <summary> Все загруженные чанки. </summary>
        private List<Chunk> _loadedChunks = new List<Chunk>();

        /// <summary> Все видимые чанки вокруг игрока. </summary>
        private List<Chunk> _visibleChunks = new List<Chunk>();

        [ProtoMember(2)]
        private string _selfDirectory;

        public World(string name, string selfDirectory)
        {
            Name = name;
            _loadedChunks = new List<Chunk>();
            _selfDirectory = selfDirectory;
        }

        /// <summary> Пустой конструктор нужен для десериализации. </summary>
        private World()
        {}

        /// <summary> Добавляет в мир новый чанк. </summary>
        /// <param name="chunk"> Новый чанк. </param>
        public void AddChunk(Chunk chunk, bool isNew = false)
        {
            _loadedChunks.Add(chunk);
            chunk.InitializeEntitySprites();

            if (isNew)
            {
                List<Chunk> appendList = new List<Chunk>() { chunk };
                using (FileStream chunkFile = new FileStream(Path.Combine(_selfDirectory, WorldConstants.WORLD_CHUNK_FILE_NAME), FileMode.Append))
                {
                    Serializer.Serialize<List<Chunk>>(chunkFile, appendList);
                }
            }
        }

        /// <summary> Возвращает список всех чанков мира. </summary>
        /// <returns> Список всех чанков мира. </returns>
        public List<Chunk> GetLoadedChunks()
        {
            return _loadedChunks;
        }

        private Chunk IsChunkExists(Vector2 chunkPosition)
        {
            foreach (Chunk chunk in _loadedChunks)
            {
                if (chunk.Position == chunkPosition)
                {
                    return chunk;
                }
            }
            return null;
        }

        /// <summary> Возвращает список видимых наблюдателем чанков. </summary>
        /// <param name="observerPosition"> Координаты наблюдателя. </param>
        private List<Chunk> GetVisibleChunks(Vector2 observerPosition)
        {
            List<Chunk> visible = new List<Chunk>();

            foreach(var chunk in GetLoadedChunks())
            {
                if (Vector2.Distance(chunk.Position, observerPosition) <= WorldConstants.CHUNK_LOAD_DISTANCE)
                {
                    visible.Add(chunk);
                }
            }

            return visible;
        }

        private List<Vector2> GetVisiblePositions(Vector2 observer)
        {
            List<Vector2> unfiltered = new List<Vector2>();

            for (int i = (int)observer.X - WorldConstants.CHUNK_LOAD_DISTANCE; i <= observer.X + WorldConstants.CHUNK_LOAD_DISTANCE; i++)
            {
                for (int j = (int)observer.Y - WorldConstants.CHUNK_LOAD_DISTANCE; j <= observer.Y + WorldConstants.CHUNK_LOAD_DISTANCE; j++)
                {
                    unfiltered.Add(new Vector2(i, j));
                }
            }

            List<Vector2> filtered = new List<Vector2>();
            foreach (var positions in unfiltered)
            {
                if (Vector2.Distance(observer, positions) <= WorldConstants.CHUNK_LOAD_DISTANCE)
                {
                    filtered.Add(positions);
                }
            }

            return filtered;
        }

        public void Update(GameTime gameTime, Vector2 observerPosition)
        {
            List<Vector2> visiblePositions = GetVisiblePositions(observerPosition);

            foreach (var position in visiblePositions)
            {
                if (IsChunkExists(position) == null)
                {
                    AddChunk(ChunkGenerator.GenerateChunk(position), true);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, Vector2 observerPosition)
        {
            foreach (var chunk in GetVisibleChunks(observerPosition))
            {
                chunk.Draw(spriteBatch, gameTime);
            }
        }

        #region IDebugInfo

        public void GetDebugInfo(ref string debugInfo)
        {
            if (!string.IsNullOrEmpty(debugInfo))
            {
                debugInfo += "\n";
            }

            debugInfo += "World:\n";
            if (_loadedChunks.Count > 0)
            {
                debugInfo += $"\tLoaded chunks:{GetLoadedChunks().Count}";
            }
            else
            {
                debugInfo += "No chunks loaded";
            }
        }

        #endregion
    }
}
