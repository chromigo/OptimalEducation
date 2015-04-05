using System.Data.Entity.Migrations;

namespace OptimalEducation.DAL.Migrations
{
    public partial class AddCharacteristicEnumIntoCharacteristics : DbMigration
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
