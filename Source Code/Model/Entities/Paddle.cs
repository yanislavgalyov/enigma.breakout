using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using EnigmaBreaker.Model;

namespace EnigmaBreaker.Model.Entities
{
    public class Paddle : BasicSprite
    {
        protected float MoveSpeed { get; set; }

        public Paddle(Game game, GraphicsDevice pDevice)
            : base(game, pDevice)
        {
            MoveSpeed = 10f;

            Size = new Vector2(125, 10);
            Texture = game.Content.Load<Texture2D>("paddle");
            Position = new Vector2(351, 650);

            SpriteTextureData = new Color[Texture.Width * Texture.Height];
            Texture.GetData(SpriteTextureData);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (Texture == null)
                return;

            base.Draw(gameTime);
        }

        public void HandleInput(KeyboardState keyboardState)
        {
            Vector2 newPos = this.Position;

            if (keyboardState.IsKeyDown(Keys.Left))
            {
                newPos.X -= MoveSpeed;
                if (newPos.X < 0)
                    newPos.X = 0;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                newPos.X += MoveSpeed;
                if (newPos.X + this.Size.X > EnigmaBreaker.Game.gamePlayScreen.Width)
                    newPos.X = EnigmaBreaker.Game.gamePlayScreen.Width - this.Size.X;
            }

            this.Position = newPos;
        }
    }
}
