using System;
using static CommonLib.Source.LibConfig;

namespace CommonLib.Source.Common.Utils.UtilClasses.Geometry
{
    public struct Point : IEquatable<Point>
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static readonly Point Origin = new Point(0, 0);

        public Point(long x, long y)
        {
            X = x;
            Y = y;
        }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double Distance(Point that)
        {
            var dX = X - that.X;
            var dY = Y - that.Y;
            return Math.Sqrt(dX * dX + dY * dY);
        }

        public double DistanceSquared(Point that)
        {
            var dX = X - that.X;
            var dY = Y - that.Y;
            return dX * dX + dY * dY;
        }

        public double DistanceXY(Point that)
        {
            var dX = Math.Abs(X - that.X);
            var dY = Math.Abs(Y - that.Y);
            return dX + dY;
        }

        public override bool Equals(object o)
        {
            if (o == null || GetType() != o.GetType())
                return false;
            var that = (Point)o;
            return Equals(that);
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return !(p1 == p2);
        }

        public override int GetHashCode()
        {
            return (X.GetHashCode() * 23) ^ (Y.GetHashCode() * 17);
        }

        public bool IsAbove(Point that)
        {
            return X > that.Y;
        }

        public bool IsBelow(Point that)
        {
            return X < that.Y;
        }

        public bool IsLeftOf(Line line)
        {
            return !line.Contains(this) && !IsRightOf(line);
        }

        public bool IsLeftOf(Point that)
        {
            return X < that.X;
        }

        public bool IsRightOf(Line line)
        {
            if (line.Contains(this))
                return false;
            if (line.IsHorizontal())
                return Y < line.YIntercept();
            if (line.IsVertical())
            {
                return X > line.XIntercept();
            }

            var linePointNullable = line.Intersection(Line.Vertical(0));
            if (linePointNullable == null)
                throw new ArgumentException("Wartość linePoint nie może wynosić null");

            var linePoint = (Point)linePointNullable;
            var temp = new Line(this, linePoint);

            if (Y < linePoint.Y)
            {
                if (line.Slope < 0) // a
                    return temp.Slope < 0 && temp.Slope > line.Slope;

                return temp.Slope < 0 || temp.Slope > line.Slope; // b
            }
            if (Y > linePoint.Y)
            {
                if (line.Slope < 0) // c
                    return temp.Slope >= 0 || temp.Slope < line.Slope;

                return temp.Slope >= 0 && temp.Slope < line.Slope; // d
            }
            return IsRightOf(linePoint); // 'this' ma taką samą współrzędną y jak 'linePoint'
        }

        public bool IsRightOf(LineSegment segment)
        {
            return PointOrientationTest(this, segment) < 0;
        }

        public bool IsLeftOf(LineSegment segment)
        {
            return PointOrientationTest(this, segment) > 0;
        }

        public static double PointOrientationTest(Point point, LineSegment segment)
        {
            var p0 = point;
            var p1 = segment.StartPoint;
            var p2 = segment.EndPoint;

            return (p1.X - p0.X) * (p2.Y - p0.Y) - (p1.Y - p0.Y) * (p2.X - p0.X);
        }

        public bool IsRightOf(Point that)
        {
            return X > that.X;
        }

        public static Point Midpoint(Point a, Point b)
        {
            return new Point(
                (a.X + b.X) / 2,
                (a.Y + b.Y) / 2
            );
        }

        public static double Slope(Point from, Point to)
        {
            return (to.Y - from.Y) / (to.X - from.X);
        }

        public Point Translate(long dx, long dy)
        {
            return new Point(X + dx, Y + dy);
        }

        public Point Translate(int dx, int dy)
        {
            return new Point(X + dx, Y + dy);
        }

        public Point Translate(double dx, double dy)
        {
            return new Point(X + dx, Y + dy);
        }

        public override string ToString()
        {
            return $"({X:0.00}, {Y:0.00})";
        }

        public static double Angle(Point p1, Point p2, Point refp)
        {
            return Math.Atan2(p1.Y - refp.Y, p1.X - refp.X) - Math.Atan2(p2.Y - refp.Y, p2.X - refp.X) * 180 / Math.PI;
        }

        //public static implicit operator Point(System.Windows.Point value)
        //{
        //    return ToPoint(value);
        //}

        //public static implicit operator System.Windows.Point(Point value)
        //{
        //    return new System.Windows.Point(value.X, value.Y);
        //}

        //public static Point ToPoint(System.Windows.Point value)
        //{
        //    return new Point(value.X, value.Y);
        //}

        public bool Equals(Point that)
        {
            return Math.Abs(X - that.X) < TOLERANCE && Math.Abs(Y - that.Y) < TOLERANCE;
        }
    }
}
