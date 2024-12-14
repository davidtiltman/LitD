using LitD.Core.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace LitD.World.Entities
{
    /// <summary> Базовый класс сущности. </summary>
    internal class Entity
    {
        #region свойства сущности
        public Vector2 EntityPosition { get; private set; }

        #region текстура сущности
        /*
         * в сейв мира текстуру особо не запишешь, поэтому записывать будем её название.
         * затем при загрузке чанка для каждой сущности будет инициализироваться текстура по названию.
         */
        public string EntitySpriteName { get; private set; }
        private Texture2D _entitySprite = null;
        public Texture2D EntitySprite
        { 
            get
            {
                return _entitySprite;
            }
            set
            {
                throw new NotImplementedException("Hot-swapping sprites for entity is not implemented!");
            }
        }
        #endregion
        #endregion

        public Entity(string textureName, Vector2 spawnPosition)
        {
            EntitySpriteName = textureName;
            EntityPosition = spawnPosition;
        }

        #region обновление и отрисовка
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
