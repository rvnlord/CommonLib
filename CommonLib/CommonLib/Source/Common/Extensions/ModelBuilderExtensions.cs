using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CommonLib.Source.Common.Extensions
{
    public static class ModelBuilderExtensions
    {
        private class PersonalDataConverter : ValueConverter<string, string>
        {
            public PersonalDataConverter(IPersonalDataProtector protector) : base(s => protector.Protect(s), s => protector.Unprotect(s)) { }
        }
        
        //public static void RenameIdentityTables(this ModelBuilder mb, StoreOptions storeOptions)
        //{
        //    if (mb == null) 
        //        throw new ArgumentNullException(nameof(mb));
            
        //    var maxKeyLength = storeOptions?.MaxLengthForKeys ?? 0;
        //    var encryptPersonalData = storeOptions?.ProtectPersonalData ?? false;
        //    PersonalDataConverter converter = null;

        //    mb.Entity<IdentityUser<Guid>>(b =>
        //    {
        //        b.HasKey(u => u.Id);
        //        b.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
        //        b.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");
        //        b.ToTable("Users");
        //        b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

        //        b.Property(u => u.UserName).HasMaxLength(256);
        //        b.Property(u => u.NormalizedUserName).HasMaxLength(256);
        //        b.Property(u => u.Email).HasMaxLength(256);
        //        b.Property(u => u.NormalizedEmail).HasMaxLength(256);

        //        if (encryptPersonalData)
        //        {
        //            converter = new PersonalDataConverter((IPersonalDataProtector) Activator.CreateInstance(typeof(ILookupProtector).GetImplementingTypes().Single(), Activator.CreateInstance(typeof(ILookupProtectorKeyRing).GetImplementingTypes().Single())));
        //            var personalDataProps = typeof(IdentityUser<Guid>).GetProperties().Where(
        //                prop => Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute)));
        //            foreach (var p in personalDataProps)
        //            {
        //                if (p.PropertyType != typeof(string))
        //                    throw new InvalidOperationException("Only strings can be protected");
        //                b.Property(typeof(string), p.Name).HasConversion(converter);
        //            }
        //        }

        //        b.HasMany<IdentityUserClaim<Guid>>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();
        //        b.HasMany<IdentityUserLogin<Guid>>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();
        //        b.HasMany<IdentityUserToken<Guid>>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();
        //        b.HasMany<IdentityUserRole<Guid>>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
        //    });

        //    mb.Entity<IdentityRole<Guid>>(b =>
        //    {
        //        b.HasKey(r => r.Id);
        //        b.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();
        //        b.ToTable("Roles");
        //        b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();
        //        b.Property(u => u.Name).HasMaxLength(256);
        //        b.Property(u => u.NormalizedName).HasMaxLength(256);
        //        b.HasMany<IdentityUserRole<Guid>>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();
        //        b.HasMany<IdentityRoleClaim<Guid>>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
        //    });

        //    mb.Entity<IdentityRoleClaim<Guid>>(b =>
        //    {
        //        b.HasKey(rc => rc.Id);
        //        b.ToTable("RolesClaims");
        //    });

        //    mb.Entity<IdentityUserRole<Guid>>(b =>
        //    {
        //        b.HasKey(r => new { r.UserId, r.RoleId });
        //        b.ToTable("UserRoles");
        //    });
            
        //    mb.Entity<IdentityUserLogin<Guid>>(b =>
        //    {
        //        b.HasKey(e => new { e.LoginProvider, e.ProviderKey });
        //        if (maxKeyLength > 0)
        //        {
        //            b.Property(l => l.LoginProvider).HasMaxLength(maxKeyLength);
        //            b.Property(l => l.ProviderKey).HasMaxLength(maxKeyLength);
        //        }
        //        b.ToTable("UsersLogins");

        //    });

        //    mb.Entity<IdentityUserToken<Guid>>(b =>
        //    {
        //        b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

        //        if (maxKeyLength > 0)
        //        {
        //            b.Property(t => t.LoginProvider).HasMaxLength(maxKeyLength);
        //            b.Property(t => t.Name).HasMaxLength(maxKeyLength);
        //        }

        //        if (encryptPersonalData)
        //        {
        //            var tokenProps = typeof(IdentityUserToken<Guid>).GetProperties().Where(prop => Attribute.IsDefined(prop, typeof(ProtectedPersonalDataAttribute)));
        //            foreach (var p in tokenProps)
        //            {
        //                if (p.PropertyType != typeof(string))
        //                    throw new InvalidOperationException("Only strings can be protected");
        //                b.Property(typeof(string), p.Name).HasConversion(converter);
        //            }
        //        }
        //        b.ToTable("UsersTokens");
        //    });

        //    mb.Entity<IdentityUserClaim<Guid>>(b =>
        //    {
        //        b.HasKey(e => new { e.UserId, e.ClaimType, e.ClaimValue });
        //        b.ToTable("UsersClaims");
        //    });
        //}
    }
}
