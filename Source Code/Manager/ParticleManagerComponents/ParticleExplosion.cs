using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using EnigmaBreaker.Model.Entities;
using Microsoft.Xna.Framework.Content;
using EnigmaBreaker.Model;

namespace EnigmaBreaker.Manager.ParticleManagerComponents
{
    public class ParticleExplosion
    {
        private Texture2D _texture { get; set; }
        private Vector2 _position { get; set; }
        private float _scale { get; set; }
        private Color _color { get; set; }

        public bool IsActive { get; set; }

        public ParticleExplosion()
        {
            _scale = 0.2f;
            IsActive = true;
            _color = new Color(0, 0, 0, 88);
        }

        public ParticleExplosion(Ball b, Vector2 offAlign, ContentManager content)
            : this()
        {
            _texture = content.Load<Texture2D>("spot");
            _position = b.Position + offAlign;
        }

        public void Update(GameTime gameTime)
        {
            if (_scale > 2.0f)
            {
                IsActive = false;
            }
            else
            {
                _scale += 0.07f;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_texture != null)
            {
                Vector2 newPos = _position - (new Vector2(_texture.Width, _texture.Height) * _scale / 2);
                spriteBatch.Draw(_texture, newPos, null, _color, 0.0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
            }
        }
    }
}
