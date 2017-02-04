using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Framework.States.Transitions
{
    class ThrowAwayTransition : ITransition
    {
        public static readonly Type Id = typeof(ThrowAwayTransition);

        private readonly GraphicsDevice _device;
        private readonly BasicEffect _basicEffect;
        private float _elapsedTime;
        private const float AnimationTime = 1.0f;
        private readonly VertexBuffer _vertexBuffer;

        public ThrowAwayTransition(GraphicsDevice device)
        {
            _device = device;

            _basicEffect = new BasicEffect(device)
            {
                LightingEnabled = false, 
                TextureEnabled = true,
                Projection = Matrix.CreatePerspective(2.0f, 2.0f, 1.0f, 1000.0f),
                View = Matrix.Identity
            };
            
            var vertices = new[]
            {
                new VertexPositionTexture(new Vector3(-1,-1,0),new Vector2(0,1)), 
                new VertexPositionTexture(new Vector3(-1, 1,0),new Vector2(0,0)), 
                new VertexPositionTexture(new Vector3( 1,-1,0),new Vector2(1,1)), 
                new VertexPositionTexture(new Vector3( 1, 1,0),new Vector2(1,0)),
 
                new VertexPositionTexture(new Vector3( 1,-1,0),new Vector2(0,1)), 
                new VertexPositionTexture(new Vector3( 1, 1,0),new Vector2(0,0)), 
                new VertexPositionTexture(new Vector3(-1,-1,0),new Vector2(1,1)), 
                new VertexPositionTexture(new Vector3(-1, 1,0),new Vector2(1,0))
            };

            _vertexBuffer = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration, 8, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);
        }

        public Texture2D Source { get; set; }
        public Texture2D Target { get; set; }
        public bool TransitionReady { get; set; }

        public void Begin()
        {
            TransitionReady = false;
            _elapsedTime = 0;

        }

        public void Update(float elapsedTime)
        {
            _elapsedTime += elapsedTime;
            if (_elapsedTime > AnimationTime)
            {
                _elapsedTime = AnimationTime;
                TransitionReady = true;
            }

            float delta = _elapsedTime/AnimationTime;

            const float totalTurnageValue = MathHelper.TwoPi*4 + MathHelper.PiOver2;

            _basicEffect.World = Matrix.CreateRotationX(delta * totalTurnageValue) * Matrix.CreateTranslation(0.0f, 0.0f, -1.0f - MathHelper.SmoothStep(0, 60, delta));
        }

        public void Render(SpriteBatch spriteBatch)
        {
            _device.Clear(Color.Black);
            _device.RasterizerState = RasterizerState.CullCounterClockwise;

            _device.SetVertexBuffer(_vertexBuffer);
            RenderSide(Source, 0);
            RenderSide(Source, 4);
        }

        private void RenderSide(Texture2D texture, int vertexOffset)
        {
            _basicEffect.Texture = texture;
            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _device.DrawPrimitives(PrimitiveType.TriangleStrip, vertexOffset, 2);
            }
        }
    }
}
