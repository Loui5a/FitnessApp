using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitnessApp.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExerciseModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Exercise = table.Column<string>(type: "text", nullable: false),
                    DefaultDuration = table.Column<string>(type: "text", nullable: false),
                    DefaultReps = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProgramModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecordLogModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reps = table.Column<int>(type: "integer", nullable: false),
                    Sets = table.Column<int>(type: "integer", nullable: false),
                    Weight = table.Column<decimal>(type: "numeric", nullable: false),
                    ExerciseId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordLogModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecordLogModels_ExerciseModels_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "ExerciseModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramLogModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    ExerciseId = table.Column<int>(type: "integer", nullable: false),
                    ProgramId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramLogModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramLogModels_ExerciseModels_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "ExerciseModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramLogModels_ProgramModels_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "ProgramModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramLogModels_ExerciseId",
                table: "ProgramLogModels",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramLogModels_ProgramId",
                table: "ProgramLogModels",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordLogModels_ExerciseId",
                table: "RecordLogModels",
                column: "ExerciseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgramLogModels");

            migrationBuilder.DropTable(
                name: "RecordLogModels");

            migrationBuilder.DropTable(
                name: "ProgramModels");

            migrationBuilder.DropTable(
                name: "ExerciseModels");
        }
    }
}
