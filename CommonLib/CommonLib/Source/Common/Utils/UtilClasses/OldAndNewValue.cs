namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public struct OldAndNewValue<T>
    {
        public T OldValue { get; }
        public T NewValue { get; }

        public OldAndNewValue(T oldValue, T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
