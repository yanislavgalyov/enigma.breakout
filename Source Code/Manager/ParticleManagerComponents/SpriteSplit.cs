using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EnigmaBreaker.Model.Entities;

namespace EnigmaBreaker.Manager.ParticleManagerComponents
{

  public struct SpritePiece
  {
    public Vector2 Position;
    public Vector2 Velocity;
    public Rectangle SourceRectangle;
    public float RotationRate;
    public float Angle;
    public float Age;
    public float MaxAge;
  }

  public class SpriteExploder
  {
    private List<SpritePiece> pieces;
    private Vector2 position;
    private Vector2 origin;

    private Texture2D Texture;

    public bool IsActive { get; set; }

    public List<SpritePiece> Pieces { get { return pieces; } }

    public SpriteExploder(Brick b, Vector2 OffAlign, int pieceWidth, int pieceHeight)
    {
        Random R = new Random();

        IsActive = true;

        this.position = b.Position + OffAlign;
        this.Texture = b.Texture;
        int spriteWidth = Texture.Width;
        int spriteHeight = Texture.Height;

        this.origin = new Vector2((float)spriteWidth * 0.5f, (float)spriteHeight * 0.5f);

        // explostion of the texture at its center!
        this.position += this.origin; 

        pieces = new List<SpritePiece>();

        // create an instance of each sprite piece
        for (int x = 0; x < spriteWidth; x += pieceWidth)
            for (int y = 0; y < spriteHeight; y += pieceHeight)
            {
                SpritePiece piece = new SpritePiece();

                piece.Position = position + new Vector2(x, y);

                float vx = x - ((float)spriteWidth * 0.5f);
                float vy = y - ((float)spriteHeight * 0.5f);

                piece.Velocity = new Vector2(vx, vy);
                if (piece.Velocity == Vector2.Zero)
                    piece.Velocity.X += 0.1f;

                float l = piece.Velocity.Length() * 3.0f;
                piece.Velocity.Normalize();
                piece.Velocity *= (100.0f + l);

                piece.SourceRectangle = new Rectangle(x, y, pieceWidth, pieceHeight);

                piece.Age = 0.0f;
                piece.MaxAge = 2.0f;
                piece.Angle = 0;
                piece.RotationRate = 45.0f * (float)R.NextDouble();

                pieces.Add(piece);
            }
    }

    public SpriteExploder(Vector2 position, int spriteWidth, int spriteHeight, int pieceWidth, int pieceHeight)
    {
      Random R = new Random();

      IsActive = true;

      this.position = position;
      this.origin = new Vector2((float)spriteWidth * 0.5f, (float)spriteHeight * 0.5f);
      
      pieces = new List<SpritePiece>();

      // create an instance of each sprite piece
      for (int x = 0; x < spriteWidth; x += pieceWidth)
        for (int y = 0; y < spriteHeight; y += pieceHeight)
        {
          SpritePiece piece = new SpritePiece();

          piece.Position = position + new Vector2(x, y);

          float vx = x - ((float)spriteWidth * 0.5f);
          float vy = y - ((float)spriteHeight * 0.5f);

          piece.Velocity = new Vector2(vx, vy);
          if (piece.Velocity == Vector2.Zero)
            piece.Velocity.X += 0.1f;

          float l = piece.Velocity.Length() * 3.0f;
          piece.Velocity.Normalize();
          piece.Velocity *= (100.0f + l);

          piece.SourceRectangle = new Rectangle(x, y, pieceWidth, pieceHeight);

          piece.Age = 0.0f;
          piece.MaxAge = 25.0f;
          piece.Angle = 0;
          piece.RotationRate = 45.0f * (float)R.NextDouble();

          pieces.Add(piece);
        }

    }

    public void Update(GameTime gameTime)
    {
        if (IsActive)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < pieces.Count; i++)
            {
                SpritePiece piece = pieces[i];

                // continue on if the piece is dead
                if (piece.Age == -1)
                {
                    IsActive = false;
                    continue;
                }

                // update age - if past max age then kill, otherwise update position

                piece.Age += elapsedTime;
                if (piece.Age >= piece.MaxAge)
                    piece.Age = -1;
                else
                {
                    piece.Angle = (piece.Angle + piece.RotationRate * elapsedTime) % 360.0f;
                    piece.Position += piece.Velocity * elapsedTime;

                    if (piece.SourceRectangle.Width > 1 && piece.SourceRectangle.Height > 1)
                    {
                        var decreasingValue = (int)Math.Floor(piece.Age);
                        piece.SourceRectangle.Width -= decreasingValue;
                        piece.SourceRectangle.Height -= decreasingValue;
                    }
                    else 
                    {
                        piece.Age = - 1;
                    }
                }

                pieces[i] = piece;
            }
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (Texture != null)
        {
            foreach (SpritePiece piece in pieces)
                if (piece.Age >= 0)
                    spriteBatch.Draw(Texture, piece.Position, piece.SourceRectangle, Color.White, (float)MathHelper.ToRadians(piece.Angle), origin, Vector2.One, SpriteEffects.None, 0);
        }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Texture2D spriteTexture)
    {
      foreach(SpritePiece piece in pieces)
        if (piece.Age >= 0)
          spriteBatch.Draw(spriteTexture, piece.Position, piece.SourceRectangle, Color.White, (float)MathHelper.ToRadians(piece.Angle), origin, Vector2.One, SpriteEffects.None, 0);
    }
  }
}
