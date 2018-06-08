namespace MechanicsForum.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProblemsUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Problems", "PostDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Problems", "ModifiedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Problems", "ModifiedDate");
            DropColumn("dbo.Problems", "PostDate");
        }
    }
}
