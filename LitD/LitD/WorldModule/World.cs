using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using ProtoBuf;
using System.IO;
using System.Diagnostics;

namespace LitD.WorldModule
{
    /// <summary> Игровой мир, состоящий из чанков. </summary>
    [ProtoContract]
    internal class World
    {
        [ProtoMember(1)]
        public string Name { get; private set; }
        private List<Chunk> _loadedChunks = new List<Chunk>();

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

        public bool IsChunkExists(Vector2 chunkPosition)
        {
            foreach (Chunk chunk in _loadedChunks)
            {
                if (chunk.Position == chunkPosition)
                {
                    return true;
                }
            }
            return false;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var chunk in GetLoadedChunks())
            {
                chunk.Draw(spriteBatch, gameTime);
            }
        }

        /// <summary> Обновляет массив загруженных чанков. </summary>
        public void UpdateLoadedChunks()
        {
            
        }
    }
}
