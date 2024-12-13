using LitD.Core.Textures;
using LitD.World;
using LitD.World.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace LitD
{
    public class LitDGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public LitDGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
        }

        #region логика игры

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            #region проверка/создание директорий
            System.IO.Directory.CreateDirectory("Saves");
            #endregion


            TextureManager.Init(Content, GraphicsDevice);
            /*
             * LoadContent вызывается во время выполнения
             */
            string world = WorldHandler.CreateNew();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureManager.LoadTextures();

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            _spriteBatch.End();

            // TODO: Add your drawing code here
            base.Draw(gameTime);
        }

        #endregion

        #region ивенты

        private void OnResize(object sender, EventArgs e)
        {
            // todo
        }

        #endregion
    }
}
