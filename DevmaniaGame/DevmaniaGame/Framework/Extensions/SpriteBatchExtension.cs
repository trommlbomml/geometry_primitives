using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DevmaniaGame.Framework.Extensions
{
    internal struct NinePatchField
    {
        public Rectangle Source;
        public Rectangle Target;

        public NinePatchField(Rectangle source, Rectangle target)
        {
            Source = source;
            Target = target;
        }
    }

    public class NinePatchParameter
    {
        private readonly int _leftBorder;
        private readonly int _rightBorder;
        private readonly int _topBorder;
        private readonly int _bottomBorder;
        private readonly int _commulativeFixedWidth;
        private readonly int _commulativeFixedHeight;

        public NinePatchParameter()
        {
            PatchFields = new List<NinePatchField>();
        }

        public NinePatchParameter(int unitBorder)
            : this()
        {
            _leftBorder = unitBorder;
            _rightBorder = unitBorder;
            _topBorder = unitBorder;
            _bottomBorder = unitBorder;
            _commulativeFixedWidth = _leftBorder + _rightBorder;
            _commulativeFixedHeight = _topBorder + _bottomBorder;
        }

        public NinePatchParameter(int horizontalBorder, int verticalBorder)
            : this()
        {
            _leftBorder = horizontalBorder;
            _rightBorder = horizontalBorder;
            _topBorder = verticalBorder;
            _bottomBorder = verticalBorder;
            _commulativeFixedWidth = _leftBorder + _rightBorder;
            _commulativeFixedHeight = _topBorder + _bottomBorder;
        }

        public NinePatchParameter(int leftBorder, int rightBorder, int topBorder, int bottomBorder)
            : this()
        {
            _leftBorder = leftBorder;
            _rightBorder = rightBorder;
            _topBorder = topBorder;
            _bottomBorder = bottomBorder;
            _commulativeFixedWidth = _leftBorder + _rightBorder;
            _commulativeFixedHeight = _topBorder + _bottomBorder;
        }

        public void Calculate(Rectangle sourceRectangle, Rectangle targetRectangle)
        {
            PatchFields.Clear();

            int innerTargetWidth = targetRectangle.Width - _commulativeFixedWidth;
            int innerTargetHeight = targetRectangle.Height - _commulativeFixedHeight;

            int innerSourceWidth = sourceRectangle.Width - _commulativeFixedWidth;
            int innerSourceHeight = sourceRectangle.Height - _commulativeFixedHeight;

            Rectangle currentSourceRectangle = new Rectangle(sourceRectangle.X, sourceRectangle.Y, _leftBorder, _topBorder);
            Rectangle currentTargetRectangle = new Rectangle(targetRectangle.X, targetRectangle.Y, _leftBorder, _topBorder);

            if (currentTargetRectangle.Width > 0 && currentTargetRectangle.Height > 0)
                PatchFields.Add(new NinePatchField(currentSourceRectangle, currentTargetRectangle));

            currentSourceRectangle.X += _leftBorder;
            currentTargetRectangle.X += _leftBorder;
            currentSourceRectangle.Width = innerSourceWidth;
            currentTargetRectangle.Width = innerTargetWidth;
            if (currentTargetRectangle.Width > 0 && currentTargetRectangle.Height > 0)
                PatchFields.Add(new NinePatchField(currentSourceRectangle, currentTargetRectangle));

            currentSourceRectangle.X += innerSourceWidth;
            currentTargetRectangle.X += innerTargetWidth;
            currentSourceRectangle.Width = _rightBorder;
            currentTargetRectangle.Width = _rightBorder;
            if (currentTargetRectangle.Width > 0 && currentTargetRectangle.Height > 0)
                PatchFields.Add(new NinePatchField(currentSourceRectangle, currentTargetRectangle));

            currentSourceRectangle.X = sourceRectangle.X;
            currentSourceRectangle.Y += _topBorder;
            currentSourceRectangle.Width = _leftBorder;
            currentSourceRectangle.Height = innerSourceHeight;
            currentTargetRectangle.X = targetRectangle.X;
            currentTargetRectangle.Y += _topBorder;
            currentTargetRectangle.Width = _leftBorder;
            currentTargetRectangle.Height = innerTargetHeight;
            if (currentTargetRectangle.Width > 0 && currentTargetRectangle.Height > 0)
                PatchFields.Add(new NinePatchField(currentSourceRectangle, currentTargetRectangle));

            currentSourceRectangle.X += _leftBorder;
            currentSourceRectangle.Width = innerSourceWidth;
            currentTargetRectangle.X += _leftBorder;
            currentTargetRectangle.Width = innerTargetWidth;
            if (currentTargetRectangle.Width > 0 && currentTargetRectangle.Height > 0)
                PatchFields.Add(new NinePatchField(currentSourceRectangle, currentTargetRectangle));

            currentSourceRectangle.X += innerSourceWidth;
            currentSourceRectangle.Width = _rightBorder;
            currentTargetRectangle.X += innerTargetWidth;
            currentTargetRectangle.Width = _rightBorder;
            if (currentTargetRectangle.Width > 0 && currentTargetRectangle.Height > 0)
                PatchFields.Add(new NinePatchField(currentSourceRectangle, currentTargetRectangle));

            currentSourceRectangle.X = sourceRectangle.X;
            currentSourceRectangle.Y += innerSourceHeight;
            currentSourceRectangle.Width = _leftBorder;
            currentSourceRectangle.Height = _bottomBorder;
            currentTargetRectangle.X = targetRectangle.X;
            currentTargetRectangle.Y += innerTargetHeight;
            currentTargetRectangle.Width = _leftBorder;
            currentTargetRectangle.Height = _bottomBorder;
            if (currentTargetRectangle.Width > 0 && currentTargetRectangle.Height > 0)
                PatchFields.Add(new NinePatchField(currentSourceRectangle, currentTargetRectangle));

            currentSourceRectangle.X += _leftBorder;
            currentSourceRectangle.Width = innerSourceWidth;
            currentTargetRectangle.X += _leftBorder;
            currentTargetRectangle.Width = innerTargetWidth;
            if (currentTargetRectangle.Width > 0 && currentTargetRectangle.Height > 0)
                PatchFields.Add(new NinePatchField(currentSourceRectangle, currentTargetRectangle));

            currentSourceRectangle.X += innerSourceWidth;
            currentSourceRectangle.Width = _rightBorder;
            currentTargetRectangle.X += innerTargetWidth;
            currentTargetRectangle.Width = _rightBorder;
            if (currentTargetRectangle.Width > 0 && currentTargetRectangle.Height > 0)
                PatchFields.Add(new NinePatchField(currentSourceRectangle, currentTargetRectangle));
        }

        internal List<NinePatchField> PatchFields { get; private set; }
        public int CommulativeFixedWidth { get { return _commulativeFixedWidth; } }
        public int CommulativeFixedHeight { get { return _commulativeFixedHeight; } }
    }

    public static class SpriteBatchExtension
    {
        public static void DrawNinePatch(this SpriteBatch spritebatch, Texture2D texture, Rectangle sourceRectangle, Rectangle targetRectangle, Color color, NinePatchParameter ninePatchParameter)
        {
            ninePatchParameter.Calculate(sourceRectangle, targetRectangle);
            ninePatchParameter.PatchFields.ForEach(n => spritebatch.Draw(texture, n.Target, n.Source, color));
        }

        public static void DrawNinePatch(this SpriteBatch spritebatch, Texture2D texture, Color color, NinePatchParameter ninePatchParameter)
        {
            ninePatchParameter.PatchFields.ForEach(n => spritebatch.Draw(texture, n.Target, n.Source, color));
        }
    }
}
