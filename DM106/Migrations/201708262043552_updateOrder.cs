namespace DM106.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateOrder : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "dataPedido", c => c.DateTime());
            AlterColumn("dbo.Orders", "dataEntrega", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "dataEntrega", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Orders", "dataPedido", c => c.DateTime(nullable: false));
        }
    }
}
