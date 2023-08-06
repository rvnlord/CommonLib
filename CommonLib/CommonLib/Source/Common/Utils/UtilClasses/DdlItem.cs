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

    public class DdlItem<TValue, TItem>
    {
        public TValue Value { get; set; }
        public string Text { get; set; }
        public TItem Item { get; set; }

        public DdlItem(TValue value, string text)
        {
            Value = value;
            Text = text;
            Item = default;
        }

        public DdlItem(TValue value, string text, TItem item)
        {
            Value = value;
            Text = text;
            Item = item;
        }

        public override string ToString()
        {
            return $"{Value}, {Text}";
        }
    }
}
