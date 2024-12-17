using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using LitD.System.SerializableTypes;

namespace LitD.System
{
    internal class Camera
    {
        public Matrix Transform { get; private set; }
        public Vector2 Position { get; private set; }
        public float Zoom { get; set; } = 0.5f;

        public Camera()
        {
            Position = Vector2.Zero;
        }

        public void Update(Vector2 playerPosition, int screenWidth, int screenHeight)
        {
            Vector2 screenCenter = new Vector2(screenWidth / 2, screenHeight / 2) / Zoom;

            Position = playerPosition - screenCenter;

            Transform = Matrix.CreateTranslation(new Vector3(-Position, 0)) *
                         Matrix.CreateScale(Zoom);
        }
    }
}
