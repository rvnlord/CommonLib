using System;
using System.Collections.Generic;
using CommonLib.Source.Common.Extensions.Collections;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class DiscordGuildUserVM : IEquatable<DiscordGuildUserVM>
    {
        public string UserName { get; set; }
        public string GuildNickname { get; set; }
        public string DisplayName => GuildNickname ?? UserName;
        public ushort DiscriminatorValue { get; set; }
        public string Discriminator => DiscriminatorValue.ToString("D4");
        public ExtendedTime CreatedAt { get; set; }
        public ExtendedTime JoinedCurrentGuildAt { get; set; }
        public List<string> Roles { get; set; } = new();
        public ulong GuildId { get; set; }

        public string ToDetailedString() => $"{UserName}#{Discriminator} ({DisplayName}) [{Roles?.JoinAsString(", ")}], Created: {CreatedAt.ToString("dd-MM-yyyy")}, Joined: {JoinedCurrentGuildAt.ToString("dd-MM-yyyy")}";
        public override string ToString() => $"{UserName}#{Discriminator} ({DisplayName}) [{Roles?.JoinAsString(", ")}]";

        public bool Equals(DiscordGuildUserVM other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(UserName, other.UserName, StringComparison.InvariantCultureIgnoreCase) && DiscriminatorValue == other.DiscriminatorValue;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DiscordGuildUserVM)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(UserName, StringComparer.InvariantCultureIgnoreCase);
            hashCode.Add(DiscriminatorValue);
            return hashCode.ToHashCode();
        }

        public static bool operator ==(DiscordGuildUserVM left, DiscordGuildUserVM right) => Equals(left, right);
        public static bool operator !=(DiscordGuildUserVM left, DiscordGuildUserVM right) => !Equals(left, right);
    }
}
