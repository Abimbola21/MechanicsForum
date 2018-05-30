namespace MechanicsForum.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class status : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Problems", "Status", c => c.String());
            AddColumn("dbo.Problems", "Comment", c => c.String());
            DropColumn("dbo.Problems", "Solved");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Problems", "Solved", c => c.Boolean(nullable: false));
            DropColumn("dbo.Problems", "Comment");
            DropColumn("dbo.Problems", "Status");
        }
    }
}
