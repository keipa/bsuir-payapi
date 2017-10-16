namespace PayAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cards",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CardId = c.String(),
                        CVV = c.Int(nullable: false),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Owner_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Owner_Id)
                .Index(t => t.Owner_Id);
            
            CreateTable(
                "dbo.Devices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeviceHash = c.String(),
                        Name = c.String(),
                        Owner_Id = c.Int(),
                        Card_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Owner_Id)
                .ForeignKey("dbo.Cards", t => t.Card_Id)
                .Index(t => t.Owner_Id)
                .Index(t => t.Card_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Requseted = c.DateTime(nullable: false),
                        Executed = c.DateTime(nullable: false),
                        amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        from_Id = c.Int(),
                        to_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Devices", t => t.from_Id)
                .ForeignKey("dbo.Users", t => t.to_Id)
                .Index(t => t.from_Id)
                .Index(t => t.to_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "to_Id", "dbo.Users");
            DropForeignKey("dbo.Transactions", "from_Id", "dbo.Devices");
            DropForeignKey("dbo.Devices", "Card_Id", "dbo.Cards");
            DropForeignKey("dbo.Cards", "Owner_Id", "dbo.Users");
            DropForeignKey("dbo.Devices", "Owner_Id", "dbo.Users");
            DropIndex("dbo.Transactions", new[] { "to_Id" });
            DropIndex("dbo.Transactions", new[] { "from_Id" });
            DropIndex("dbo.Devices", new[] { "Card_Id" });
            DropIndex("dbo.Devices", new[] { "Owner_Id" });
            DropIndex("dbo.Cards", new[] { "Owner_Id" });
            DropTable("dbo.Transactions");
            DropTable("dbo.Users");
            DropTable("dbo.Devices");
            DropTable("dbo.Cards");
        }
    }
}
