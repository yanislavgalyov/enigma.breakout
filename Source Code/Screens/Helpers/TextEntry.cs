using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EnigmaBreaker.Screens;
using Microsoft.Xna.Framework.Graphics;
using EnigmaBreaker.Manager;
using EnigmaBreaker.Screens.BaseScreens;

namespace EnigmaBreaker.Screens.Helpers
{
    /// <summary>
    /// Helper class represents each enty in Credits or HighScores. Just text!
    /// </summary>
    class TextEntry
    {
        #region Properties

        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
        {
            get; set; 
        }

        #endregion

        #region Initialization

        public TextEntry(string text)
        {
            Text = text;
        }

        #endregion

        #region Update and Draw

        public virtual void Update(MenuScreen screen, GameTime gameTime)
        {  
        }

        public virtual void Draw(MenuScreen screen, Vector2 position, GameTime gameTime)
        {
            Color color = new Color(37,32,85);
                        
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont font;
            string showText;
            if (Text.StartsWith("@"))
            {
                font = screenManager.CreditsBoldFont;
                showText = Text.Substring(1);
            }
            else
            {
                font = screenManager.CreditsFont;
                showText = Text;
            }

            Vector2 origin = Vector2.Zero;

            spriteBatch.DrawString(font, showText, position, color, 0,
                                   origin, 1.0f, SpriteEffects.None, 0);
        }

        public virtual int GetHeight(MenuScreen screen)
        {
            if (Text.StartsWith("@"))
                return screen.ScreenManager.CreditsBoldFont.LineSpacing;
            else
                return screen.ScreenManager.CreditsFont.LineSpacing;
        }

        #endregion
    }
}
