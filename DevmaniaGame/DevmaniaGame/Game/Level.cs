
using System;
using System.Collections.Generic;
using System.Linq;
using DevmaniaGame.Framework.Collision;
using DevmaniaGame.Framework.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Game
{
    class Level
    {
        private const int MinAvailableFoods = 2;
        private const int MaxAvailableFoods = 10;

        private ContentManager _content;
        private Texture2D _background;
        private List<Food> _foodList;
        private List<FallingFood> _fallingFoodList;
        private float _foodSpawnTimer;
        private Random _random;
        private bool _batchDropping;
        private int _numToDrop;
        private Vector2 _batchCenter;

        private static readonly Vector2[] DropCenters = new[]
                                                            {
                                                                new Vector2(400, 320), new Vector2(400, 190),
                                                                new Vector2(400, 470), new Vector2(610, 320),
                                                                new Vector2(180, 320)
                                                            };
        private const int DropSpreadX = 170;
        private const int DropSpreadY = 120;

        private float _sparkleTimer;

        private ParticleManager _particleManager;
        
        public Level(ContentManager content, ParticleManager particleManager)
        {
            _content = content;
            _random = new Random();
            _background = content.Load<Texture2D>("Textures/background");
            _foodSpawnTimer = 0.2f;
            _batchDropping = true;
            _batchCenter=DropCenters[0];
            _numToDrop = 3+_random.Next(5);
            _foodList=new List<Food>();
            _fallingFoodList = new List<FallingFood>();
            _sparkleTimer = 1;
            _particleManager = particleManager;
        }

        public void Update(float elapsedTime)
        {
            _foodSpawnTimer -= elapsedTime;
            if (_foodSpawnTimer < 0)
            {
                Food newFood = new Food(_content, (FoodType)_random.Next((int)FoodType.Count), Vector2.Zero);
                Vector2 newPosition = Vector2.Zero;
                bool generate = true;
                while(generate)
                {
                    newPosition = new Vector2(_batchCenter.X + _random.Next(DropSpreadX) - DropSpreadX/2, _batchCenter.Y + _random.Next(DropSpreadY) - DropSpreadY/2);
                    Circle newBounds;
                    newBounds.X = (int)newPosition.X;
                    newBounds.Y = (int)newPosition.Y;
                    newBounds.Radius = Food.CollisionSizes[newFood.FoodSize];
                    generate = _foodList.Any(f => f.Bounds.Intersects(newBounds));
                }
                FallingFood newFallingFood = new FallingFood(newFood, _random.Next(500, 700), newPosition);
                _fallingFoodList.Add(newFallingFood);
                _particleManager.AddParticle((ParticleType)newFood.FoodSize,newFallingFood.MyFood.Position);

                if (_batchDropping)
                {
                    if (_numToDrop > 0)
                    {
                        //spawn next food in batch
                        _numToDrop--;
                        _foodSpawnTimer += (_random.Next(9) + 2) * 0.01f;//In-Batch drop Time
                    }
                    else
                    {
                        //spawn next batch
                        _batchDropping = false;
                        _numToDrop = _random.Next(4) + 2;
                        _foodSpawnTimer = _numToDrop * 0.7f + _random.Next(9) * 0.5f;//Wait between Batches
                        _batchCenter = DropCenters[_random.Next(DropCenters.Length)];
                    }
                }
                else
                {
                    _numToDrop--;
                    _batchDropping = true;
                    _foodSpawnTimer += (_random.Next(9) + 2) * 0.01f;//In-Batch drop Time
                }
            }
            else if (_foodList.Count + _fallingFoodList.Count + _numToDrop < MinAvailableFoods && _foodSpawnTimer > 0.5f)
                _foodSpawnTimer = 0.5f;
            else if (_foodList.Count + _fallingFoodList.Count + _numToDrop > MaxAvailableFoods && _foodSpawnTimer < 1.5f)
                _foodSpawnTimer += 1.5f;

            foreach (Food food in _foodList)
            {
                food.Update(elapsedTime);
            }
            List<FallingFood> toRemove=new List<FallingFood>();
            foreach (FallingFood food in _fallingFoodList)
            {
                if(!food.Update(elapsedTime))
                {
                    toRemove.Add(food);
                }
            }

            foreach (FallingFood fallingFood in toRemove)
            {
                _foodList.Add(fallingFood.MyFood);
                DepthRenderer.Register(fallingFood.MyFood);
                _fallingFoodList.Remove(fallingFood);
            }
            _sparkleTimer -= elapsedTime;
            if(_sparkleTimer<0)
            {
                if(_foodList.Count>0)
                {
                    int index = _random.Next(_foodList.Count);
                    _particleManager.AddParticle(ParticleType.Blinky, _foodList[index].Position + new Vector2(_random.Next(-20, 20), _random.Next(-20, 20)));
                }
                _sparkleTimer += (_random.Next(6) + 1)*0.2f;
            }
        }

        public bool TryCollectFood(Player player, out Food collectedFood)
        {
            collectedFood = _foodList.FirstOrDefault(f => f.Bounds.Intersects(player.Bounds));
            if (collectedFood != null)
            {
                _foodList.Remove(collectedFood);
                return true;
            }
            return false;
        }

        public void DropFood(Player player, Food droppedFood)
        {
            droppedFood.Position.Y += 35;
            _foodList.Add(droppedFood);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_background, Vector2.Zero, Color.White);
            foreach (Food food in _foodList)
            {
                food.DrawShadow(spriteBatch);
            }
            foreach (FallingFood fallingFood in _fallingFoodList)
            {
                fallingFood.DrawShadow(spriteBatch);
            }
        }

        public void DrawFallingFood(SpriteBatch spriteBatch)
        {
            foreach (FallingFood fallingFood in _fallingFoodList)
            {
                fallingFood.Draw(spriteBatch);
            }
        }
    }
}
