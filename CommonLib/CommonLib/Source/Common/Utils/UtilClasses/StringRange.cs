namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class StringRange : Pair<string, string>
    {
        public string From => First;
        public string To => Second;

        public StringRange(string from, string to) : base(from, to) { }
    }
}
