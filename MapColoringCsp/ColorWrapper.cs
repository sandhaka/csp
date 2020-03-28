using System.Drawing;
using Csp.Csp;

namespace MapColoringCsp
{
    public class ColorWrapper : CspValue
    {
        public Color Color { get; }

        private ColorWrapper(Color color)
        {
            Color = color;
        }

        public static implicit operator Color(ColorWrapper colorWrapper) => colorWrapper.Color;
        public static implicit operator ColorWrapper(Color color) => new ColorWrapper(color);

        protected override int TypeConcernedGetHashCode()
        {
            return Color.GetHashCode();
        }

        protected override bool TypeConcernedEquals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var b2 = (ColorWrapper)obj;
            return Color == b2.Color;
        }
    }
}