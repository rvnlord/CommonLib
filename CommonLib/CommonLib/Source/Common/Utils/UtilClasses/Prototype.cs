using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class Prototype<T>
    {
        public T DeepCopy() => (T)ObjectExtensions.DeepCopy(this);
        public T ShallowCopy() => (T)MemberwiseClone();
    }
}