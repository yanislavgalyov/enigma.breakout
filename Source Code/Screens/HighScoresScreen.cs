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
using System.Linq;
using EnigmaBreaker.Screens.Helpers;
using EnigmaBreaker.Screens.BaseScreens;
#endregion

namespace EnigmaBreaker.Screens
{
    /// <summary>
    /// Screen shows High Scores
    /// </summary>
    class HighScoresScreen : MenuScreen
    {
        #region Initialization

        public HighScoresScreen()
            : base("High Scores")
        {
            MenuEntry backMenuEntry = new MenuEntry("Back");
            backMenuEntry.Selected += OnCancel;
            MenuEntries.Add(backMenuEntry);

            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);

            HighScoresHelper.Initialize();

            var hs = HighScoresHelper.LoadHighScores(HighScoresHelper.HighScoresFilename);

            for (int i = 0; i < hs.Count; i++)
            {
                TextEntries.Add(new TextEntry(string.Format("{0}  {2}Level  {1}", hs.PlayerName[i], hs.Score[i], hs.Level[i])));
            }
        }

        #endregion
    }
}
