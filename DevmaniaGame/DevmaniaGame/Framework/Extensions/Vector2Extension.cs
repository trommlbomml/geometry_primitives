using System;
using Microsoft.Xna.Framework;

namespace DevmaniaGame.Framework.Extensions
{
    static class Vector2Extension
    {
        public static Vector2 SnapToPixels(this Vector2 v)
        {
            return new Vector2((float)Math.Round(v.X), (float)Math.Round(v.Y));
        }
    }
}
