
using Microsoft.Xna.Framework;

namespace DevmaniaGame.Framework.Collision
{
    struct Circle
    {
        public int X;
        public int Y;
        public int Radius;

        public Circle(int x,int y,int radius)
        {
            X = x;
            Y = y;
            Radius = radius;
        }

        private static int LengthSq(int x1, int x2, int y1, int y2)
        {
            return (x2 - x1)*(x2 - x1) + (y2 - y1)*(y2 - y1);
        }

        public bool Intersects(Circle circle)
        {
            return LengthSq(X, circle.X, Y, circle.Y) < (circle.Radius + Radius) * (circle.Radius + Radius);
        }

        public bool Intersects(Rectangle rectangle)
        {
            int testX = X;
            int testY = Y;

            if (testX < rectangle.Left) testX = rectangle.Left;
            if (testX > rectangle.Right) testX = rectangle.Right;
            if (testY < rectangle.Top) testY = rectangle.Top;
            if (testY > rectangle.Bottom) testY = rectangle.Bottom;

            return LengthSq(testX, X, testY, Y) < Radius*Radius;
        }

    }
}
