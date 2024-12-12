using LitD.Core.Textures;
using LitD.Entities;
using LitD.World;
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

        private List<Entity> _entities = new List<Entity>();

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureManager.LoadTextures();

            _entities.Add(new TileEntity("Dirt", new Vector2(100, 127)));
            _entities.Add(new Entity("Grass", new Vector2(514, 320)));
            _entities.Add(new TileEntity("fjduwhriub23", new Vector2(400, 400)));
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

            foreach (Entity entity in _entities)
                entity.Draw(_spriteBatch, gameTime);

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
