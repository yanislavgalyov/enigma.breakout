#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using EnigmaBreaker.Manager;
using EnigmaBreaker.Model;
using EnigmaBreaker.Model.Entities;
using EnigmaBreaker.Screens.BaseScreens;
using System.Diagnostics;
using System.Collections.Generic;
#endregion

namespace EnigmaBreaker.Screens
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields & Properties

        ContentManager content;
        SpriteFont gameFont;

        public Ball Ball { get; set; }
        public Paddle Paddle { get; set; }
        public Level Level { get; set; }

        public Texture2D MenuScreen { get; set; }
        public Vector2 MenuScreenPosition { get; set; }

        public Texture2D UpperScreen { get; set; }
        public Vector2 UpperScreenPosition { get; set; }

        public Texture2D PlayScreen { get; set; }

        public Texture2D Life { get; set; }

        public SpriteFont Font { get; set; }

        Random random = new Random();

        #endregion

        #region Initialization
        public Boolean ballPaused;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            ballPaused = true;
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("gamefont");

            // ball, paddle, bricks, etc loading of content 
            //should be made in everyone's class with contentmanager as parameter
            // example Ball.CustonLoadContent(content);

            Paddle = new Paddle(ScreenManager.GetGame, ScreenManager.GetGDevice);

            Ball = new Ball(ScreenManager.GetGame, ScreenManager.GetGDevice, Paddle);

            Level = new Level(ScreenManager.GetGame, ScreenManager.GetGDevice, content, 1);

            if (ScreenManager.ParticleManager != null)
            {
                ScreenManager.ParticleManager.Reset();
            }

            //temp

            MenuScreen = content.Load<Texture2D>("gamemenuscreen");
            MenuScreenPosition = Vector2.Zero;

            UpperScreen = content.Load<Texture2D>("gameupperscreen");
            UpperScreenPosition = new Vector2(124, 0);

            if (!string.IsNullOrEmpty(Level.BackgroundAssetName))
            {
                try
                {
                    PlayScreen = content.Load<Texture2D>(Level.BackgroundAssetName);
                }
                catch (Exception e)
                {
                    PlayScreen = content.Load<Texture2D>("playscreen");
                    Debug.WriteLine(e.InnerException);
                }
            }
            else
            {
                PlayScreen = content.Load<Texture2D>("playscreen");
            }

            Life = content.Load<Texture2D>("ball");

            Font = content.Load<SpriteFont>("ingamefont");

            
            Thread.Sleep(100);

           SoundManager.PlayMusic("TearingIn");

            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            if (!otherScreenHasFocus)
            {
                SoundManager.ResumePlay();
                Paddle.Update(gameTime);
                Level.Update(gameTime);


                var apwList = Level.ActivePowers.ToArray();
                for(int i = 0; i<apwList.Length;i++)
                {
                    apwList[i].Position = new Vector2(10, 20 + (30 * i));
                    apwList[i].Update(gameTime);
                    if (apwList[i].PowerTimer >= apwList[i].MaxPowerTimer)
                    {
                        Level.ActivePowers.Remove(apwList[i]);
                        apwList[i].RemovePower(Ball, Paddle, Level);
                    }
                }


                //wait for space to be pressed before releasing the ball
                if (!ballPaused)
                {
                    Ball.Update(gameTime);

                    if (Ball.Speed > 0.0f && ScreenManager.ParticleManager.ParticleTrail == null)
                    {
                        var screenRect = EnigmaBreaker.Game.gamePlayScreen;

                        ScreenManager.ParticleManager.SetParticleTrail(Ball, new Vector2(screenRect.X, screenRect.Y)); 
                    }

                    CheckBallAgainstLevel(Ball, Level);
                    CheckBallPaddle(Ball, Paddle);
                }
                else
                {
                    Ball.Position = new Vector2(Paddle.Position.X + (Paddle.Size.X / 2), Paddle.Position.Y - (Ball.Size.Y + 1));
                }
                CheckPaddleAgainstLevel();
            }
            else
            {
                SoundManager.PauseMusic();
            }

            ScreenManager.ParticleManager.Update(gameTime);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public void CheckBallAgainstLevel(Ball ball, Level level)
        {
            #region Ball against Walls
            var screenRect = EnigmaBreaker.Game.gamePlayScreen;

            var ballRect = Ball.GetRectangle;

            // ball coordinates in the gameplay screen!
            ballRect.X += screenRect.X;
            ballRect.Y += screenRect.Y;

            // ballRect only used if checking for collision with level boundaries
            if (ballRect.Right >= screenRect.Right)
            {
                if (ball.Direction.X > 0)
                    ball.Direction = new Vector2(-ball.Direction.X, ball.Direction.Y);
                SoundManager.PlayCue("BallHit");
            }
            else if (ballRect.Left <= screenRect.Left)
            {
                if (ball.Direction.X < 0)
                    ball.Direction = new Vector2(-ball.Direction.X, ball.Direction.Y);
                SoundManager.PlayCue("BallHit");
            }

            if (ballRect.Top <= screenRect.Top)
            {
                if (ball.Direction.Y < 0)
                {
                    ball.Direction = new Vector2(ball.Direction.X, -ball.Direction.Y);
                    SoundManager.PlayCue("BallHit");
                    ball.Bounce(false);
                }
            }
            //if the ball falls down (+25 border size + 50 just for delay)
            if (screenRect.Bottom + EnigmaBreaker.Game.gamePlayScreenBorder + 50 <= ballRect.Top)
            {
                ResetBall();
                SoundManager.PlayCue("BallLost");
            }

            #endregion


            // toArray so we could modify the collection
            Brick[] brickList = level.Bricks.ToArray();

            String hitPattern = "";
            int brickCount = 0;

            for (int i = 0; i < brickList.Length; i++)
            {
                Rectangle brickRect = brickList[i].GetRectangle;
                if (Ball.GetRectangle.Intersects(brickRect))
                {
                    if (IntersectPixels(Ball.GetRectangle, ball.SpriteTextureData, brickRect, brickList[i].SpriteTextureData))
                    {
                        brickCount++;
                        if (Ball.GetRectangle.Center.X > brickRect.Right)
                        {
                            hitPattern += "r";
                        }
                        if (Ball.GetRectangle.Center.X < brickRect.Left)
                        {
                            hitPattern += "l";
                        }
                        if (Ball.GetRectangle.Center.Y < brickRect.Top)
                        {
                            hitPattern += "t";
                        }
                        if (Ball.GetRectangle.Center.Y > brickRect.Bottom)
                        {
                            hitPattern += "b";
                        }

                        if (brickList[i].Hit(Level))
                        {
                            ScreenManager.ParticleManager.AddBrickSpriteExploder(brickList[i], new Vector2(screenRect.X, screenRect.Y));
                        }

                        ScreenManager.ParticleManager.AddParticleExplosion(ball, new Vector2(screenRect.X, screenRect.Y));

                        if (Level.IsCompleted())
                        {
                            LoadNextLevel(Level.LevelNumber);
                        }

                        SoundManager.PlayCue("BrickHit");
                        ball.Bounce(false);
                    }
                }
            }
            if (!Ball.Meteor)
            {
                if (hitPattern != "")
                {
                    Console.WriteLine(hitPattern + " " + brickCount);
                }
                if (brickCount == 1)
                {
                    if (hitPattern.Length == 1) //hits only one side of the brick
                    {
                        if ((hitPattern[0] == 't' || hitPattern[0] == 'b'))
                        {
                            ball.Direction = new Vector2(ball.Direction.X, -ball.Direction.Y);
                        }
                        if ((hitPattern[0] == 'l' || hitPattern[0] == 'r'))
                        {
                            ball.Direction = new Vector2(-ball.Direction.X, ball.Direction.Y);
                        }
                    }
                    else //hits a corner
                    {
                        if (hitPattern == "lb" || hitPattern == "bl")
                        {

                            //bottom left corner is hit
                            ball.Direction = new Vector2(-1f, 1f);
                        }
                        else if (hitPattern == "rb" || hitPattern == "br")
                        {
                            //bottom right corner is hit
                            ball.Direction = new Vector2(1f, 1f);
                        }
                        else if (hitPattern == "lt" || hitPattern == "tl")
                        {
                            //topleft
                            ball.Direction = new Vector2(-1f, -1f);
                        }
                        else
                        {
                            //topright
                            ball.Direction = new Vector2(1f, -1f);
                        }
                    }
                }
                else if (brickCount > 1)//hits several bricks
                {
                    //use hitPattern to figure out bouncing
                    hitPattern = SimplifyHitParttern(hitPattern);
                    Console.WriteLine(hitPattern);
                    if (hitPattern == "")
                    {
                        ball.Direction = new Vector2(-ball.Direction.X, -ball.Direction.Y);
                    }
                    else if (hitPattern[1] == hitPattern[0])
                    {
                        if (hitPattern[0] == 'l' || hitPattern[0] == 'r')
                            ball.Direction = new Vector2(-ball.Direction.X, ball.Direction.Y);
                        if (hitPattern[0] == 't' || hitPattern[0] == 'b')
                            ball.Direction = new Vector2(ball.Direction.X, -ball.Direction.Y);
                    }
                    else ball.Direction = new Vector2(-ball.Direction.X, -ball.Direction.Y);
                }
            }
        }

        public void CheckPaddleAgainstLevel()
        {
            var powerUpList = Level.PowerUps.ToArray();
            foreach (var powerup in powerUpList)
            {
                if (powerup.Active)
                {
                    if (powerup.GetRectangle.Intersects(Paddle.GetRectangle))
                    {
                        powerup.UsePower(Ball, Paddle, Level);
                        Level.RemoveLevelItem(powerup);
                        Level.ActivatePower(powerup);
                    }
                }
            }
        }

        public void CheckBallPaddle(Ball ball, Paddle paddle)
        {
            if (Ball.GetRectangle.Intersects(paddle.GetRectangle))
            {

                if (IntersectPixels(ball.GetRectangle, ball.SpriteTextureData, paddle.GetRectangle, paddle.SpriteTextureData) && !ball.BounceOfPaddle)
                {
                    ball.Direction = new Vector2(ball.Direction.X, -ball.Direction.Y);
                    SoundManager.PlayCue("BallHit");
                    ball.Bounce(true);
                    
                    var ratio = 1.0f;

                    if (ball.Direction.X > 0)
                    {
                        var ballCenter = Ball.BallCenter;

                        ratio = (ballCenter.X - paddle.Position.X) / (paddle.Size.X / 2);
                        if (ratio < Ball.XChangeMin)
                            ratio = Ball.XChangeMin;
                        else if (ratio > Ball.XChangeMax)
                            ratio = Ball.XChangeMax;
                    }
                    else if (ball.Direction.X <= 0)
                    {
                        var ballCenter = Ball.BallCenter;

                        ratio = (paddle.Position.X + paddle.Size.X - ballCenter.X) / (paddle.Size.X / 2);
                        if (ratio < Ball.XChangeMin)
                            ratio = Ball.XChangeMin;
                        else if (ratio > Ball.XChangeMax)
                            ratio = Ball.XChangeMax;
                    }

                    var x = ball.Direction.X * ratio;

                    if (Math.Abs(x) < 0.2f)
                    {
                        if (ball.Direction.X <= 0)
                            x = -0.4f;
                        else
                            x = 0.4f;
                    }

                    // yani - use vector normalization (unit vector)
                    ball.Direction = new Vector2((float)x, ball.Direction.Y);
                }
            }
        }

        public static bool IntersectPixels(Rectangle rectangleA, Color[] dataA,
                                    Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);                
            }
            else
            {
                if (keyboardState.IsKeyDown(Keys.D) && keyboardState.IsKeyDown(Keys.A) && keyboardState.IsKeyDown(Keys.L))
                {
                    // cheat for testing purposes
                    ResetGame(Level.LevelNumber + 1);
                }

                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    ballPaused = false;
                }
                Paddle.HandleInput(keyboardState);
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 0, 0);

            //todo2
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Resolution.getTransformationMatrix());
            //ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend,
            //                       SpriteSortMode.Deferred,
            //                       SaveStateMode.SaveState,
            //                       Resolution.getTransformationMatrix());

            // TODO : position should not be hard-coded
            ScreenManager.SpriteBatch.Draw(MenuScreen,MenuScreenPosition, Color.White);
            ScreenManager.SpriteBatch.Draw(UpperScreen, UpperScreenPosition, Color.White);

            for (var i = 0; i < Level.Lives; i++)
            {
                ScreenManager.SpriteBatch.Draw(Life, UpperScreenPosition + new Vector2(40, 39) + new Vector2(20 * i, 0), Color.White);
            }

            string levelTitle = string.Format("Level: {0}", Level.Title);

            Vector2 textSize = Font.MeasureString(levelTitle);
            Vector2 levelTitlePosition = new Vector2((UpperScreen.Width - textSize.X) / 2 + UpperScreenPosition.X, UpperScreenPosition.Y + 32);
            ScreenManager.SpriteBatch.DrawString(Font, levelTitle, levelTitlePosition, Color.Black);

            string scoreString = string.Format("Scores: {0, 7}", Level.Scores);
            textSize = Font.MeasureString(scoreString);
            Vector2 scoreStringPosition = new Vector2(UpperScreen.Width - textSize.X - 50 + UpperScreenPosition.X, UpperScreenPosition.Y + 32);
            ScreenManager.SpriteBatch.DrawString(Font, scoreString, scoreStringPosition, Color.Black);

            //Drawing PlayScreen
            ScreenManager.SpriteBatch.Draw(PlayScreen, EnigmaBreaker.Game.gamePlayScreen, Color.White);
            ScreenManager.SpriteBatch.End();

            Level.Draw(gameTime);
            Ball.Draw(gameTime);
            Paddle.Draw(gameTime);

            if (ScreenManager.ParticleManager.SpriteBatch == null)
                ScreenManager.ParticleManager.SpriteBatch = ScreenManager.SpriteBatch;
            ScreenManager.ParticleManager.Draw(gameTime);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }

        #endregion

        #region Help Functions

        private void UpdatePlayScreenTexture(string asset)
        {
            PlayScreen = content.Load<Texture2D>(asset); 
        }

        public void LoadNextLevel(int levelNumber)
        {
            levelNumber++;
            ResetGame(levelNumber);
        }
        /// <summary>
        /// This function is used to reset the scores, lifes and levels and start from the scratch.
        /// </summary>
        public void ResetGame()
        {
            ResetGame(1);
        }

        public void ResetGame(int levelNumber)
        {
            // saves scores and lives from the level
            var oldLevelScores = Level.Scores;
            var oldLevelLives = Level.Lives;

            Paddle = new Paddle(ScreenManager.GetGame, ScreenManager.GetGDevice);
            Ball = new Ball(ScreenManager.GetGame, ScreenManager.GetGDevice, Paddle);
            ScreenManager.ParticleManager.Reset();
            PauseBall();

            Level = new Level(ScreenManager.GetGame, ScreenManager.GetGDevice, content, levelNumber);
            UpdatePlayScreenTexture(Level.BackgroundAssetName);

            if (Level.GameOver || oldLevelLives <= 0)
            {
                ScreenManager.AddScreen(new InputNameScreen() { Scores = (int)oldLevelScores, Level = Level.LevelNumber }, ControllingPlayer);
            }
            else
            {
                Level.Lives = oldLevelLives;
                Level.Scores = oldLevelScores;

                if (!string.IsNullOrEmpty(Level.BackgroundAssetName))
                {
                    try
                    {
                        PlayScreen = content.Load<Texture2D>(Level.BackgroundAssetName);
                    }
                    catch (Exception e)
                    {
                        PlayScreen = content.Load<Texture2D>("playscreen");
                        Debug.WriteLine(e.InnerException);
                    }
                }
                else
                {
                    PlayScreen = content.Load<Texture2D>("playscreen");
                }
            }

            ScreenManager.Game.ResetElapsedTime();
        }

        /// <summary>
        /// This function is used to pause the ball. (Paddle, PowerUps, etc.. will still work).
        /// </summary>
        public void PauseBall()
        {
            ballPaused = true;
        }
        /// <summary>
        /// This method is used to reset ball once it is lost.
        /// It will also pause the ball.
        /// </summary>
        public void ResetBall()
        {
            Paddle = new Paddle(ScreenManager.GetGame, ScreenManager.GetGDevice);
            Ball = new Ball(ScreenManager.GetGame, ScreenManager.GetGDevice, Paddle);
            PauseBall();

            ScreenManager.ParticleManager.ParticleTrail = null;

            Level.Lives--;
            Level.ActivePowers = new List<PowerUp>();
            if (Level.Lives <= 0)
            {
                ResetGame(Level.LevelNumber);
            }
        }
        /// <summary>
        /// Simplyfies hit pattern into 2 letters
        /// </summary>
        /// <param name="hitPattern">letters of hit places on the bricks</param>
        /// <returns>new simplyfied hitpattern</returns>
        private String SimplifyHitParttern(String hitPattern)
        {
            if (hitPattern.Length == 1)
                return hitPattern + hitPattern;
            Char[] chars = hitPattern.ToCharArray();

            int lBucket = 0;
            int rBucket = 0;
            int tBucket = 0;
            int bBucket = 0;

            for (int i = 0; i < chars.Length; i++)
            {
                switch (chars[i])
                {
                    case 'b':
                        bBucket++;
                        break;
                    case 't':
                        tBucket++;
                        break;
                    case 'l':
                        lBucket++;
                        break;
                    case 'r':
                        rBucket++;
                        break;
                    default:
                        break;
                }
            }
            int rem = 0;

            if (tBucket >= bBucket)
                rem = bBucket;   
            else
                rem = tBucket;
            tBucket -= rem;
            bBucket -= rem;
            
            rem = 0;
            if (rBucket >= lBucket) 
                rem = lBucket;
            else 
                rem = rBucket;
            lBucket -= rem;
            rBucket -= rem;

            String returnString = "";
            if (bBucket > 0)
            {
                if (bBucket > 1)
                    return "bb";
                else
                    returnString += "b";
            }
            if (tBucket > 0)
            {
                if (tBucket > 1)
                    return "tt";
                else
                    returnString += "t";
            }
            if (lBucket > 0)
            {
                if (lBucket > 1)
                    return "ll";
                else
                    returnString += "l";
            }
            if (rBucket > 0)
            {
                if (rBucket > 1)
                    return "rr";
                else
                    returnString += "r";
            }

            return returnString;
        }
        #endregion
    }
}
