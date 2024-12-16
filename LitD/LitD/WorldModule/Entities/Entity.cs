using LitD.Core.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using ProtoBuf;
using LitD.System.SerializableTypes;
using LitD.WorldModule.Entities.Placeable;

namespace LitD.WorldModule.Entities
{
    /// <summary> Базовый класс сущности. </summary>
    [ProtoContract]
    [ProtoInclude(101, typeof(TileEntity))]
    internal class Entity
    {
        #region свойства сущности
        [ProtoMember(1)]
        public SerializableVector2 EntityPosition { get; private set; }

        [ProtoMember(2)]
        public bool IsCollidable { get; private set; }

        #region текстура сущности
        /*
         * в сейв мира текстуру особо не запишешь, поэтому записывать будем её название.
         * затем при загрузке чанка для каждой сущности будет инициализироваться текстура по названию.
         */
        [ProtoMember(3)]
        public string EntitySpriteName { get; private set; }

        private Texture2D _entitySprite = null;
        public Texture2D EntitySprite
        { 
            get
            {
                return _entitySprite;
            }
            private set
            {
                _entitySprite = value;
            }
        }
        #endregion
        #endregion

        public Entity(string textureName, Vector2 spawnPosition, bool isCollidable = false)
        {
            EntitySpriteName = textureName;
            EntityPosition = spawnPosition;
            IsCollidable = isCollidable;
        }

        /// <summary> Пустой конструктор нужен для десериализации. </summary>
        protected Entity()
        { }


        #region обновление и отрисовка
        /// <summary> Инициализирует спрайт сущности. </summary>
        /// <exception cref="InvalidDataException"> Бросается, если имя спрайта не указано. </exception>
        public virtual void InitializeSprite()
        {
            if (string.IsNullOrEmpty(EntitySpriteName))
            {
                throw new InvalidDataException("No entity sprite name provided!");
            }
            EntitySprite = TextureManager.GetTexture(EntitySpriteName);
        }

        public virtual void Update(GameTime gameTime)
        {
 
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(EntitySprite, EntityPosition, Color.White);
        }
        #endregion
    }
}
