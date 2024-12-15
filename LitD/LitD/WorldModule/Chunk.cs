using LitD.WorldModule.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.Serialization;
using ProtoBuf;
using LitD.System.SerializableTypes;

namespace LitD.WorldModule
{
    /// <summary> Сектор в мире, содержащий фиксированное количество тайлов. </summary>
    [ProtoContract]
    internal class Chunk
    {
        [ProtoMember(1)]
        public SerializableVector2 Position { get; private set; }

        [ProtoMember(2)]
        private Entity[] _contentTiles;

        public Chunk(Vector2 chunkPosition)
        {
            Position = chunkPosition;
            _contentTiles = new Entity[WorldConstants.CHUNK_SIZE * WorldConstants.CHUNK_SIZE];
        }

        private Chunk()
        {}

        /// <summary>
        /// Переписывает тайл чанка.
        /// </summary>
        /// <param name="entity"> Тайл. </param>
        /// <param name="entityPosition"> Координаты тайла относительно начала чанка. </param>
        /// <exception cref="IndexOutOfRangeException"> Координаты тайла находятся за пределами чанка. </exception>
        public void SetTile(Entity entity, Vector2 entityPosition)
        {
            try
            {
                _contentTiles[(int)entityPosition.Y * WorldConstants.CHUNK_SIZE + (int)entityPosition.X] = entity;
            }
            catch (Exception e)
            {
                throw new IndexOutOfRangeException("Tile position is out of chunk bounds");
            }
        }

        /// <summary> Возвращает все тайлы чанка. </summary>
        /// <returns> Массив всех тайлов. </returns>
        public Entity[] GetTiles()
        {
            return _contentTiles;
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (Entity tile in GetTiles())
            {
                tile.Draw(spriteBatch, gameTime);
            }
        }
    }
}
