
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Game
{
    enum InGameCallOutType
    {
        Ready = 0,
        Go = 1,
        TenSeconds = 2,
        Three = 3,
        Two = 4,
        One = 5,
        Over = 6
    }

    class InGameCallOutRenderer
    {
        private readonly Texture2D _texture;

        public InGameCallOutRenderer(ContentManager content)
        {
            _texture = content.Load<Texture2D>("textures/ingamecallouts");
        }

        public void Draw(SpriteBatch spriteBatch, InGameCallOutType callOutType, float alpha)
        {
            Rectangle rectangle = new Rectangle(0, (int)callOutType * 128, 1024, 128);
            float scale = 1;
            if (callOutType == InGameCallOutType.Go && alpha < 0.9)
                scale =1/(alpha + 0.1f);
            spriteBatch.Draw(_texture, new Vector2(DevmaniaGame.ScreenWidth, DevmaniaGame.ScreenHeight) * 0.5f,rectangle, 
                Color.White * alpha, 0.0f, new Vector2(512, 64),scale, SpriteEffects.None, 0 );
        }
    }
}
