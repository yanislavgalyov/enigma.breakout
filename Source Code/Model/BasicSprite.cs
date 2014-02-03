using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using EnigmaBreaker.Model;

namespace EnigmaBreaker.Model
{
    public partial class BasicSprite : Microsoft.Xna.Framework.GameComponent
    {
        public Game Engine { get; set; }
        public SpriteBatch SpriteBatch = null;
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 Size { get; set; }
        public Color ColorTint { get; set; }

        public Color[] SpriteTextureData { get; set; }


        public Rectangle GetRectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
            }
        }

        public BasicSprite(Game game, GraphicsDevice pDevice)
            : base(game)
        {
            Engine = (EnigmaBreaker.Game)game;
            SpriteBatch = new SpriteBatch(pDevice);

            ColorTint = Color.White;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime)
        {
            if (Texture == null)
                return;

            //todo2
            SpriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, null, Resolution.getTransformationMatrix());
            //SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.None, Resolution.getTransformationMatrix());
            //create offset rectangle so that it places it right on the gameplayscreen and ignores position of other screens
            Rectangle r = new Rectangle(EnigmaBreaker.Game.gamePlayScreen.X + GetRectangle.X, EnigmaBreaker.Game.gamePlayScreen.Y + GetRectangle.Y, (int)Size.X, (int)Size.Y);
            SpriteBatch.Draw(Texture, r, ColorTint);
            SpriteBatch.End();
        }
    }
}
