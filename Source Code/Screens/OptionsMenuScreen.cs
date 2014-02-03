#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using EnigmaBreaker.Screens;
using EnigmaBreaker.Screens.Helpers;
using EnigmaBreaker.Screens.BaseScreens;
using System.ComponentModel;
using EnigmaBreaker.Model;
using System.Linq;
using System;
using Microsoft.Xna.Framework.Input;
using EnigmaBreaker.Manager;
#endregion

namespace EnigmaBreaker.Screens
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry ScreenResolution;
        MenuEntry SoundVolume;

        static GameResolution currentResolution;
        static int currentSoundVolume;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            OptionsHelper.Initialize();

            int width = Resolution.GetWidth();

            switch (width)
            {
                case 800:
                    currentResolution = GameResolution.Small;
                    break;
                case 1024:
                    if (Resolution.GetFullScreen())
                    {
                        currentResolution = GameResolution.Large;
                    }
                    else
                    {
                        currentResolution = GameResolution.Medium;
                    }
                    break;
                default:
                    currentResolution = GameResolution.Medium;
                    break;
            }

            // Create our menu entries.
            SoundVolume = new MenuEntry(string.Empty);
            SoundVolume.Slider = OptionSlider.VolumeSlider;

            ScreenResolution = new MenuEntry(string.Empty);

            SetMenuEntryText();

            MenuEntry backMenuEntry = new MenuEntry("Back");

            // Hook up menu event handlers.
            ScreenResolution.Selected += new System.EventHandler<PlayerIndexEventArgs>(ScreenResolution_Selected);
            SoundVolume.Selected += new System.EventHandler<PlayerIndexEventArgs>(SoundVolume_Selected);
            SoundVolume.Configure += new EventHandler<PlayerIndexEventArgs>(SoundVolume_Configure);

            backMenuEntry.Selected += OnCancel;
            
            // Add entries to the menu.
            MenuEntries.Add(ScreenResolution);
            MenuEntries.Add(SoundVolume);

            MenuEntries.Add(backMenuEntry);
        }

        

        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            ScreenResolution.Text = "Screen Resolution: " + currentResolution.GetDescription();
            SoundVolume.Text = "Volume: " + currentSoundVolume.ToString();
        }


        #endregion

        #region Handle Input

        void ScreenResolution_Selected(object sender, PlayerIndexEventArgs e)
        {
            currentResolution++;

            if (currentResolution > GameResolution.Large)
                currentResolution = 0;

            OptionsHelper.SaveOptions(currentResolution.ToString());

            // set resolution
            switch (currentResolution)
            {
                case GameResolution.Large:
                    Resolution.SetResolution(1024, 768, true);
                    break;
                case GameResolution.Medium:
                    Resolution.SetResolution(1024, 768, false);
                    break;
                case GameResolution.Small:
                    Resolution.SetResolution(800, 600, false);
                    break;
            }

            SetMenuEntryText();
        }

        void SoundVolume_Selected(object sender, PlayerIndexEventArgs e)
        {
            OptionsHelper.SaveOptions(currentSoundVolume);
        }

        void SoundVolume_Configure(object sender, PlayerIndexEventArgs e)
        {
            var menuEntry = (MenuEntry)sender;

            if (menuEntry.SliderUp)
            {
                SoundManager.IncreaseVolume();
            }
            else
            {
                SoundManager.DecreaseVolume();
            }

            currentSoundVolume = (int)(SoundManager.Volume() * 100);

            SetMenuEntryText();
        }

        #endregion

        #region Update

        public override void LoadContent()
        {
            currentSoundVolume = (int)(SoundManager.Volume() * 100);
            SetMenuEntryText();
        }

        #endregion

    }
}
