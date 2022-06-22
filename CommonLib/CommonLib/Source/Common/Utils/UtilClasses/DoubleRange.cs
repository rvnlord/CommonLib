namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class DoubleRange : Pair<double, double>
    {
        public double From => First;
        public double To => Second;

        public DoubleRange(double from, double to) : base(from, to) { }
    }
}
