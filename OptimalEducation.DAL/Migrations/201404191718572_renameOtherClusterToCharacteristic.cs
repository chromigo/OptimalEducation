namespace OptimalEducation.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renameOtherClusterToCharacteristic : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Weights", name: "ClusterId", newName: "CharacterisicId");
            RenameIndex(table: "dbo.Weights", name: "IX_ClusterId", newName: "IX_CharacterisicId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Weights", name: "IX_CharacterisicId", newName: "IX_ClusterId");
            RenameColumn(table: "dbo.Weights", name: "CharacterisicId", newName: "ClusterId");
        }
    }
}
