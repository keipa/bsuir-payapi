namespace PayAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tokens : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.Guid(nullable: false),
                        Expired = c.DateTime(nullable: false),
                        Used = c.Boolean(nullable: false),
                        Owner_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Devices", t => t.Owner_Id)
                .Index(t => t.Owner_Id);
            
            AddColumn("dbo.Cards", "connected", c => c.Boolean(nullable: false));
            AddColumn("dbo.Transactions", "UseDevice_Id", c => c.Int());
            CreateIndex("dbo.Transactions", "UseDevice_Id");
            AddForeignKey("dbo.Transactions", "UseDevice_Id", "dbo.Devices", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "UseDevice_Id", "dbo.Devices");
            DropForeignKey("dbo.Tokens", "Owner_Id", "dbo.Devices");
            DropIndex("dbo.Transactions", new[] { "UseDevice_Id" });
            DropIndex("dbo.Tokens", new[] { "Owner_Id" });
            DropColumn("dbo.Transactions", "UseDevice_Id");
            DropColumn("dbo.Cards", "connected");
            DropTable("dbo.Tokens");
        }
    }
}
