using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTableAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "Categories",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price50 = table.Column<double>(type: "float", nullable: false),
                    Price100 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[,]
                {
                    { 1, "Billy Spark", "akdjaks asldijaslid asldijalidjgubr ggkruggd slfoksel", "SWD999921", 99.0, 90.0, 80.0, 85.0, "One Man's Sky" },
                    { 2, "Jason Brody", "akdjaks asldijaslid asldijalidjgubr ggkruggd slfoksel", "SWD999451", 80.0, 75.0, 65.0, 70.0, "Rocky" },
                    { 3, "Tommy Frag", "akdjaks asldijaslid asldijalidjgubr ggkruggd slfoksel", "SWD993321", 40.0, 45.0, 35.0, 40.0, "Invisible" },
                    { 4, "Abella Danger", "akdjaks asldijaslid asldijalidjgubr ggkruggd slfoksel", "SWD991911", 120.0, 100.0, 80.0, 85.0, "Sambo" },
                    { 5, "Franky Modest", "akdjaks asldijaslid asldijalidjgubr ggkruggd slfoksel", "SWD941121", 99.0, 90.0, 80.0, 85.0, "Lust" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "name",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}
