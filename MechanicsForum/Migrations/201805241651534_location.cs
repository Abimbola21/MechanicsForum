namespace MechanicsForum.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class location : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Problems", "Solved", c => c.Boolean(nullable: false));
            AddColumn("dbo.Problems", "location", c => c.String());
            DropColumn("dbo.Problems", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Problems", "Status", c => c.Boolean(nullable: false));
            DropColumn("dbo.Problems", "location");
            DropColumn("dbo.Problems", "Solved");
        }
    }
}
