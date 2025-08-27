using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitnessApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class Remove_DefaultReps2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultReps",
                table: "ExerciseModels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultReps",
                table: "ExerciseModels",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
