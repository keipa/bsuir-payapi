using System.Data.Entity.Migrations;

namespace PayAPI.Migrations
{
    public partial class logs : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Devices", "Card_Id", "dbo.Cards");
            DropIndex("dbo.Devices", new[] {"Card_Id"});
            CreateTable(
                    "dbo.Logs",
                    c => new
                    {
                        Id = c.Int(false, true),
                        Exception = c.String(),
                        BrokenAt = c.DateTime(false)
                    })
                .PrimaryKey(t => t.Id);

            AddColumn("dbo.Cards", "AuthorizationCode", c => c.Int(false));
            AddColumn("dbo.Devices", "WrongInputCount", c => c.Int(false));
            AddColumn("dbo.Devices", "BannedUntil", c => c.DateTime(false));
            AddColumn("dbo.Users", "email", c => c.String());
            AddColumn("dbo.Users", "phone", c => c.String());
            AddColumn("dbo.Tokens", "ExpiredDate", c => c.DateTime(false));
            DropColumn("dbo.Devices", "Card_Id");
            DropColumn("dbo.Tokens", "Expired");
        }

        public override void Down()
        {
            AddColumn("dbo.Tokens", "Expired", c => c.DateTime(false));
            AddColumn("dbo.Devices", "Card_Id", c => c.Int());
            DropColumn("dbo.Tokens", "ExpiredDate");
            DropColumn("dbo.Users", "phone");
            DropColumn("dbo.Users", "email");
            DropColumn("dbo.Devices", "BannedUntil");
            DropColumn("dbo.Devices", "WrongInputCount");
            DropColumn("dbo.Cards", "AuthorizationCode");
            DropTable("dbo.Logs");
            CreateIndex("dbo.Devices", "Card_Id");
            AddForeignKey("dbo.Devices", "Card_Id", "dbo.Cards", "Id");
        }
    }
}