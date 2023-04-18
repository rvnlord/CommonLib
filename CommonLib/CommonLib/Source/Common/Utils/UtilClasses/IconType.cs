using System;
using System.Configuration;
using System.Linq;
using CommonLib.Source.Common.Converters;
using CommonLib.Source.Common.Extensions;

namespace CommonLib.Source.Common.Utils.UtilClasses
{
    public class IconType : IEquatable<IconType>
    {
        public bool Equals(IconType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return RegularIcon == other.RegularIcon && LightIcon == other.LightIcon && SolidIcon == other.SolidIcon && BrandsIcon == other.BrandsIcon && DuotoneIcon == other.DuotoneIcon && ThinIcon == other.ThinIcon && SharpSolidIcon == other.SharpSolidIcon;
        }
        
        public RegularIconType? RegularIcon { get; private set; } // private set required for reflection
        public LightIconType? LightIcon { get; private set; }
        public SolidIconType? SolidIcon { get; private set; }
        public BrandsIconType? BrandsIcon { get; private set; }
        public DuotoneIconType? DuotoneIcon { get; private set; }
        public ThinIconType? ThinIcon { get; private set; }
        public SharpSolidIconType? SharpSolidIcon { get; private set; }

        public object[] IconTypes => new object[] { RegularIcon, LightIcon, SolidIcon, BrandsIcon, DuotoneIcon, ThinIcon, SharpSolidIcon };
        public string SetName => IconTypes.SingleOrDefault(i => i is not null)?.GetType().Name.BeforeOrNull("IconType");
        public string IconName => ToString();

        public IconType() { }

        public IconType(string setName, string iconName)
        {
            var setTypePropertyName = setName.KebabCaseToPascalCase() + "Icon";
            this.SetPropertyValue(setTypePropertyName, iconName.KebabCaseToPascalCase().ToEnum(Type.GetType(GetType().Namespace + '.' + setTypePropertyName + "Type")));
        }

        public IconType(RegularIconType? regularIcon)
        {
            RegularIcon = regularIcon;
        }

        public IconType(LightIconType? lightIcon)
        {
            LightIcon = lightIcon;
        }

        public IconType(SolidIconType? solidIcon)
        {
            SolidIcon = solidIcon;
        }

        public IconType(BrandsIconType? brandsIcon)
        {
            BrandsIcon = brandsIcon;
        }

        public IconType(DuotoneIconType? duotoneIcon)
        {
            DuotoneIcon = duotoneIcon;
        }

        public IconType(ThinIconType? thinIcon)
        {
            ThinIcon = thinIcon;
        }

        public IconType(SharpSolidIconType? sharpSolidIcon)
        {
            SharpSolidIcon = sharpSolidIcon;
        }

        public static IconType From(RegularIconType regularIcon) => new IconType(regularIcon);
        public static IconType From(LightIconType lightIcon) => new IconType(lightIcon);
        public static IconType From(SolidIconType solidIcon) => new IconType(solidIcon);
        public static IconType From(BrandsIconType brandsIcon) => new IconType(brandsIcon);
        public static IconType From(DuotoneIconType duotoneIcon) => new IconType(duotoneIcon);
        public static IconType From(ThinIconType thinIcon) => new IconType(thinIcon);
        public static IconType From(SharpSolidIconType sharpSolidIcon) => new IconType(sharpSolidIcon);

        public override string ToString()
        {
            return RegularIcon?.EnumToString() ?? LightIcon?.EnumToString() ?? SolidIcon?.EnumToString() ?? DuotoneIcon?.EnumToString() ?? BrandsIcon?.EnumToString() ?? ThinIcon?.EnumToString() ?? SharpSolidIcon?.EnumToString();
        }

        public override bool Equals(object other)
        {
            if (other is not IconType that)
                return false;
            return RegularIcon == that.RegularIcon && LightIcon == that.LightIcon && SolidIcon == that.SolidIcon && BrandsIcon == that.BrandsIcon && DuotoneIcon == that.DuotoneIcon && ThinIcon == that.ThinIcon && SharpSolidIcon == that.SharpSolidIcon;
        }

        public override int GetHashCode() => HashCode.Combine(RegularIcon, LightIcon, SolidIcon, BrandsIcon, DuotoneIcon);

        public static bool operator ==(IconType left, IconType right) => Equals(left, right);
        public static bool operator !=(IconType left, IconType right) => !Equals(left, right);
    }
}
