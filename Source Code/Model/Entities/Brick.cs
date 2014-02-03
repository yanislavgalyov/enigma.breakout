using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using EnigmaBreaker.Model;
using Microsoft.Xna.Framework;

namespace EnigmaBreaker.Model.Entities
{
    /// <summary>
    /// Enum that shows what type bricks are.
    /// Makes it esier to keep track of different type bricks that require different looks.
    /// </summary>
    public enum BrickType 
    { 
        Normal = 1, 
        Ice = 2, 
        Stone = 3, 
        Metal = 4, 
        Rogue = 5 
    };

    public class Brick : BasicSprite
    {
        public PowerUp powerUp;

        private BrickType _type;
        public BrickType Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;

                switch (_type)
                {
                    case BrickType.Normal:
                        this.ColorTint = Color.White;
                        break;
                    case BrickType.Ice:
                        this.ColorTint = Color.LightBlue;
                        break;
                    case BrickType.Stone:
                        this.ColorTint = Color.SteelBlue;
                        break;
                    case BrickType.Rogue:
                        this.ColorTint = Color.Transparent; //todo2
                        break;
                    case BrickType.Metal:
                        this.ColorTint = Color.Blue;
                        break;
                    default:
                        this.ColorTint = Color.White;
                        break;
                }
            }
        }

        public Brick(Game game, GraphicsDevice pDevice)
            : base(game, pDevice)
        {
        }

        /// <summary>
        /// Call this method each time brick gets a hit from the ball.
        /// This method will destroy the brick or weaken it once hit.
        /// </summary>
        /// <param name="level">Reference of the Level so that we can properly remove this brick if it is needed to.</param>
        public bool Hit(Level level)
        {           
            switch (this.Type)
            {
                case BrickType.Rogue:
                    level.Scores += 100;
                    this.Type = BrickType.Normal;
                    break;
                case BrickType.Metal:
                    level.Scores += 50;
                    this.Type = BrickType.Stone;
                    break;
                case BrickType.Stone:
                    level.Scores += 30;
                    this.Type = BrickType.Ice;
                    break;
                case BrickType.Ice:
                    level.Scores += 10;
                    this.Type = BrickType.Normal;
                    break;
                default:
                    level.Scores += 200;
                    if (powerUp != null)
                    {
                        powerUp.Active = true;
                    }
                    level.RemoveLevelItem(this);
                    return true;                    
            }

            return false;
        }
    }
}
