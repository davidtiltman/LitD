using LitD.Core.Textures;
using LitD.System;
using LitD.System.SerializableTypes;
using LitD.WorldModule;
using LitD.WorldModule.Entities.Alive.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.IO;

namespace LitD
{
    public class LitDGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private string _worldDirectory;
        private World _world;
        private PlayerEntity _player;
        private Camera _camera;

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
            _worldDirectory = WorldLoader.CreateNew();
            _player = new PlayerEntity("Player", new SerializableVector2(0, 0));
            _camera = new Camera();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureManager.LoadTextures();

            // загрузка созданного в Initialize мира
            //WorldLoader.LoadWorld(_worldDirectory, out _world);
            WorldLoader.LoadWorld("Saves\\12_17_2024 11_10_17 PM", out _world);
            _player.InitializeSprite();
            // =====================================
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            _player.Update(gameTime);
            _camera.Update(_player.EntityPosition, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);

            if(!_world.IsChunkExists(_player.GetChunkPosition()))
            {
                _world.AddChunk(
                    ChunkGenerator.GenerateChunk(_player.GetChunkPosition()),
                    true
                );
            }
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(transformMatrix: _camera.Transform);
            _world.Draw(_spriteBatch, gameTime, _player.GetChunkPosition());
            _player.Draw(_spriteBatch, gameTime);
            _spriteBatch.End();

            // TODO: Add your drawing code here
            base.Draw(gameTime);
        }

        #endregion

        #region ивенты

        private void OnResize(object sender, EventArgs e)
        {
            _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
        }

        #endregion
    }
}
