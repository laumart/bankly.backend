using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestBankly.Api.Migrations
{
    /// <inheritdoc />
    public partial class inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionId = table.Column<string>(type: "VARCHAR(100)", nullable: false),
                    AccountOrigin = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    AccountDestination = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Status = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    ErrorReason = table.Column<string>(type: "VARCHAR(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_transaction_id",
                table: "Transaction",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transaction");
        }
    }
}
