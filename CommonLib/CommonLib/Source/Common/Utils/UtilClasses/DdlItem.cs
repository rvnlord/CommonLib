namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class DdlItem
    {
        public int? Index { get; set; }
        public string Text { get; set; }

        public DdlItem(int? index, string text)
        {
            Index = index;
            Text = text;
        }

        public override string ToString()
        {
            return $"{Index}, {Text}";
        }
    }

    public class DdlItem<TValue>
    {
        public TValue Value { get; set; }
        public string Text { get; set; }

        public DdlItem(TValue value, string text)
        {
            Value = value;
            Text = text;
        }

        public override string ToString()
        {
            return $"{Value}, {Text}";
        }
    }
}
