#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EnigmaBreaker.Manager;
using EnigmaBreaker.Screens;
using EnigmaBreaker.Screens.Helpers;
using EnigmaBreaker.Model;
using System.Linq;
#endregion

namespace EnigmaBreaker.Screens.BaseScreens
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class MenuScreen : GameScreen
    {
        #region Fields

        List<TextEntry> textEntries = new List<TextEntry>();

        protected List<MenuEntry> menuEntries = new List<MenuEntry>();

        protected int selectedEntry = 0;
        string menuTitle;

        #endregion

        #region Properties


        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        protected IList<TextEntry> TextEntries
        {
            get { return textEntries; }
        }

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            // Move to the previous menu entry?
            if (input.IsMenuUp(ControllingPlayer))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;

                SoundManager.PlayCue("BallLost");
            }

            // Move to the next menu entry?
            if (input.IsMenuDown(ControllingPlayer))
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
                SoundManager.PlayCue("BallLost");
            }

            PlayerIndex playerIndex;

            if (input.IsMenuLeft(ControllingPlayer, out playerIndex))
            {
                var selectedMenuEntry = menuEntries[selectedEntry];

                if (selectedMenuEntry != null)
                {
                    if (selectedMenuEntry.Slider == OptionSlider.VolumeSlider)
                    {
                        OnConfigureEntry(selectedEntry, playerIndex, false);
                    }
                }
            }

            if (input.IsMenuRight(ControllingPlayer, out playerIndex))
            {
                var selectedMenuEntry = menuEntries[selectedEntry];

                if (selectedMenuEntry != null)
                {
                    if (selectedMenuEntry.Slider == OptionSlider.VolumeSlider)
                    {
                        OnConfigureEntry(selectedEntry, playerIndex, true);
                    }
                }
            }

            // Accept or cancel the menu? We pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            

            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                OnSelectEntry(selectedEntry, playerIndex);
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);
            }
        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[selectedEntry].OnSelectEntry(playerIndex);
        }

        protected virtual void OnConfigureEntry(int entryIndex, PlayerIndex playerIndex, bool sliderUp)
        {
            menuEntries[selectedEntry].OnConfigureEntry(playerIndex, sliderUp);
        }


        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            Vector2 vViewportSize = Resolution.GetVirtualViewportSize();

            Vector2 position = new Vector2(100, 150);

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            //todo2
            spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, null, Resolution.getTransformationMatrix());
            //spriteBatch.Begin(SpriteBlendMode.AlphaBlend,SpriteSortMode.Texture, SaveStateMode.None, Resolution.getTransformationMatrix());

            // Draw each text entry in turn.
            for (int i = 0; i < textEntries.Count; i++)
            {
                TextEntry tEntry = textEntries[i];

                Vector2 menuEntryTextSize;
                if (tEntry.Text.StartsWith("@"))
                {
                    menuEntryTextSize = ScreenManager.CreditsBoldFont.MeasureString(tEntry.Text.Substring(1));
                }
                else
                {
                    menuEntryTextSize = ScreenManager.CreditsFont.MeasureString(tEntry.Text);
                }

                position.X = (vViewportSize.X - menuEntryTextSize.X) / 2;

                tEntry.Draw(this, position, gameTime);

                position.Y += tEntry.GetHeight(this);
            }


            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                bool isSelected = IsActive && (i == selectedEntry);

                Vector2 menuEntryTextSize = font.MeasureString(menuEntry.Text);
                position.X = (vViewportSize.X - menuEntryTextSize.X) / 2;

                menuEntry.Draw(this, position, isSelected, gameTime);

                position.Y += menuEntry.GetHeight(this);
            }

            // Draw the menu title.
            float titleScale = 1.2f;
            Vector2 titlePosition = new Vector2(0, 40);
            titlePosition.X = (vViewportSize.X - font.MeasureString(menuTitle).X * titleScale) / 2;
            Vector2 titleOrigin = Vector2.Zero;
            Color titleColor = new Color(192, 192, 192, TransitionAlpha);

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();
        }


        #endregion
    }
}
