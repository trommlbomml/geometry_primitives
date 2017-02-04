using System;
using DevmaniaGame.Framework.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using DevmaniaGame.Framework.Extensions;

namespace DevmaniaGame.Framework.Drawing
{
    static class ShapeRenderer
    {
        private static Texture2D _blank;

        public static void LoadContent(ContentManager content, GraphicsDevice device)
        {
            Texture2D texture = new Texture2D(device, 1, 1);
            texture.SetData(new[] { Color.White });
            _blank = texture;
        }

        public static void DrawLine(SpriteBatch spriteBatch, int x1, int y1, int x2, int y2, Color color)
        {
            int dX = x2 - x1;
            int dY = y2 - y1;
            float length = (float)Math.Sqrt(dX * dX + dY * dY);

            spriteBatch.Draw(_blank, new Vector2(x1, y1), null, color, (float)Math.Atan2(dY, dX), Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }

        public static void DrawRectangle(SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
        {
            spriteBatch.Draw(_blank, new Rectangle(x, y, width, 1), color);
            spriteBatch.Draw(_blank, new Rectangle(x, y + height - 1, width, 1), color);
            spriteBatch.Draw(_blank, new Rectangle(x, y, 1, height), color);
            spriteBatch.Draw(_blank, new Rectangle(x + width - 1, y, 1, height), color);
        }

        public static void DrawFilledRectangle(SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
        {
            spriteBatch.Draw(_blank, new Rectangle(x,y,width,height), color);
        }

        public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle target, Color color)
        {
            DrawRectangle(spriteBatch, target.X, target.Y, target.Width, target.Height, color);
        }

        public static void DrawCircle(SpriteBatch spriteBatch, int x, int y, int radius, int slices, Color color)
        {
            float deltaAngle = 1 / (float)slices * MathHelper.TwoPi;
            float segmentWidth = 2f * (float)Math.Sin(deltaAngle * 0.5f) * radius;

            for (int i = 0; i <= slices; i++)
            {
                spriteBatch.Draw(_blank,
                    new Vector2(x + (float)(Math.Cos(i * deltaAngle) * radius),
                                y + (float)(Math.Sin(i * deltaAngle) * radius)).SnapToPixels(),
                    null, color, i * deltaAngle + deltaAngle * 0.5f + MathHelper.PiOver2,
                    Vector2.Zero, new Vector2(segmentWidth, 1), SpriteEffects.None, 0);
            }
        }

        public static void DrawCircle(SpriteBatch spriteBatch, Circle circle, int slices, Color color)
        {
            DrawCircle(spriteBatch, circle.X, circle.Y, circle.Radius, slices, color);
        }
    }
}
