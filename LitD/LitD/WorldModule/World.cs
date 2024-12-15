using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ProtoBuf;

namespace LitD.WorldModule
{
    /// <summary> Игровой мир, состоящий из чанков. </summary>
    [ProtoContract]
    internal class World
    {
        [ProtoMember(1)]
        public string Name { get; private set; }

        [ProtoMember(2)]
        private List<Chunk> _chunks;

        public World(string name)
        {
            Name = name;
            _chunks = new List<Chunk>();
        }

        private World()
        { }

        /// <summary> Добавляет в мир новый чанк. </summary>
        /// <param name="chunk"> Новый чанк. </param>
        public void AddChunk(Chunk chunk)
        {
            _chunks.Add(chunk);
        }

        /// <summary> Возвращает список всех чанков мира. </summary>
        /// <returns> Список всех чанков мира. </returns>
        public List<Chunk> GetChunks()
        {
            return _chunks;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (var chunk in GetChunks())
            {
                chunk.Draw(spriteBatch, gameTime);
            }
        }
    }
}
