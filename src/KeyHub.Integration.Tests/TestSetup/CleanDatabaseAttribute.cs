using KeyHub.Data;
using KeyHub.Data.Migrations;
using System.Data.Entity;
using System.Reflection;
using Xunit;

namespace KeyHub.Integration.Tests.TestSetup
{
    public class CleanDatabaseAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            Database.Delete("DataContext");

            var configuration = new MigrationConfiguration();
            var migrator = new DbSeederMigrator<DataContext>(configuration);
            migrator.MigrateToLatestVersion();
        }
    }
}
