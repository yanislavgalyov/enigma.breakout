using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EnigmaBreaker.Manager.ParticleManagerComponents;
using EnigmaBreaker.Model.Entities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using EnigmaBreaker.Model;

namespace EnigmaBreaker.Manager
{
    public class ParticleManager : DrawableGameComponent
    {
        private SpriteBatch _spriteBatch;
        public SpriteBatch SpriteBatch
        {
            get
            {
                return _spriteBatch;
            }
            set
            {
                _spriteBatch = value;
            }
        }

        private ContentManager _content;

        protected List<SpriteExploder> SpriteExploders { get; set; }
        public ParticleTrail ParticleTrail { get; set; }

        protected List<ParticleExplosion> ParticleExplosions { get; set; }

        public ParticleManager(Game game)
            : base(game)
        {
            SpriteExploders = new List<SpriteExploder>();
            ParticleExplosions = new List<ParticleExplosion>();
            _content = game.Content;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public void AddBrickSpriteExploder(Brick b, Vector2 offAlign)
        {
            SpriteExploder se = new SpriteExploder(b, offAlign, 5, 5);

            SpriteExploders.Add(se);            
        }

        public void AddParticleExplosion(Ball b, Vector2 offAlign)
        {
            ParticleExplosion pe = new ParticleExplosion(b, offAlign, _content);

            ParticleExplosions.Add(pe);
        }

        public void SetParticleTrail(Ball b, Vector2 offAlign)
        {
            ParticleTrail = new ParticleTrail(b, offAlign);
        }

        public void Reset()
        {
            SpriteExploders.Clear();
            ParticleTrail = null;
            ParticleExplosions.Clear();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var exploder in SpriteExploders.ToList())
            {
                if (!exploder.IsActive)
                {
                    SpriteExploders.Remove(exploder);
                    continue;
                }

                exploder.Update(gameTime);
            }

            if (ParticleTrail != null)
            {
                ParticleTrail.Update(gameTime);
            }

            foreach (var explosion in ParticleExplosions.ToList())
            {
                if (!explosion.IsActive)
                {
                    ParticleExplosions.Remove(explosion);
                    continue;
                }

                explosion.Update(gameTime);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //todo2
            _spriteBatch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, null, null, null, null, Resolution.getTransformationMatrix());
            //_spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Texture, SaveStateMode.None, Resolution.getTransformationMatrix());

            foreach (var exploder in SpriteExploders)
            {
                exploder.Draw(gameTime, _spriteBatch);
            }

            if (ParticleTrail != null)
            {
                ParticleTrail.Draw(gameTime, _spriteBatch);
            }

            foreach (var explosion in ParticleExplosions)
            {
                explosion.Draw(gameTime, _spriteBatch);
            }

            _spriteBatch.End();
        }
    }
}
