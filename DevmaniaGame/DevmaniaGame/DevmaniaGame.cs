
using DevmaniaGame.Framework.Drawing;
using DevmaniaGame.Framework.Input;
using DevmaniaGame.Framework.States;
using DevmaniaGame.Framework.States.Transitions;
using DevmaniaGame.Game;
using DevmaniaGame.Intro;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame
{
    public class DevmaniaGame : Microsoft.Xna.Framework.Game
    {
        public const int ScreenWidth = 800;
        public const int ScreenHeight = 600;
        public const int BorderTop = 50;
        public const int Border = 30;

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private StateManager _stateManager;

        public DevmaniaGame()
        {
            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = ScreenWidth,
                PreferredBackBufferHeight = ScreenHeight,
#if !DEBUG
                IsFullScreen = true,
#endif
            };
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            _stateManager = new StateManager(GraphicsDevice) {Content = Content};
            
            _stateManager.RegisterTransition(new BlendTransition());
            _stateManager.RegisterTransition(new FlipTransition(GraphicsDevice));
            _stateManager.RegisterTransition(new GrowTransition(GraphicsDevice));
            _stateManager.RegisterTransition(new SlideTransition(GraphicsDevice));
            _stateManager.RegisterTransition(new CardTransition(GraphicsDevice));
            _stateManager.RegisterTransition(new ThrowAwayTransition(GraphicsDevice));

            _stateManager.RegisterState(new InGameState());
            _stateManager.RegisterState(new ResultGame());
            _stateManager.RegisterState(new IntroState());
            _stateManager.RegisterState(new ManualState());
            _stateManager.SetCurrentState(IntroState.Id, null);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            ShapeRenderer.LoadContent(Content, GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardEx.Update();

            if(!_stateManager.Update(gameTime))
                Exit();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _stateManager.Draw(gameTime, _spriteBatch);
            base.Draw(gameTime);
        }

        static void Main()
        {
            using (var game = new DevmaniaGame())
            {
                game.Run();
            }
        }
    }
}