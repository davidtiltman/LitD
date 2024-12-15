using LitD.Core.Textures;
using LitD.WorldModule;
using LitD.WorldModule.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Runtime.Serialization;
using ProtoBuf;
using System.Diagnostics;
using System.Linq;

namespace LitD
{
    public class LitDGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private string _worldFile;
        private World _world;

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
            Directory.CreateDirectory("Saves");
            #endregion


            TextureManager.Init(Content, GraphicsDevice);

            // создание мира пока происходит здесь. Но должно будет по нажатии соответствующей кнопки, когда она появится, ахах
            _worldFile = WorldLoader.CreateNew();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureManager.LoadTextures();

            // загрузка созданного в Initialize мира
            WorldLoader.LoadWorld(_worldFile, out _world);
            // =====================================
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
            _world.Draw(_spriteBatch, gameTime);
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
