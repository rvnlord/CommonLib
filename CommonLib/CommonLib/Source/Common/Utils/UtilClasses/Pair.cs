namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class Pair<T, T2>
    {
        public T First { get; set; }
        public T2 Second { get; set; }

        public Pair()
        {
        }

        public Pair(T first, T2 second)
        {
            First = first;
            Second = second;
        }
    };
}
