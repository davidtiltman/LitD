using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LitD.World.Entities
{
    internal class TileEntity : Entity
    {
        #region свойства сущности
        public bool Collidable { get; private set; }
        #endregion

        public TileEntity(string textureName, Vector2 spawnPosition) : base(textureName, spawnPosition)
        {
        }

        #region обновление и отрисовка
        public override void Update(GameTime gameTime)
        { }

        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(EntitySprite, EntityPosition, Color.White);
        }
        #endregion
    }
}
