namespace MechanicsForum.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Problem : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Problems",
                c => new
                    {
                        ProblemId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Status = c.Boolean(nullable: false),
                        UserId = c.String(),
                        MediaPath = c.String(),
                    })
                .PrimaryKey(t => t.ProblemId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Problems");
        }
    }
}
