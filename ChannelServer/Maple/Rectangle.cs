using System;

namespace Loki.Maple
{
    public class Rectangle
    {
        public Point LT;
        public Point RB;

        public Rectangle(Point lt, Point rb)
        {
            this.LT = lt;
            this.RB = rb;
        }
    }
}
