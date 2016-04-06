namespace KeyHub.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateToEF6 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DomainLicenses", "LicenseId", "dbo.Licenses");
            DropForeignKey("dbo.LicenseCustomerApps", "LicenseId", "dbo.Licenses");
            DropForeignKey("dbo.SKUs", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.PrivateKeys", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.VendorCredentials", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.Features", "VendorId", "dbo.Vendors");
            AddForeignKey("dbo.DomainLicenses", "LicenseId", "dbo.Licenses", "ObjectId");
            AddForeignKey("dbo.LicenseCustomerApps", "LicenseId", "dbo.Licenses", "ObjectId");
            AddForeignKey("dbo.SKUs", "VendorId", "dbo.Vendors", "ObjectId");
            AddForeignKey("dbo.PrivateKeys", "VendorId", "dbo.Vendors", "ObjectId");
            AddForeignKey("dbo.VendorCredentials", "VendorId", "dbo.Vendors", "ObjectId");
            AddForeignKey("dbo.Features", "VendorId", "dbo.Vendors", "ObjectId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Features", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.VendorCredentials", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.PrivateKeys", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.SKUs", "VendorId", "dbo.Vendors");
            DropForeignKey("dbo.LicenseCustomerApps", "LicenseId", "dbo.Licenses");
            DropForeignKey("dbo.DomainLicenses", "LicenseId", "dbo.Licenses");
            AddForeignKey("dbo.Features", "VendorId", "dbo.Vendors", "ObjectId", cascadeDelete: true);
            AddForeignKey("dbo.VendorCredentials", "VendorId", "dbo.Vendors", "ObjectId", cascadeDelete: true);
            AddForeignKey("dbo.PrivateKeys", "VendorId", "dbo.Vendors", "ObjectId", cascadeDelete: true);
            AddForeignKey("dbo.SKUs", "VendorId", "dbo.Vendors", "ObjectId", cascadeDelete: true);
            AddForeignKey("dbo.LicenseCustomerApps", "LicenseId", "dbo.Licenses", "ObjectId", cascadeDelete: true);
            AddForeignKey("dbo.DomainLicenses", "LicenseId", "dbo.Licenses", "ObjectId", cascadeDelete: true);
        }
    }
}
