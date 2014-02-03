using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using EnigmaBreaker;
using System.Diagnostics;

namespace EnigmaBreaker.Model.Entities
{
    public class Level
    {
        #region Properties

        protected Game Engine { get; set; }
        protected GraphicsDevice Graphics { get; set; }
        public int LevelNumber { get; set; }
        public List<Brick> Bricks { get; set; }
        public List<PowerUp> PowerUps { get; set; }
        public List<PowerUp> ActivePowers { get; set; }

        public int Lives { get; set; }

        public long Scores { get; set; }

        public string Title { get; set; }
        public string BackgroundAssetName { get; set; }

        public bool GameOver { get; set; }

        #endregion

        public Level(Game engine, GraphicsDevice graphics, ContentManager content)
        {
            this.Engine = engine;
            this.Graphics = graphics;
            this.Scores = 0;
            this.Lives = 3;
            this.LevelNumber = 1;
            this.Bricks = new List<Brick>();
            this.PowerUps = new List<PowerUp>();
            this.ActivePowers = new List<PowerUp>();
            this.GameOver = false;

            LoadLevel();
        }

        public Level(Game engine, GraphicsDevice graphics, ContentManager content, int levelNumber)
        {
            this.Engine = engine;
            this.Graphics = graphics;
            this.Scores = 0;
            this.Lives = 3;
            this.LevelNumber = levelNumber;
            this.Bricks = new List<Brick>();
            this.PowerUps = new List<PowerUp>();
            this.ActivePowers = new List<PowerUp>();
            this.GameOver = false;

            LoadLevel();
        }

        public void Update(GameTime gameTime)
        {
            var pwList = PowerUps.ToArray();
            foreach (PowerUp pw in pwList)
            {
                pw.Update(gameTime);
                if (pw.Position.Y > EnigmaBreaker.Game.gamePlayScreen.Bottom + 100)
                {
                    RemoveLevelItem(pw);
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            foreach (Brick brick in Bricks)
            {
                brick.Draw(gameTime);
            }
            foreach (PowerUp pw in PowerUps)
            {
                pw.Draw(gameTime);
            }

            foreach (PowerUp pw in ActivePowers)
            {
                pw.Draw(gameTime);
            }
        }

        public Boolean IsCompleted()
        {
            return Bricks.Count == 0;
        }

        public void RemoveLevelItem(EnigmaBreaker.Model.BasicSprite item)
        {
            if (item is Brick)
                this.Bricks.Remove((Brick)item);
            else if (item is PowerUp)
                this.PowerUps.Remove((PowerUp)item);
        }

        private void LoadLevel()
        {
            String levelString = String.Empty;

            try
            {
                levelString = this.Engine.Content.Load<String>("Levels/" + this.LevelNumber);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.InnerException);

                if (this.LevelNumber > 1)
                {
                    this.LevelNumber = 1;
                }

                levelString = this.Engine.Content.Load<String>("Levels/1");
            }
            Texture2D brickTexture = this.Engine.Content.Load<Texture2D>("brick"); 

            //remove spaces
            levelString = levelString.Replace(" ", "");
            //remove new lines
            levelString = levelString.Replace("\n", "");
            //split into rows
            String[] rows = levelString.Split(',');

            this.Title = rows[0].Replace('@', ' ');

            this.BackgroundAssetName = rows[1];

            for (int row = 3; row < rows.Length; row++)
            {
                for (int colum = 0; colum < rows[row].Length; colum++)
                {
                    string r = rows[row][colum].ToString();
                    if (r != "0")
                    {
                        //Create a brick and add it to the list
                        Brick brick = new Brick(this.Engine, this.Graphics);
                        brick.Position = new Vector2(colum * EnigmaBreaker.Game.gamePlayScreenBorder * 2, (row - 1) * EnigmaBreaker.Game.gamePlayScreenBorder);
                        brick.Texture = brickTexture;
                        brick.Size = new Vector2(50, 25);
                        BrickType t = (BrickType)int.Parse(r);
                        brick.Type = t;

                        brick.SpriteTextureData = new Color[brick.Texture.Width * brick.Texture.Height];
                        brick.Texture.GetData(brick.SpriteTextureData);
                        this.Bricks.Add(brick);
                    }
                }
            }

            int powerupsCount = int.Parse(rows[2]);
            Random random = new Random();

            //creating a temp brick list
            List<Brick> tempBrickList = new List<Brick>(this.Bricks);

            //lets start adding powerups
            Random rand = new Random();
            for (int i = 0; i < powerupsCount; i++)
            {
                //if there are no more bricks left
                if (tempBrickList.Count == 0)
                {
                    break; //stop adding powerups
                }
                else //otherwise
                {
                    
                    //pick a random brick from the list
                    Brick tempBrick = tempBrickList.ElementAt(random.Next(0, tempBrickList.Count));
                    // TODO: change the hardcoded 6 into the ammount of different PowerUpTypes we have
                    //PowerUpType.Count doesnt work here, find new way.
                    addPowerUpToBrick(tempBrick, rand.Next(1, 6));  // Enum.GetValues(typeof(PowerUpType)).Length = 6 , but it is bad for performance
                    //remove this brick from the list, so we wont try to add powerups to it anymore
                    tempBrickList.Remove(tempBrick);
                }
            }
        }
        
        private void addPowerUpToBrick(Brick brick, int powerupType)
        {
            brick.powerUp = new PowerUp(this.Engine, this.Graphics, (PowerUpType)powerupType);
            brick.powerUp.Position = brick.Position;
            brick.powerUp.Size = brick.Size;
            this.PowerUps.Add(brick.powerUp);
        }

        public void ActivatePower(PowerUp powerUp)
        {
            if (powerUp.Type == PowerUpType.Meteor)
            {
                PowerUp[] pws = ActivePowers.ToArray();
                foreach (PowerUp pw in pws)
                {
                    if (pw.Type == powerUp.Type)
                    {
                        pw.MaxPowerTimer += powerUp.MaxPowerTimer;
                        return;
                    }
                }
            }
            this.ActivePowers.Add(powerUp);
            powerUp.PowerActivated = true;
        }
    }
}
