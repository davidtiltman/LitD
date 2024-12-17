using LitD.System.SerializableTypes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace LitD.WorldModule.Entities.Alive.Player
{
    internal class PlayerEntity : Entity
    {
        public float MoveSpeed { get; set; } = 250f;

        public PlayerEntity(string textureName, SerializableVector2 spawnPosition, bool isCollidable = false) : base(textureName, spawnPosition, isCollidable)
        {}

        public Vector2 GetChunkPosition()
        {
            return new Vector2(
                (float)Math.Floor(EntityPosition.X / WorldConstants.CHUNK_SIZE_IN_PIXELS),
                (float)Math.Floor(EntityPosition.Y / WorldConstants.CHUNK_SIZE_IN_PIXELS)            
            );
        }

        #region обновление и отрисовка
        public virtual void Update(GameTime gameTime)
        {
            // это временная кринж-реализация движения. 
            var key = Keyboard.GetState();
            float acceleration = 1f;
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (key.IsKeyDown(Keys.LeftShift)) acceleration = 2f;

            if (key.IsKeyDown(Keys.W)) EntityPosition.Y -= MoveSpeed * acceleration * deltaTime;
            if (key.IsKeyDown(Keys.S)) EntityPosition.Y += MoveSpeed * acceleration * deltaTime;
            if (key.IsKeyDown(Keys.A)) EntityPosition.X -= MoveSpeed * acceleration * deltaTime;
            if (key.IsKeyDown(Keys.D)) EntityPosition.X += MoveSpeed * acceleration * deltaTime;
        }

        public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(EntitySprite, EntityPosition, Color.White);
        }
        #endregion
    }
}
