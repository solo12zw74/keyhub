using KeyHub.Core.Data;
using KeyHub.Model.Definition.Membership;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace KeyHub.Data.Configuration.Membership
{
    public class ExternalLoginConfiguration : EntityTypeConfiguration<ExternalLogin>, IEntityConfiguration
    {
        public ExternalLoginConfiguration()
        {
            HasKey(l => new { l.LoginProvider, l.ProviderKey })
                .ToTable("webpages_OAuthMembership");

            Property(l => l.LoginProvider).HasColumnName("Provider");
            Property(l => l.ProviderKey).HasColumnName("ProviderUserId");

            HasRequired(l => l.User).WithMany(u => u.ExternalLogins);
        }
        public void AddConfiguration(ConfigurationRegistrar registrar)
        {
            registrar.Add(this);
        }
    }
}
