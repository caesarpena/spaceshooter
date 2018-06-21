using UnityEngine;

namespace Stellar_Sprites
{
    public struct SS_Point
    {
        public int x;
        public int y;

        public SS_Point(int aX, int aY) { x = aX; y = aY; }

        public static SS_Point operator +(SS_Point p1, SS_Point p2)
        {
            return new SS_Point(p1.x + p2.x, p1.y + p2.y);
        }

        public static SS_Point operator -(SS_Point p1, SS_Point p2)
        {
            return new SS_Point(p1.x - p2.x, p1.y - p2.y);
        }

        public static SS_Point operator /(SS_Point p1, SS_Point p2)
        {
            return new SS_Point(p1.x / p2.x, p1.y / p2.y);
        }

        public static SS_Point operator /(SS_Point p1, int s)
        {
            return new SS_Point(p1.x / s, p1.y / s);
        }

        public static int Distance(SS_Point p1, SS_Point p2)
        {
            return (int)Mathf.Sqrt(Mathf.Pow((p2.x - p1.x), 2) + Mathf.Pow((p2.y - p1.y), 2));
        }

        public static SS_Point Scale(SS_Point p, SS_Point center, float scaleX, float scaleY)
        {
            SS_Point scaledPoint = new SS_Point();

            int cX = ((center.x + p.x) / 2);
            int cY = ((center.y + p.y) / 2);

            scaledPoint.x = cX + (int)((p.x - cX) * scaleX);
            scaledPoint.y = cY + (int)((p.y - cY) * scaleY);

            return scaledPoint;
        }

        public static SS_Point Rotate(SS_Point p, int cx, int cy, float angle)
        {
            float s = Mathf.Sin(angle * Mathf.Deg2Rad);
            float c = Mathf.Cos(angle * Mathf.Deg2Rad);

            // translate point back to origin:
            p.x -= cx;
            p.y -= cy;

            // rotate point
            float rotatedX = p.x * c - p.y * s;
            float rotatedY = p.x * s + p.y * c;

            // translate point back:
            p.x = (int)rotatedX + cx;
            p.y = (int)rotatedY + cy;

            return p;
        }

        public Vector2 ToVector2
        {
            get { return new Vector2(x, y); }
        }

        public override string ToString()
        {
            return x.ToString() + ", " + y.ToString();
        }
    }
}
