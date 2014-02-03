using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnigmaBreaker.Screens.BaseScreens;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace EnigmaBreaker.Screens
{
    class LogoScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        Texture2D LogoBackground;

        #endregion

        #region Initialization

        public LogoScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            LogoBackground = content.Load<Texture2D>("logo_background");
        }

        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update and Draw

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            double elapsedTIme = gameTime.TotalGameTime.Seconds;

            if (elapsedTIme > 2)
            {
                ScreenManager.AddScreen(new BackgroundScreen(), null);
                ScreenManager.AddScreen(new MainMenuScreen(), null);
                ScreenManager.RemoveScreen(this);
            }

            base.Update(gameTime, otherScreenHasFocus, false);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Center the text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle vewportRect = new Rectangle(0, 0, viewport.Width, viewport.Height);

            Color color = new Color(255, 255, 255, TransitionAlpha);

            spriteBatch.Begin();

            spriteBatch.Draw(LogoBackground, vewportRect, color); 

            spriteBatch.End();
        }

        #endregion
    }
}
