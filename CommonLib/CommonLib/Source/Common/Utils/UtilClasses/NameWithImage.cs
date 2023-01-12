using System;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class NameWithImage : IEquatable<NameWithImage>
    {
        public string Name { get; set; }
        public FileData Image { get; set; }

        public bool Equals(NameWithImage other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((NameWithImage)obj);
        }

        public override int GetHashCode() => Name is not null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(Name) : 0;
        public static bool operator ==(NameWithImage left, NameWithImage right) => Equals(left, right);
        public static bool operator !=(NameWithImage left, NameWithImage right) => !Equals(left, right);

        public override string ToString() => Name;
    }
}
