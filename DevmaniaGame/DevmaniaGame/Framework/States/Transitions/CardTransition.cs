using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Framework.States.Transitions
{
    internal class CardTransition : ITransition
    {
        public static readonly Type Id = typeof (CardTransition);

        private readonly VertexBuffer _vertexBuffer;
        private readonly GraphicsDevice _device;
        private readonly BasicEffect _basicEffect;
        private float _elapsedTime;
        private const float AnimationTime = 0.6f;

        public Texture2D Source { get; set; }
        public Texture2D Target { get; set; }
        public bool TransitionReady { get; set; }

        public CardTransition(GraphicsDevice device)
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
                                   new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
                                   new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)),
                                   new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1)),
                                   new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0))
                               };

            _vertexBuffer = new VertexBuffer(device, VertexPositionTexture.VertexDeclaration, 4, BufferUsage.WriteOnly);
            _vertexBuffer.SetData(vertices);
        }

        public void Begin()
        {
            _elapsedTime = 0;
            TransitionReady = false;
        }

        public void Update(float elapsedTime)
        {
            _elapsedTime += elapsedTime;
            if (_elapsedTime >= AnimationTime)
            {
                TransitionReady = true;
                _elapsedTime = AnimationTime;
            }
        }

        private void RenderQuad(Texture2D texture, Matrix world)
        {
            _basicEffect.Texture = texture;
            _basicEffect.World = world;
            foreach (EffectPass pass in _basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            }
        }

        public void Render(SpriteBatch spriteBatch)
        {
            _device.DepthStencilState = DepthStencilState.None;
            float delta = MathHelper.SmoothStep(0, 1, _elapsedTime/AnimationTime);

            _device.SetVertexBuffer(_vertexBuffer);
            RenderQuad(Target, Matrix.CreateTranslation(0.0f, 0.0f, -1.0f));
            RenderQuad(Source, Matrix.CreateTranslation(0.0f, delta * 2.0f, -1.0f));
        }
    }
}
