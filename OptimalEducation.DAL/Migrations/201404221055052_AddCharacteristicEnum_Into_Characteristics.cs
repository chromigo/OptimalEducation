namespace OptimalEducation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCharacteristicEnum_Into_Characteristics : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Characteristics", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Characteristics", "Type");
        }
    }
}
