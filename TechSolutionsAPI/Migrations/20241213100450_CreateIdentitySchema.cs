using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechSolutionsAPI.Migrations
{
    public partial class CreateIdentitySchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if the 'Customers' table exists before attempting to create it.
            // Reason : Tables were creating using MSSMS prior to using EF
            if (!TableExists(migrationBuilder, "Customers"))
            {
                migrationBuilder.CreateTable(
                    name: "Customers",
                    columns: table => new
                    {
                        CustomerId = table.Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        FirstName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                        LastName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                        Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                        PhoneNumber = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                        InvoiceDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK__Customer__A4AE64D87BAAE00C", x => x.CustomerId);
                    });
            }

            // Check if the 'Addresses' table exists before attempting to create it.
            // Reason : Tables were creating using MSSMS prior to using EF
            if (!TableExists(migrationBuilder, "Addresses"))
            {
                migrationBuilder.CreateTable(
                    name: "Addresses",
                    columns: table => new
                    {
                        AddressId = table.Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        CustomerId = table.Column<int>(type: "int", nullable: true),
                        StreetAddress = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                        City = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                        Provice = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                        PostalCode = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                        Country = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                        IsPrimary = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK__Addresse__091C2AFB8C6D1AF8", x => x.AddressId);
                        table.ForeignKey(
                            name: "FK__Addresses__Custo__29572725",
                            column: x => x.CustomerId,
                            principalTable: "Customers",
                            principalColumn: "CustomerId");
                    });
            }

            // Check if the 'Invoices' table exists before attempting to create it.
            // Reason : Tables were creating using MSSMS prior to using EF
            if (!TableExists(migrationBuilder, "Invoices"))
            {
                migrationBuilder.CreateTable(
                    name: "Invoices",
                    columns: table => new
                    {
                        InvoiceId = table.Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        CustomerId = table.Column<int>(type: "int", nullable: true),
                        InvoiceDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                        Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                        ShippingAddressID = table.Column<int>(type: "int", nullable: true),
                        Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK__Invoices__D796AAB5DF4BF638", x => x.InvoiceId);
                        table.ForeignKey(
                            name: "FK__Invoices__Custom__2D27B809",
                            column: x => x.CustomerId,
                            principalTable: "Customers",
                            principalColumn: "CustomerId");
                        table.ForeignKey(
                            name: "FK__Invoices__Shippi__2E1BDC42",
                            column: x => x.ShippingAddressID,
                            principalTable: "Addresses",
                            principalColumn: "AddressId");
                    });
            }

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Customers");
        }

        // Helper method to check if a table exists in the database
        // Ensures EF doesnt try create already existing tables
        private bool TableExists(MigrationBuilder migrationBuilder, string tableName)
        {
            // This is a simple check for table existence in SQL Server.
            // You could write a more complex check if necessary based on the DB you use.
            var sql = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'";
            var tableExists = migrationBuilder.Sql(sql);  // Execute raw SQL
            return tableExists != null;
        }
    }
}
