﻿using Arch.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using RogueliteSurvivor.Containers;
using RogueliteSurvivor.Physics;
using RogueliteSurvivor.Scenes;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RogueliteSurvivor
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        const int scaleFactor = 3;
        private Matrix transformMatrix;

        private World world = null;
        Box2D.NetStandard.Dynamics.World.World physicsWorld = null;
        System.Numerics.Vector2 gravity = System.Numerics.Vector2.Zero;

        Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();
        Dictionary<string, PlayerContainer> playerCharacters = new Dictionary<string, PlayerContainer>();
        Dictionary<string, MapContainer> mapContainers = new Dictionary<string, MapContainer>();
        string currentScene = "main-menu";
        string nextScene = string.Empty;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.SynchronizeWithVerticalRetrace = true;
        }

        protected override void Initialize()
        {
            transformMatrix = Matrix.CreateScale(scaleFactor, scaleFactor, 1f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            world = World.Create();
            physicsWorld = new Box2D.NetStandard.Dynamics.World.World(gravity);
            physicsWorld.SetContactListener(new GameContactListener());
            physicsWorld.SetContactFilter(new GameContactFilter());

            loadPlayerCharacters();
            loadPlayableMaps();

            GameScene gameScene = new GameScene(_spriteBatch, Content, _graphics, world, physicsWorld, playerCharacters, mapContainers);

            MainMenuScene mainMenu = new MainMenuScene(_spriteBatch, Content, _graphics, world, physicsWorld, playerCharacters, mapContainers);
            mainMenu.LoadContent();

            LoadingScene loadingScene = new LoadingScene(_spriteBatch, Content, _graphics, world, physicsWorld);
            loadingScene.LoadContent();

            GameOverScene gameOverScene = new GameOverScene(_spriteBatch, Content, _graphics, world, physicsWorld);
            gameOverScene.LoadContent();

            scenes.Add("game", gameScene);
            scenes.Add("main-menu", mainMenu);
            scenes.Add("loading", loadingScene);
            scenes.Add("game-over", gameOverScene);
        }

        private void loadPlayerCharacters()
        {
            JObject players = JObject.Parse(File.ReadAllText(Path.Combine(Content.RootDirectory, "Datasets", "player-characters.json")));
            playerCharacters = new Dictionary<string, PlayerContainer>();

            foreach (var player in players["data"])
            {
                playerCharacters.Add(
                    PlayerContainer.GetPlayerContainerName(player),
                    PlayerContainer.ToPlayerContainer(player)
                );
            }
        }

        private void loadPlayableMaps()
        {
            JObject maps = JObject.Parse(File.ReadAllText(Path.Combine(Content.RootDirectory, "Datasets", "maps.json")));
            mapContainers = new Dictionary<string, MapContainer>();

            foreach (var map in maps["data"])
            {
                mapContainers.Add(
                    MapContainer.MapContainerName(map),
                    MapContainer.ToMapContainer(map)
                );
            }
        }

        protected override void Update(GameTime gameTime)
        {
            switch (currentScene)
            {
                case "loading":
                    nextScene = scenes[currentScene].Update(gameTime, scenes["game"].Loaded);
                    break;
                default:
                    nextScene = scenes[currentScene].Update(gameTime);
                    break;
            }

            if (!string.IsNullOrEmpty(nextScene))
            {
                switch (nextScene)
                {
                    case "game":
                        break;
                    case "main-menu":
                        break;
                    case "game-over":
                        break;
                    case "loading":
                        Task.Run(() =>
                            {
                                ((GameScene)scenes["game"]).SetGameSettings(((MainMenuScene)scenes["main-menu"]).GetGameSettings());
                                scenes["game"].LoadContent();
                            });
                        break;
                    case "exit":
                        Exit();
                        break;
                }

                currentScene = nextScene;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (currentScene)
            {
                case "loading":
                    scenes[currentScene].Draw(gameTime, transformMatrix, scenes["game"].Loaded);
                    break;
                default:
                    scenes[currentScene].Draw(gameTime, transformMatrix);
                    break;
            }

            base.Draw(gameTime);
        }
    }
}
