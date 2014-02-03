using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnigmaBreaker.Manager;
using Microsoft.Xna.Framework;
using Kraken.Xna.Input;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using EnigmaBreaker.Screens.BaseScreens;
using EnigmaBreaker.Model;
using EnigmaBreaker.Screens.Helpers;

namespace EnigmaBreaker.Screens
{
    /// <summary>
    /// Screen for entering players name after game over and writing his high scores
    /// </summary>
    public class InputNameScreen : GameScreen
    {
        #region Properties 

        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Texture2D Texture { get; set; }

        public int Scores { get; set; }
        public int Level { get; set; }

        KInputService KInput;
        Type KInputType { get; set; }
        String buffer = "";
        SpriteFont font;

        #endregion

        void input_KeyDown(object sender, EventArgs e)
        {
            KKeyboardEventArgs args = (KKeyboardEventArgs)e;
            if (args.Key == Keys.Back && buffer != "")
            {
                buffer = buffer.Substring(0, buffer.Length - 1);
            }
            if (args.Key == Keys.Enter && buffer.Length > 0)
            {
                // check if file exists and it is not empty
                HighScoresHelper.Initialize();
               
                HighScoresHelper.SaveHighScore(this.Scores, buffer, this.Level);

                // remove KInput service from Game.Services
                if (KInputType != null)
                    ScreenManager.GetGame.Services.RemoveService(KInputType);

                LoadingScreen.Load(ScreenManager, true, null,
                               new BackgroundScreen(), new MainMenuScreen(), new HighScoresScreen());

                this.ExitScreen();       
            }
            if (args.Key == Keys.Escape)
            {
                if (KInputType != null)
                    ScreenManager.GetGame.Services.RemoveService(KInputType);

                this.ExitScreen();
            }
            else if (KInputService.IsAlpha(args.Key))
            {
                if (buffer.Length < 10)
                {
                    if (args.ShiftState == KShiftState.Shift)
                        buffer += args.Key.ToString().ToUpper();
                    else
                        buffer += args.Key.ToString().ToLower();
                }
            }
        }

        public override void LoadContent()
        {
            this.IsPopup = true;

            KInput = new KInputService(ScreenManager.GetGame);
            KInputType = KInput.GetType();

            font = ScreenManager.GetGame.Content.Load<SpriteFont>("gameoverfont");

            KInput.KeyDown += new EventHandler(input_KeyDown);

            Texture = ScreenManager.GetGame.Content.Load<Texture2D>("gameover");
            Position = Vector2.Zero;
            Size = new Vector2(Texture.Width, Texture.Height);
            
            base.LoadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {            
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // KInput should be updated if it is NOT added to Game Components
            KInput.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Center the screen content in the viewport.
            Vector2 viewportSize = Resolution.GetVirtualViewportSize();
            Position = (viewportSize - Size) / 2;

            //todo2
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, null, Resolution.getTransformationMatrix());
            //spriteBatch.Begin(SpriteBlendMode.AlphaBlend,
            //                       SpriteSortMode.Texture,
            //                       SaveStateMode.None,
            //                       Resolution.getTransformationMatrix());

            // TODO : design
            spriteBatch.Draw(Texture, Position, Color.White);

            // Draw the points
            spriteBatch.DrawString(font, string.Format("{0, -10} points!", this.Scores), Position + new Vector2(120, 25), Color.Red);
            // Draw the name
            spriteBatch.DrawString(font, string.Format("Enter your name:"), Position + new Vector2(120, 43), Color.Black);
            spriteBatch.DrawString(font, string.Format("{0, -10}", buffer), Position + new Vector2(140, 67), Color.Black);


            spriteBatch.End();
        }
    }
}
