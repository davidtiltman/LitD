using LitD.Core.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace LitD.WorldModule.Entities
{
    /// <summary> Базовый класс сущности. </summary>
    [DataContract]
    [KnownType(typeof(TileEntity))]
    internal class Entity
    {
        #region свойства сущности
        [DataMember]
        public Vector2 EntityPosition { get; private set; }

        #region текстура сущности
        /*
         * в сейв мира текстуру особо не запишешь, поэтому записывать будем её название.
         * затем при загрузке чанка для каждой сущности будет инициализироваться текстура по названию.
         */
        [DataMember]
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

        public Entity(string textureName, Vector2 spawnPosition)
        {
            EntitySpriteName = textureName;
            EntityPosition = spawnPosition;
        }

        public Entity()
        {
            EntitySpriteName = "Empty";
            EntityPosition = Vector2.Zero;
        }


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
