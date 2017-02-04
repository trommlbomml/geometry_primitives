
using System;
using DevmaniaGame.Framework.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Game
{
    class BalanceBar
    {
        private readonly Texture2D _mask;
        private readonly SpriteFont _timeFont;

        private readonly Color _left = new Color(189, 3, 42);
        private readonly Color _right = new Color(29, 162, 131);

        public BalanceBar(ContentManager content)
        {
            _mask = content.Load<Texture2D>("Textures/gui");
            _timeFont = content.Load<SpriteFont>("Fonts/TimerFont");
        }

        public void Render(SpriteBatch spriteBatch, Player p1, Player p2, float remainingTime)
        {
            DrawBalanceBar(spriteBatch, p1, p2);
            DrawTime(spriteBatch, remainingTime);
        }

        private void DrawBalanceBar(SpriteBatch spriteBatch, Player p1, Player p2)
        {
            int diff = p1.Score - p2.Score;
            int bar = 400 + (int)(350 * Math.Pow(diff / (1.0 + Math.Abs(diff)), 5));
            ShapeRenderer.DrawFilledRectangle(spriteBatch, 0, 7, bar, 26, _left);
            ShapeRenderer.DrawFilledRectangle(spriteBatch, bar, 7, 800 - bar, 26, _right);

            spriteBatch.Draw(_mask, Vector2.Zero, Color.White);
        }

        private void DrawTime(SpriteBatch spriteBatch, float remainingTime)
        {
            int seconds = (int)Math.Truncate(remainingTime);
            int milliseconds = (int)Math.Truncate((remainingTime - seconds) * 100);

            string text = string.Format("{0:00}:{1:00}", seconds, milliseconds);
            float len = _timeFont.MeasureString(text).X;

            spriteBatch.DrawString(_timeFont, text, new Vector2(400 - len * 0.5f, 36), Color.White);
        }

    }
}
