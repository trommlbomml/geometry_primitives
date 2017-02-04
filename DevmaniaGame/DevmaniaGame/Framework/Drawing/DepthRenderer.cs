
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Framework.Drawing
{
    static class DepthRenderer
    {
        private static readonly List<IDepthSortable> DepthSortables = new List<IDepthSortable>(); 
        
        public static void Clear()
        {
            DepthSortables.Clear();
        }


        public static void Register(IDepthSortable depthSortable)
        {
            DepthSortables.Add(depthSortable);
        }

        public static void UnRegister(IDepthSortable depthSortable)
        {
            DepthSortables.Remove(depthSortable);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            DepthSortables.Sort(CompareDepth);

            foreach (IDepthSortable depthSortable in DepthSortables)
            {
                depthSortable.Draw(spriteBatch);
            }
        }

        private static int CompareDepth(IDepthSortable d1, IDepthSortable d2)
        {
            if (d1.Depth > d2.Depth)
                return 1;
            if (d1.Depth < d2.Depth)
                return -1;
            return 0;
        }
    }
}
