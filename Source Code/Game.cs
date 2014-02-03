using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using EnigmaBreaker.Manager;
using EnigmaBreaker.Screens;
using System.Configuration;
using EnigmaBreaker.Model;
using System.Threading;
using EnigmaBreaker.Screens.Helpers;
using System.Diagnostics;

namespace EnigmaBreaker
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        ScreenManager screenManager;

        public static Rectangle gamePlayScreen = new Rectangle(
            int.Parse(ConfigurationSettings.AppSettings["GamePlayScreenX"]),
            int.Parse(ConfigurationSettings.AppSettings["GamePlayScreenY"]),
            int.Parse(ConfigurationSettings.AppSettings["GamePlayScreenWidth"]),
            int.Parse(ConfigurationSettings.AppSettings["GamePlayScreenHeight"])
            );

        public static int gamePlayScreenBorder = int.Parse(ConfigurationSettings.AppSettings["GamePlayScreenBorder"]);

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Resolution.Init(ref graphics);

            
            screenManager = new ScreenManager(this);
            SoundManager.Initialize(this, screenManager);
            Resolution.SetVirtualResolution(1024, 768);

            if (screenManager != null)
            {
                SetGameResolution();
            }
            else
            {
                Resolution.SetResolution(800, 600, false);
            }

            Components.Add(screenManager);

            screenManager.AddScreen(new LogoScreen(), null);
        }

        private void SetGameResolution()
        {
            try
            {
                OptionData data = OptionsHelper.LoadOptions(OptionsHelper.OptionsFilename);
                string resolution = data.GameResolution;

                if (resolution == GameResolution.Large.ToString())
                {
                    Resolution.SetResolution(1024, 768, true);
                }
                else if (resolution == GameResolution.Medium.ToString())
                {
                    Resolution.SetResolution(1024, 768, false);
                }
                else if (resolution == GameResolution.Small.ToString())
                {
                    Resolution.SetResolution(800, 600, false);
                }
                else
                {
                    Resolution.SetResolution(800, 600, false);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException);
                Resolution.SetResolution(800, 600, false);

            }
        }

        protected override void Initialize()
        {
            Window.Title = "Enigma Breaker";
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            base.Draw(gameTime);
        }
    }
}
