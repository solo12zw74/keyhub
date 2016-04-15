using KeyHub.Core.Data;
using KeyHub.Model.Definition.Membership;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace KeyHub.Data.Configuration.Membership
{
    public class PasswordLoginConfiguration : EntityTypeConfiguration<PasswordLogin>, IEntityConfiguration
    {
        public PasswordLoginConfiguration()
        {
            HasKey(p => p.UserId)
                .ToTable("webpages_Membership");
            Property(u => u.Created).HasColumnName("CreateDate");
            Property(u => u.Confirmed).HasColumnName("IsConfirmed");
            Property(u => u.LastLoginFailed).HasColumnName("LastPasswordFailureDate");
            Property(u => u.LoginFailureCounter).HasColumnName("PasswordFailuresSinceLastSuccess");
            Property(u => u.PasswordHash).HasColumnName("Password");
            Property(u => u.PasswordChanged).HasColumnName("PasswordChangedDate");
            Property(u => u.VerificationToken).HasColumnName("PasswordVerificationToken");
            Property(u => u.VerificationTokenExpires).HasColumnName("PasswordVerificationTokenExpirationDate");

            HasRequired(l => l.User).WithOptional(u => u.PasswordLogin);
        }
        public void AddConfiguration(ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}
