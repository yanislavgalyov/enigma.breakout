using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using EnigmaBreaker.Model;

namespace EnigmaBreaker.Model.Entities
{

    public class Ball : BasicSprite
    {
        #region Properties

        public Boolean Meteor { get; set; }
        public Boolean BounceOfPaddle { get; set; }
        public float Speed { get; set; }
        public Vector2 Direction { get; set; }
        public Vector2 OldDirection { get; set; }

        public Vector2 StepMovement { get; set; }
        public Vector2 BallCenter
        {
            get
            {
                return new Vector2(this.Position.X + this.Size.X / 2, this.Position.Y + this.Size.Y / 2);
            }
        }

        public static float XChangeMax = 1.3f;
        public static float XChangeMin = 0.7f;

        public bool BallGoestThroughPaddle { get; set; }

        #endregion

        public Ball(Game game, GraphicsDevice pDevice, Paddle paddle)
            : base(game, pDevice)
        {
            Speed = 0.05f;

            BallGoestThroughPaddle = false;
            BounceOfPaddle = true;
            Meteor = false;

            Size = new Vector2(16, 16);
            Texture = game.Content.Load<Texture2D>("ball");
            Position = new Vector2(paddle.Position.X + (paddle.Size.X / 2), paddle.Position.Y - (Size.Y + 1));
            Direction = new Vector2(1f, -1f);
            OldDirection = Direction;

            SpriteTextureData = new Color[Texture.Width * Texture.Height];
            Texture.GetData(SpriteTextureData);
        }

        public override void Initialize()
        {
            base.Initialize();
        }
  
        public override void Update(GameTime gameTime)
        {
            //Displays directional ball speed
            //Console.WriteLine(Math.Sqrt((Direction.X * Direction.X) + (Direction.Y * Direction.Y)));
            if (OldDirection != Direction)
            {
                OldDirection = Direction;
                StepMovement = Vector2.Zero;
            }

            // as we have direction we take a distant point on the line of direction and use Vector2.Lerp to interpolate the ball towards it
            if (StepMovement == null || StepMovement == Vector2.Zero)
            {
                Vector2 DistantDestination = Direction * 100 + Position;
                Vector2 newPosition = Vector2.Lerp(Position, DistantDestination, Speed);

                StepMovement = newPosition - Position;
            }           

            Position += StepMovement;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (Texture == null)
                return;

            base.Draw(gameTime);
        }
        public void Bounce(Boolean ofPaddle)
        {
            if (!ofPaddle)
            {
                BounceOfPaddle = false;
            }
            else
            {
                BounceOfPaddle = true;
            }
            Speed += 0.0001f;
        }
    }
}
