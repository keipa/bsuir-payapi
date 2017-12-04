namespace PayAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class complete_activation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Activations",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        isActive = c.Boolean(nullable: false),
                        Card_Id = c.Int(),
                        Device_Id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Cards", t => t.Card_Id)
                .ForeignKey("dbo.Devices", t => t.Device_Id)
                .Index(t => t.Card_Id)
                .Index(t => t.Device_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Activations", "Device_Id", "dbo.Devices");
            DropForeignKey("dbo.Activations", "Card_Id", "dbo.Cards");
            DropIndex("dbo.Activations", new[] { "Device_Id" });
            DropIndex("dbo.Activations", new[] { "Card_Id" });
            DropTable("dbo.Activations");
        }
    }
}
