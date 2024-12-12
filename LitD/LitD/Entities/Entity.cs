using LitD.Core.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LitD.Entities
{
    /// <summary> Базовый класс сущности. </summary>
    internal class Entity
    {
        public Texture2D EntitySprite { get; private set; }
        public Vector2 EntityPosition { get; private set; }

        public Entity(string textureName, Vector2 spawnPosition)
        {
            EntitySprite = TextureManager.GetTexture(textureName);
            EntityPosition = spawnPosition;
        }

        public virtual void Update(GameTime gameTime)
        {

        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime) 
        {
            spriteBatch.Draw(EntitySprite, EntityPosition, Color.White);
        }
    }
}
