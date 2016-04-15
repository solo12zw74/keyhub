using KeyHub.Core.Data;
using KeyHub.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace KeyHub.Data.DataConfiguration
{
    /// <summary>
    /// Configures the <see cref="KeyHub.Model.User"/> table
    /// </summary>
    public class UserConfiguration : EntityTypeConfiguration<User>, IEntityConfiguration
    {
        public UserConfiguration()
        {
            ToTable("Users");
            HasKey(p => p.Id);
            Property(u => u.Id)
                .HasColumnName("UserId")
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity); ;
            Property(p => p.Email).HasMaxLength(256);
            HasMany(x => x.Rights)
                .WithRequired(x => x.User)
                .WillCascadeOnDelete(true);
            Ignore(p => p.UserName);
            HasMany(u => u.Roles).WithMany(r => r.Users).Map((config) =>
            {
                config
                    .ToTable("webpages_UsersInRoles")
                    .MapLeftKey("UserId")
                    .MapRightKey("RoleId");
            });
        }

        public void AddConfiguration(System.Data.Entity.ModelConfiguration.Configuration.ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}