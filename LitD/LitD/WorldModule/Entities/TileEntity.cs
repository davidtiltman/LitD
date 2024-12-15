using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProtoBuf;

namespace LitD.WorldModule.Entities
{
    /// <summary> Класс для тайлов, из которых строится мир. </summary>
    [ProtoContract]
    internal class TileEntity : Entity
    {
        public TileEntity(string textureName, Vector2 spawnPosition, bool isCollidabble = false) : base(textureName, spawnPosition, isCollidabble)
        {
        }

        private TileEntity() : base()
        {}

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
