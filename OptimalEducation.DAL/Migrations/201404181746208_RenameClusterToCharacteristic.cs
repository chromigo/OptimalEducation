namespace OptimalEducation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameClusterToCharacteristic : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Clusters", newName: "Characteristics");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Characteristics", newName: "Clusters");
        }
    }
}
