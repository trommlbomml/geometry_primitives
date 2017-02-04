
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Framework.Drawing
{
    interface IDepthSortable
    {
        int Depth { get; }
        void Draw(SpriteBatch spriteBatch);
    }
}
