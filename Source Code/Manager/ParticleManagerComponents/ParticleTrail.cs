using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using EnigmaBreaker.Model.Entities;
using EnigmaBreaker.Model;

namespace EnigmaBreaker.Manager.ParticleManagerComponents
{
    public class ParticleTrailPiece
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public byte Transparency { get; set; }
        public float Scale { get; set; }
        public float Age { get; set; }
        public float MaxAge { get; set; }

        public ParticleTrailPiece()
        {
            Age = 0.0f;
            MaxAge = 3.0f;
            Transparency = 255;
            Scale = 1.0f;
        }

        public ParticleTrailPiece(BasicSprite bs, Vector2 offAlign)
            : this()
        {
            Texture = bs.Texture;
            Position = bs.Position + offAlign;
        }
    }

    public class ParticleTrail
    {
        BasicSprite BasicSprite { get; set; }
        Vector2 OffAling { get; set; }
        List<ParticleTrailPiece> Pieces { get; set; }
        float ElapsedTime { get; set; }
        float SpawingTime { get; set; }

        public bool IsActive { get; set; }

        public ParticleTrail()
        {
            IsActive = true;
            ElapsedTime = 0.0f;
            SpawingTime = 0.3f;

            Pieces = new List<ParticleTrailPiece>();
        }

        public ParticleTrail(BasicSprite bs, Vector2 offAlign)
            : this()
        {
            BasicSprite = bs;
            OffAling = offAlign;
        }

        public void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                var elapsedTimeMS = (float)gameTime.ElapsedGameTime.Milliseconds / 100;
                ElapsedTime += elapsedTimeMS;

                if (ElapsedTime > SpawingTime)
                {
                    ElapsedTime = 0.0f;
                    ParticleTrailPiece ptp = new ParticleTrailPiece(BasicSprite, OffAling);
                    Pieces.Add(ptp);
                }

                foreach (ParticleTrailPiece piece in Pieces.ToList())
                {
                    if (piece.Age >= piece.MaxAge || piece.Scale < 0.1f || piece.Transparency < 10)
                    {
                        Pieces.Remove(piece);
                    }
                    else
                    {
                        piece.Age += elapsedTimeMS;
                        piece.Scale -= elapsedTimeMS / 5;
                        piece.Transparency -= (byte)(elapsedTimeMS * 80);
                    }
                    
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (ParticleTrailPiece piece in Pieces)
            {
                if (piece.Age < piece.MaxAge)
                {
                    if (piece.Texture != null)
                    {
                        spriteBatch.Draw(piece.Texture, piece.Position, null, new Color(255,255, 255, piece.Transparency), 0.0f, Vector2.One, piece.Scale, SpriteEffects.None, 0);
                    }
                    else
                    {
                        piece.Age = piece.MaxAge;
                    }
                }
            }
        }
    }

    

}
