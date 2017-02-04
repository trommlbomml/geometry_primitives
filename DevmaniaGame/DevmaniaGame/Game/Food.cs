using System;
using System.Collections.Generic;
using DevmaniaGame.Framework.Collision;
using DevmaniaGame.Framework.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Game
{
    internal enum FoodType
    {
        FoodCircle = 0,
        FoodTriangle = 1,
        FoodQuad = 2,
        Count = 3
    };

    internal enum FoodSize
    {
        Small = 0,
        Medium = 1,
        Large = 2
    };
    class Food : IDepthSortable
    {
        static readonly Random Rand = new Random();

        public static readonly Dictionary<FoodSize, int> CollisionSizes = new Dictionary<FoodSize, int>
                                                                               {
                                                                                   {FoodSize.Small, 15},
                                                                                   {FoodSize.Medium, 18},
                                                                                   {FoodSize.Large, 30}
                                                                               };
       

        public FoodType Type { get; private set; }
        private Texture2D _image;
        private Rectangle _foodTile;
        public Vector2 Position;
        public Circle Bounds;
        public FoodSize FoodSize;

        public Texture2D Shadow;

        public Food(ContentManager content, FoodType type, Vector2 position)
        {
            Type = type;
            Position = position;
            int sizeRand = Rand.Next(10);
            int index;
            if (sizeRand < 4)
            {
                FoodSize = FoodSize.Small;
                index = Rand.Next(4);
            }
            else if (sizeRand < 9)
            {
                FoodSize = FoodSize.Medium;
                index = Rand.Next(4);
            }
            else
            {
                FoodSize = FoodSize.Large;
                index = Rand.Next(2);
            }

            int scale;
            switch (FoodSize)
            {
                case FoodSize.Small:
                    _image = content.Load<Texture2D>("Textures/fruit_s");
                    scale = 32;
                    break;
                case FoodSize.Medium:
                    _image = content.Load<Texture2D>("Textures/fruit_m");
                    scale = 64;
                    break;
                case FoodSize.Large:
                    _image = content.Load<Texture2D>("Textures/fruit_l");
                    scale = 96;
                    break;
                default:
                    scale = 1;
                    break;
            }

            Shadow = content.Load<Texture2D>("Textures/shadow");
            _foodTile = new Rectangle(index*scale,(3-(int)Type)*scale,scale,scale);
            Bounds = new Circle((int) Position.X, (int) Position.Y, CollisionSizes[FoodSize]);

            DepthRenderer.Register(this);
        }

        public void Update(float timeElapsed)
        {
            Bounds.X = (int) Position.X;
            Bounds.Y = (int) Position.Y;
        }

        public int Depth { get { return (int)Math.Round(Position.Y, 0); } }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_image, Position, _foodTile, Color.White, 0.0f, new Vector2(0.5f * _foodTile.Width,0.5f * _foodTile.Height), 1.0f, SpriteEffects.None, 0);
        }

        public void DrawShadow(SpriteBatch spriteBatch,int yOff=20)
        {
            spriteBatch.Draw(Shadow,Position+new Vector2(0,yOff-(FoodSize==FoodSize.Small?20:0)),null,Color.White,0,new Vector2(64,108),
                GetScaleFromFoodSize(FoodSize)* 1.2f, SpriteEffects.None, 0);
        }

        public static float GetScaleFromFoodSize(FoodSize foodSize)
        {
            switch (foodSize)
            {
                case FoodSize.Medium: return 0.5f;
                case FoodSize.Small: return 0.25f;
                default: return 1.0f;
            }
        }
    }
}
