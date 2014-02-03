using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using EnigmaBreaker.Model;
using Microsoft.Xna.Framework;

namespace EnigmaBreaker.Model.Entities
{
    public enum PowerUpType
    {
        SpeedUp = 1,
        SpeedDown = 2,
        PaddleInc = 3,
        PaddleDec = 4,
        Meteor = 5
    }
    
    

    public class PowerUp : BasicSprite
    {
        public PowerUpType Type { get; set; }
        public Boolean Active { get; set; }
        public float Speed { get; set; }
        public Boolean PowerActivated { get; set; }
        public float PowerTimer { get; set; }
        public float MaxPowerTimer { get; set; }
        public SpriteFont Font { get; set; }

        private const float speedChange = 0.01f;

        /// <summary>
        /// Paddle constructor.
        /// </summary>
        /// <param name="x">x coordinate of the paddle</param>
        /// <param name="y">y coordinate of the paddle</param>
        /// <param name="texture">Paddle texture</param>
        public PowerUp(Game game, GraphicsDevice pDevice, PowerUpType type)
            : base(game, pDevice)
        {
            Speed = 3f;
            Active = false;
            PowerActivated = false;
            this.PowerTimer = 0f;
            Type = type;
            int tempTimer = 0;
            switch (type)
            {
                case PowerUpType.SpeedUp:
                    this.Texture = game.Content.Load<Texture2D>("powerup_speedup");
                    tempTimer = 15;
                    break;
                case PowerUpType.SpeedDown:
                    this.Texture = game.Content.Load<Texture2D>("powerup_speeddown");
                    tempTimer = 15;
                    break;
                case PowerUpType.PaddleInc:
                    this.Texture = game.Content.Load<Texture2D>("powerup_paddleinc");
                    tempTimer = 60;
                    break;
                case PowerUpType.PaddleDec:
                    this.Texture = game.Content.Load<Texture2D>("powerup_paddledec");
                    tempTimer = 60;
                    break;
                case PowerUpType.Meteor:
                    this.Texture = game.Content.Load<Texture2D>("powerup_meteor");
                    tempTimer = 30;
                    break;
                default:
                    this.Texture = game.Content.Load<Texture2D>("blank");
                    tempTimer = 1;
                    break;
            }
            Font = game.Content.Load<SpriteFont>("gamefont");
            MaxPowerTimer = tempTimer * 10000000;
            SpriteTextureData = new Color[Texture.Width * Texture.Height];
            Texture.GetData(SpriteTextureData);
        }

        /// <summary>
        /// Called each time we need to update Object
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Active)
            {
                Position = new Vector2(Position.X, Position.Y + Speed);
                base.Update(gameTime);
            }
            if (PowerActivated)
            {
                PowerTimer += gameTime.ElapsedGameTime.Ticks;
                //Console.WriteLine(PowerTimer);
            }
        }

        /// <summary>
        /// Called each time we need to draw Object
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Active)
            {
                base.Draw(gameTime);
            }
            if (PowerActivated)
            {
                if (PowerTimer < 1000000)
                    return;
                if (Texture == null)
                    return;

                //todo2
                SpriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, null, Resolution.getTransformationMatrix());
                //SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.None, Resolution.getTransformationMatrix());
                //create offset rectangle so that it places it right on the gameplayscreen and ignores position of other screens
                Rectangle r = new Rectangle(this.GetRectangle.X, this.GetRectangle.Y, (int)Size.X, (int)Size.Y);
                SpriteBatch.Draw(Texture, r, ColorTint);
                String str = ((int)((MaxPowerTimer - PowerTimer)/10000000)).ToString();
                Vector2 textPos = new Vector2(this.GetRectangle.Right + 10, this.GetRectangle.Top - 10);
                Color textColor = new Color(255, 255, 255);
                SpriteBatch.DrawString(Font, str, textPos, textColor);
                SpriteBatch.End();
            }
        }

        public void UsePower(Ball ball, Paddle paddle, Level level)
        {
            switch (this.Type)
            {
                case PowerUpType.SpeedUp:
                    ball.Speed += speedChange;
                    break;
                case PowerUpType.SpeedDown:
                    ball.Speed -= speedChange;
                    break;
                case PowerUpType.PaddleInc:
                    if (paddle.Size.X < 200)
                    {
                        paddle.Size = new Vector2(paddle.Size.X + 25, paddle.Size.Y);
                        paddle.Position = new Vector2(paddle.Position.X - 12.5f, paddle.Position.Y);
                    }
                    break;
                case PowerUpType.PaddleDec:
                    if (paddle.Size.X > 50)
                    {
                        paddle.Size = new Vector2(paddle.Size.X - 25, paddle.Size.Y);
                        paddle.Position = new Vector2(paddle.Position.X + 12.5f, paddle.Position.Y);
                    }
                    break;
                case PowerUpType.Meteor:
                    ball.Meteor = true;
                    break;
                default:
                    
                    break;
            }
            level.Scores += 100;
            Active = false;
        }

        /// <summary>
        /// Inverts the action of the UsePower method
        /// </summary>
        /// <param name="ball">Ball object in the game.</param>
        /// <param name="paddle">Paddle object in the game.</param>
        /// <param name="level">Level object in the game.</param>
        public void RemovePower(Ball ball, Paddle paddle, Level level)
        {
            switch (this.Type)
            {
                case PowerUpType.SpeedUp:
                    ball.Speed -= speedChange;
                    break;
                case PowerUpType.SpeedDown:
                    ball.Speed += speedChange;
                    break;
                case PowerUpType.PaddleInc:
                    if (paddle.Size.X > 50)
                    {
                        paddle.Size = new Vector2(paddle.Size.X - 25, paddle.Size.Y);
                        paddle.Position = new Vector2(paddle.Position.X + 12.5f, paddle.Position.Y);
                    }
                    break;
                case PowerUpType.PaddleDec:
                    if (paddle.Size.X < 200)
                    {
                        paddle.Size = new Vector2(paddle.Size.X + 25, paddle.Size.Y);
                        paddle.Position = new Vector2(paddle.Position.X - 12.5f, paddle.Position.Y);
                    }
                    break;
                case PowerUpType.Meteor:
                    ball.Meteor = false;
                    break;
                default:

                    break;
            }
            PowerActivated = false;
        }
    }
}
