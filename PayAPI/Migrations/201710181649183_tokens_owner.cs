namespace PayAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tokens_owner : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Tokens", name: "Owner_Id", newName: "RelatedDevice_Id");
            RenameIndex(table: "dbo.Tokens", name: "IX_Owner_Id", newName: "IX_RelatedDevice_Id");
            AddColumn("dbo.Tokens", "RelatedCard_Id", c => c.Int());
            CreateIndex("dbo.Tokens", "RelatedCard_Id");
            AddForeignKey("dbo.Tokens", "RelatedCard_Id", "dbo.Cards", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tokens", "RelatedCard_Id", "dbo.Cards");
            DropIndex("dbo.Tokens", new[] { "RelatedCard_Id" });
            DropColumn("dbo.Tokens", "RelatedCard_Id");
            RenameIndex(table: "dbo.Tokens", name: "IX_RelatedDevice_Id", newName: "IX_Owner_Id");
            RenameColumn(table: "dbo.Tokens", name: "RelatedDevice_Id", newName: "Owner_Id");
        }
    }
}
