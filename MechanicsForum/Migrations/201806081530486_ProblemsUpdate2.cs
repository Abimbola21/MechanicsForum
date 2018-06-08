namespace MechanicsForum.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProblemsUpdate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Problems", "DateClosed", c => c.DateTime(nullable: false));
            AddColumn("dbo.Problems", "SolvedBy", c => c.String());
            AddColumn("dbo.Problems", "ClosedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Problems", "ClosedBy");
            DropColumn("dbo.Problems", "SolvedBy");
            DropColumn("dbo.Problems", "DateClosed");
        }
    }
}
