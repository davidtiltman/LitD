using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using LitD.System.SerializableTypes;

namespace LitD.WorldModule.Entities.Alive.Player
{
    internal class Camera
    {
        public Matrix Transform { get; private set; }
        public Vector2 Position { get; private set; }
        public float Zoom { get; set; } = 1f;

        public Camera()
        {
            Position = Vector2.Zero;
        }

        public void Update(Vector2 playerPosition, int screenWidth, int screenHeight)
        {
            Position = playerPosition - new Vector2(screenWidth / 2, screenHeight / 2);

            Transform = Matrix.CreateTranslation(new Vector3(-Position, 0)) *
                         Matrix.CreateScale(Zoom);
        }
    }
}
