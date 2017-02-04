
using System;
using System.Collections.Generic;
using DevmaniaGame.Framework.Collision;
using DevmaniaGame.Framework.Drawing;
using DevmaniaGame.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DevmaniaGame.Game
{
    enum LookDirection
    {
        South = 0,
        SouthEast = 1,
        East = 2,
        NorthEast = 3,
        North = 4,
        NorthWest = 5,
        West = 6,
        SouthWest = 7,
        Idle = 255,
    }

    class Player : IDepthSortable
    {
        private Keys _upKey;
        private Keys _downKey;
        private Keys _leftKey;
        private Keys _rightKey;
        private Keys _actionKey;
        private bool _leftForFood;
        private LookDirection _currentDirection;
        private Dictionary<LookDirection, AnimatedSprite> _animations;
        private AnimatedSprite _currentAnimation;
        private SoundEffect _takeSfx;
        private SoundEffect _putSfx;

        private const int FrameSize = 128;
        private const int FrameCount = 6;
        private const int OffsetCollisionShapeY = 32;

        public Vector2 Speed;
        public Vector2 MaxSpeed;
        public Vector2 IncSpeed;
        public Vector2 BrakeSpeed;
        public Vector2 Position;
        public float AnimationTargetY;

        public int Score;
        public Circle Bounds;
        public Food CollectedFood { get; set; }

        public float ComboTimer;
        private Texture2D _comboImgGrey;
        private Texture2D _comboImgHighlite;
        private Rectangle _comboRect;
        private bool _left;

        private ParticleManager _particleManager;
        private float _dustTimer;

        public Player(int playerIndex, ContentManager content)
        {
            Score = 0;
            Speed = Vector2.Zero;
            MaxSpeed = new Vector2(300);
            IncSpeed = new Vector2(800);
            BrakeSpeed = new Vector2(600);
            FillFromPlayerIndex(playerIndex);
            Bounds = new Circle((int)Position.X, (int)Position.Y + OffsetCollisionShapeY, 20);
            DepthRenderer.Register(this);
            _leftForFood = false;
            _currentDirection = LookDirection.Idle;

            _takeSfx = content.Load<SoundEffect>("Sound/take");
            _putSfx = content.Load<SoundEffect>("Sound/put");

            _particleManager = new ParticleManager(content);

            Texture2D playerTexture = content.Load<Texture2D>(playerIndex == 1 ? "textures/player1" : "textures/player2");
            _animations = new Dictionary<LookDirection, AnimatedSprite>();
            _animations.Add(LookDirection.South, CreateAnimation(playerTexture, 3, SpriteEffects.None));
            _animations.Add(LookDirection.SouthEast, CreateAnimation(playerTexture, 4, SpriteEffects.None));
            _animations.Add(LookDirection.East, CreateAnimation(playerTexture,5, SpriteEffects.None));
            _animations.Add(LookDirection.NorthEast, CreateAnimation(playerTexture, 6, SpriteEffects.None));
            _animations.Add(LookDirection.North, CreateAnimation(playerTexture, 7, SpriteEffects.None));
            _animations.Add(LookDirection.NorthWest, CreateAnimation(playerTexture, 6, SpriteEffects.FlipHorizontally));
            _animations.Add(LookDirection.West, CreateAnimation(playerTexture, 5, SpriteEffects.FlipHorizontally));
            _animations.Add(LookDirection.SouthWest, CreateAnimation(playerTexture, 4, SpriteEffects.FlipHorizontally));
            _animations.Add(LookDirection.Idle, new AnimatedSprite(playerTexture, FrameSize, FrameSize, 0, 2, 1, 0.1f, false, SpriteEffects.None));
            _currentAnimation = _animations[_currentDirection];
            _currentAnimation.Play();

            ComboTimer = -1;
            _comboImgGrey = content.Load<Texture2D>("Textures/combo_dead");
            _comboImgHighlite = content.Load<Texture2D>("Textures/combo_glow");

            _comboRect=new Rectangle(playerIndex==1?0:240,24,150,72);
            _left = playerIndex == 1;
            _dustTimer = 0.0f;
        }

        private AnimatedSprite CreateAnimation(Texture2D texture, int startY, SpriteEffects effect)
        {
            return new AnimatedSprite(texture, FrameSize, FrameSize, 0, startY, FrameCount, 0.3f, true, effect);
        }

        private void FillFromPlayerIndex(int index)
        {
            switch (index)
            {
                case 1:
                    _upKey = Keys.W;
                    _downKey = Keys.S;
                    _leftKey = Keys.A;
                    _rightKey = Keys.D;
                    _actionKey = Keys.Tab;
                    break;
                case 2:
                    _upKey = Keys.Up;
                    _downKey = Keys.Down;
                    _leftKey = Keys.Left;
                    _rightKey = Keys.Right;
                    _actionKey = Keys.Space;
                    break;
            }
        }

        public void Bounce(Player other)
        {
            Vector2 direction;
            if (Math.Abs(Speed.LengthSquared()) < 0.001f)
            {
                direction = Vector2.Normalize(other.Position - Position);
            }
            else
            {
                direction = Vector2.Normalize(Speed);
            }
            Speed = -direction * MaxSpeed.X * GetSpeedModificator();
        }

        private float GetSpeedModificator()
        {
            if (CollectedFood == null)
                return 1.0f;
            if (CollectedFood.FoodSize == FoodSize.Medium)
                return 0.8f;
            if (CollectedFood.FoodSize == FoodSize.Large)
                return 0.5f;

            return 1.0f;
        }

        public void Update(float elapsedTime, Level level, IEnumerable<Monkey> monkeys)
        {
            float modificator = GetSpeedModificator();

            _currentAnimation.Update(elapsedTime);

            Point dir = Point.Zero;
            if (KeyboardEx.IsKeyDown(_rightKey))
            {
                dir.X = 1;
                Speed.X += IncSpeed.X * elapsedTime * modificator;
                if (Speed.X > MaxSpeed.X * modificator)
                    Speed.X = MaxSpeed.X * modificator;
                  _leftForFood = false;
            }
            else if (KeyboardEx.IsKeyDown(_leftKey))
            {
                dir.X = -1;
                Speed.X -= IncSpeed.X * elapsedTime * modificator;
                if (Speed.X < -MaxSpeed.X * modificator)
                    Speed.X = -MaxSpeed.X * modificator;
                _leftForFood = true;
            }
            else
            {
                if (Speed.X > 0)
                {
                    Speed.X -= BrakeSpeed.X * elapsedTime * modificator;
                    if (Speed.X < 0) Speed.X = 0;
                }
                else if (Speed.X < 0)
                {
                    Speed.X += BrakeSpeed.X * elapsedTime * modificator;
                    if (Speed.X > 0) Speed.X = 0;
                }
            }

            if (KeyboardEx.IsKeyDown(_downKey))
            {
                dir.Y = 1;
                Speed.Y += IncSpeed.Y * elapsedTime * modificator;
                if (Speed.Y > MaxSpeed.Y * modificator)
                    Speed.Y = MaxSpeed.Y * modificator;
            }
            else if (KeyboardEx.IsKeyDown(_upKey))
            {
                dir.Y = -1;
                Speed.Y -= IncSpeed.Y * elapsedTime * modificator;
                if (Speed.Y < -MaxSpeed.Y * modificator)
                    Speed.Y = -MaxSpeed.Y * modificator;
            }
            else
            {
                if (Speed.Y > 0)
                {
                    Speed.Y -= BrakeSpeed.Y * elapsedTime * modificator;
                    if (Speed.Y < 0) Speed.Y = 0;
                }
                else if (Speed.Y < 0)
                {
                    Speed.Y += BrakeSpeed.Y * elapsedTime * modificator;
                    if (Speed.Y > 0) Speed.Y = 0;
                }
            }

            LookDirection newDirection = GetLookDirectionFromPoint(dir);
            if (_currentDirection != newDirection)
            {
                _currentDirection = newDirection;
                SetCurrentAnimation(_currentDirection);
            }

            if (KeyboardEx.IsKeyDownOnce(_actionKey))
            {
                if (CollectedFood == null)
                {
                    Food food;
                    if (level.TryCollectFood(this, out food))
                    {
                        CollectedFood = food;
                        DepthRenderer.UnRegister(food);
                        _takeSfx.Play();
                    }   
                }
                else
                {
                    level.DropFood(this,CollectedFood);
                    DepthRenderer.Register(CollectedFood);
                    CollectedFood = null;
                    _putSfx.Play();
                }
            }

            Vector2 newPosition = Position + Speed*elapsedTime;

            if (CollidesWithMonkeys(monkeys, newPosition + new Vector2(0, OffsetCollisionShapeY)))
            {    
                newPosition = Position;
                Speed = Vector2.Zero;
            }
            else
            {
                if (newPosition.X < DevmaniaGame.Border + 20)
                {
                    newPosition.X = DevmaniaGame.Border + 20;
                    Speed = Vector2.Zero;
                }
                else if (newPosition.X > DevmaniaGame.ScreenWidth - DevmaniaGame.Border - 20)
                {
                    newPosition.X = DevmaniaGame.ScreenWidth - DevmaniaGame.Border - 20;
                    Speed = Vector2.Zero;
                }

                if (newPosition.Y < DevmaniaGame.BorderTop + 40)
                {
                    newPosition.Y = DevmaniaGame.BorderTop + 40;
                    Speed = Vector2.Zero;
                }
                else if (newPosition.Y > DevmaniaGame.ScreenHeight - DevmaniaGame.Border - 60)
                {
                    newPosition.Y = DevmaniaGame.ScreenHeight - DevmaniaGame.Border - 60;
                    Speed = Vector2.Zero;
                }
            }

            Position = newPosition;

            Bounds.X = (int) Position.X;
            Bounds.Y = (int) Position.Y+OffsetCollisionShapeY;

            if (CollectedFood != null)
            {
                CollectedFood.Position = Position + new Vector2((_leftForFood ? -20 : 20), -15);
            }

            ComboTimer -= elapsedTime;
            if (ComboTimer < -1)
                ComboTimer = -1;

            HandleDustTrail(elapsedTime);
        }

        private void HandleDustTrail(float elapsedTime)
        {
            _dustTimer -= elapsedTime;
            if (Speed.LengthSquared() > 250 * 250 && _dustTimer <= 0)
            {
                _particleManager.AddParticle(Speed.X > 0 ? ParticleType.DustL : ParticleType.DustR, Position + new Vector2(0, 30));
                _dustTimer = 0.1f;
            }
            _particleManager.Update(elapsedTime);
        }

        private bool CollidesWithMonkeys(IEnumerable<Monkey> monkeys, Vector2 newPosition)
        {
            Circle circle = new Circle((int) newPosition.X, (int) newPosition.Y, Bounds.Radius);
            foreach (Monkey monkey in monkeys)
            {
                if (circle.Intersects(monkey.CollisionBounds))
                    return true;
            }
            return false;
        }

        private LookDirection GetLookDirectionFromPoint(Point p)
        {
            if (p.X ==  1 && p.Y ==  0) return LookDirection.East;
            if (p.X ==  1 && p.Y ==  1) return LookDirection.SouthEast;
            if (p.X ==  0 && p.Y ==  1) return LookDirection.South;
            if (p.X == -1 && p.Y ==  1) return LookDirection.SouthWest;
            if (p.X == -1 && p.Y ==  0) return LookDirection.West;
            if (p.X == -1 && p.Y == -1) return LookDirection.NorthWest;
            if (p.X ==  0 && p.Y == -1) return LookDirection.North;
            if (p.X ==  1 && p.Y == -1) return LookDirection.NorthEast;
            return LookDirection.Idle;
        }

        private void SetCurrentAnimation(LookDirection direction)
        {
            int currentFrame = direction == LookDirection.Idle ? 0 : _currentAnimation.CurrentFrame;
            float currentTime = direction == LookDirection.Idle ? 0.0f : _currentAnimation.CurrentTime;

            _currentAnimation = _animations[direction];
            _currentAnimation.Play(currentFrame, currentTime);
        }

        public int Depth { get { return (int)Math.Round(Position.Y + OffsetCollisionShapeY, 0); } }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (CollectedFood != null)
                CollectedFood.DrawShadow(spriteBatch,60);
            _particleManager.Draw(spriteBatch);
            _currentAnimation.Draw(spriteBatch, Position, new Vector2(64));
            
            if (CollectedFood != null)
                CollectedFood.Draw(spriteBatch);
        }

        public void DrawCombo(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_comboImgGrey,new Vector2(_left?0:800,(ComboTimer<0?-ComboTimer*170:0)+600),_comboRect,Color.White,0,new Vector2(_left?0:150,60),1,SpriteEffects.None, 0);
            spriteBatch.Draw(_comboImgHighlite, new Vector2(_left ? 0 : 800, 600), _comboRect, Color.White * (ComboTimer / Monkey.ComboTime), 0, new Vector2(_left ? 0 : 150, 60), 1, SpriteEffects.None, 0);
        }
    }
}
