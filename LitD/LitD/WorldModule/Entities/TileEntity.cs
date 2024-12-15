using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.Serialization;

namespace LitD.WorldModule.Entities
{
    /// <summary> Класс для тайлов, из которых строится мир. </summary>
    [DataContract]
    internal class TileEntity : Entity
    {
        #region свойства сущности
        [DataMember]
        public bool IsCollidable { get; private set; }
        #endregion

        public TileEntity(string textureName, Vector2 spawnPosition, bool isCollidabble = false) : base(textureName, spawnPosition)
        {
            IsCollidable = isCollidabble;
        }

        public TileEntity() : base()
        {
            IsCollidable = false;
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
