using System.Drawing;

namespace MapColoringCsp
{
    public class ColorWrapper
    {
        public Color Color { get; }

        private ColorWrapper(Color color)
        {
            Color = color;
        }

        public static implicit operator Color(ColorWrapper colorWrapper) => colorWrapper.Color;
        public static implicit operator ColorWrapper(Color color) => new ColorWrapper(color);

        public static bool operator ==(ColorWrapper cv1, ColorWrapper cv2)
        {
            if (ReferenceEquals(cv1, cv2))
            {
                return true;
            }

            if (ReferenceEquals(cv1, null))
            {
                return false;
            }

            if (ReferenceEquals(cv2, null))
            {
                return false;
            }

            return cv1.Equals(cv2);
        }

        public static bool operator !=(ColorWrapper cv, ColorWrapper cv2)
        {
            return !(cv == cv2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var b2 = (ColorWrapper)obj;
            return Color == b2.Color;
        }

        public override int GetHashCode()
        {
            return Color.GetHashCode();
        }
    }
}