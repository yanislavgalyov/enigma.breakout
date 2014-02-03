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
using EnigmaBreaker.Screens.BaseScreens;
using System.Linq;
#endregion

namespace EnigmaBreaker.Screens
{
    /// <summary>
    /// Screen showing Credits
    /// </summary>
    class CreditsScreen : MenuScreen
    {
        #region Initialization

        public CreditsScreen()
            : base("Credits")
        {
            MenuEntry backMenuEntry = new MenuEntry("Back");
            backMenuEntry.Selected += OnCancel;
            MenuEntries.Add(backMenuEntry);

            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);

            CreditsHelper.Initialize();

            var credits = CreditsHelper.LoadCredits(CreditsHelper.CreditsFilename);

            for (int i = 0; i < credits.Count; i++)
            {
                var text = string.Format("{0}{1}", credits.Role[i], credits.Contributer[i]);
                TextEntry entry = new TextEntry(text);

                TextEntries.Add(entry);
            }
        }

        #endregion
    }
}
