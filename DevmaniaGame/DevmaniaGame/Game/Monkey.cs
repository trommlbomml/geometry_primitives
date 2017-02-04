using System;
using DevmaniaGame.Framework.Collision;
using DevmaniaGame.Framework.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Game
{
    enum MonkeyState
    {
        Idle=0,
        Hungry=1,
        Eating=2,
        Chewing=3
    }

    class Monkey : IDepthSortable
    {

        private static readonly int[] NumsFoodTypes = {0, 0, 0};
        private const float BubbleFadeTime = 0.3f;

        private Texture2D _bubbles;
        public Vector2 Position;
        private MonkeyState _state;
        private static Random _random=new Random();
        private FoodType _requiredFood;
        private bool _left;
        private float _timer;
        private float _bubbleAlpha;
        public float AnimationTargetY;

        private SoundEffect[] _deliverySounds;
        private SoundEffect _deliverWrongSfx;

        private AnimatedSprite _sitAnimation;
        private AnimatedSprite _eatAnimation;
        private AnimatedSprite _chewingAnimation;
        private AnimatedSprite _currentAnimation;


        private const int FrameSize = 256;

        private Player _deliveringPlayer;
        private FoodSize _deliveredSize;

        private const float MaxWaitingTime = 5;

        public const float ComboTime = 4;

        public Circle DeliveryBounds { get; private set; }
        public Circle CollisionBounds { get; private set; }

        private Popup _popup;

        public Monkey(ContentManager content, Vector2 position, bool left)
        {
            _bubbles = content.Load<Texture2D>("Textures/bubbles");
            Position = position;
            _left = left;
            _state = MonkeyState.Idle;
            _requiredFood = RandomFoodType();
            _timer = 1;
            _bubbleAlpha=0;
            DeliveryBounds = new Circle((int)Position.X, (int)Position.Y, 80);
            CollisionBounds = new Circle((int)Position.X, (int)Position.Y, 30);
            _deliveringPlayer = null;
            DepthRenderer.Register(this);

            _deliverySounds = new []
                              {
                                  content.Load<SoundEffect>("Sound/deliver"),
                                  content.Load<SoundEffect>("Sound/monkey1")
                              };
            _deliverWrongSfx = content.Load<SoundEffect>("Sound/deliver_wrong");

            Texture2D texture = content.Load<Texture2D>("textures/monkey2");
            _sitAnimation = new AnimatedSprite(texture, FrameSize, FrameSize, 0, 3, 6, 0.5f, true, left ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            _eatAnimation = new AnimatedSprite(texture, FrameSize, FrameSize, 0, 6, 20, 1.0f, false, left ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            _chewingAnimation = new AnimatedSprite(texture, FrameSize, FrameSize, 0, 7, 6, 1.0f, true, left ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
            _currentAnimation = _sitAnimation;
            _popup = new Popup(content);
        }

        public void Start()
        {
            _state=MonkeyState.Hungry;
            _currentAnimation.Play();
            _timer = 2;
            _popup.StartPosition = new Vector2(Position.X, AnimationTargetY - 40);
        }

        public void Stop()
        {
            _state = MonkeyState.Idle;
            _currentAnimation = _sitAnimation;
            _currentAnimation.Play();
        }

        public static FoodType RandomFoodType()
        {
            int newType = _random.Next((int)FoodType.Count);
            if (NumsFoodTypes[newType] > 1)
                newType = (newType + 1)%(int) FoodType.Count;
            NumsFoodTypes[newType]++;
            return (FoodType) newType;
        }

        private float _rotation;

        public void Update(float elapsedTime, Player p1, Player p2)
        {
            DeliveryBounds = new Circle((int)Position.X + (_left ? 40 : -40), (int)Position.Y + FrameSize / 4, 40);
            CollisionBounds = new Circle((int)Position.X, (int)Position.Y + FrameSize / 4, 30);
            _timer -= elapsedTime;
            _currentAnimation.Update(elapsedTime);
            _rotation += 0.4f*elapsedTime;
            switch (_state)
            {
                case MonkeyState.Idle:
                    HandleIdle();
                    break;
                case MonkeyState.Hungry:
                    HandleHungry();
                    break;
                case MonkeyState.Eating:
                    HandleEating();
                    break;
                case MonkeyState.Chewing:
                    HandleChewing();
                    break;
            }

           _popup.Update(elapsedTime);

           if (p1.CollectedFood != null && p1.Bounds.Intersects(DeliveryBounds))
               DeliverFood(p1, p1.CollectedFood);
           if (p2.CollectedFood != null && p2.Bounds.Intersects(DeliveryBounds))
               DeliverFood(p2, p2.CollectedFood);
        }

        private void HandleIdle()
        {
           
        }

        private void HandleHungry()
        {
            if (_timer > 0)
                _bubbleAlpha = MathHelper.SmoothStep(0, 1, (BubbleFadeTime - _timer) / BubbleFadeTime);
            else
            {
                _bubbleAlpha = 1;
            }
            if(_timer<-MaxWaitingTime)
            {
                NumsFoodTypes[(int) _requiredFood]--;
                _requiredFood = RandomFoodType();
                _timer = 0;
            }
        }

        private void HandleEating()
        {
            if (!_currentAnimation.Running)
            {
                _state = MonkeyState.Chewing;
                _currentAnimation = _chewingAnimation;
                _currentAnimation.Play();
                bool gotCombo = _deliveringPlayer.ComboTimer > 0;
                _deliveringPlayer.Score+=(_deliveredSize==FoodSize.Large?2:1)*(gotCombo?2:1);
                _deliveringPlayer.ComboTimer += ComboTime;
                _deliveringPlayer = null;
                
                _timer = (_random.Next(9)+2)*0.4f;
            }
        }

        private void HandleChewing()
        {
            if (_timer<0)
            {
                _state=MonkeyState.Hungry;
                _currentAnimation = _sitAnimation;
                _currentAnimation.Play();
                _timer = BubbleFadeTime;
                NumsFoodTypes[(int) _requiredFood]--;
                _requiredFood = RandomFoodType();
            }
                
        }
        
        public int Depth
        {
            get { return (int)Math.Round(Position.Y + FrameSize / 4, 0); }
        }

        private void DeliverFood(Player player,Food food)
        {
            if(_state==MonkeyState.Hungry && food.Type==_requiredFood)
            {
                _state = MonkeyState.Eating;
                _currentAnimation = _eatAnimation;
                _currentAnimation.Play();
                _deliveringPlayer = player;
                _deliveredSize = player.CollectedFood.FoodSize;
                player.CollectedFood = null;
                _timer = 0.0f;
                PlayRandomDeliverySound();
                bool gotCombo = _deliveringPlayer.ComboTimer > 0;
                _popup.Activate(gotCombo ? PopupType.Combo : PopupType.Right);
            }
            else if(_state==MonkeyState.Hungry&&food.Type!=_requiredFood)
            {
                player.CollectedFood = null;
                player.Score-=2;
                _popup.Activate(PopupType.Wrong);
                if (player.Score < 0)
                    player.Score = 0;
                _deliverWrongSfx.Play();
            }
        }

        private void PlayRandomDeliverySound()
        {
            if (_random.Next(4) == 1)
                _deliverySounds[1].Play();
            else
                _deliverySounds[0].Play();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _currentAnimation.Draw(spriteBatch, Position, new Vector2(FrameSize) * 0.5f);
            
            if(_state == MonkeyState.Hungry)
            {
                DrawBubble(spriteBatch);
            }
            //ShapeRenderer.DrawCircle(spriteBatch,CollisionBounds,16,Color.White);
        }

        private void DrawBubble(SpriteBatch spriteBatch)
        {
            Rectangle bubbleRectangle = new Rectangle((int)_requiredFood * 128, 256, 128,128);
            Rectangle shapeRectangle = new Rectangle((int)_requiredFood * 128, 384, 128,128);
            Vector2 position = Position + new Vector2(_left ? 90 : -90, -32);

            spriteBatch.Draw(_bubbles, position, bubbleRectangle, Color.White * _bubbleAlpha, 0.0f, new Vector2(64),1.0f, _left ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            spriteBatch.Draw(_bubbles, position, shapeRectangle, Color.White * _bubbleAlpha, _rotation, new Vector2(64), 1.0f, SpriteEffects.None, 0);
        }

        public void DrawPopup(SpriteBatch spriteBatch)
        {
             _popup.Draw(spriteBatch);
        }
    }
}
