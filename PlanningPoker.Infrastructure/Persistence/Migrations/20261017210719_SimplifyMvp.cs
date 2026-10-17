using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlanningPoker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyMvp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Votes_EstimationId_ParticipantId_RoundNumber",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "RoundNumber",
                table: "Votes");

            migrationBuilder.DropColumn(
                name: "RoundNumber",
                table: "Estimations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Estimations");

            migrationBuilder.RenameColumn(
                name: "HostTokenHash",
                table: "Rooms",
                newName: "AccessCodeHash");

            migrationBuilder.AddColumn<bool>(
                name: "IsRevealed",
                table: "Estimations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_EstimationId_ParticipantId",
                table: "Votes",
                columns: new[] { "EstimationId", "ParticipantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_AccessCodeHash",
                table: "Rooms",
                column: "AccessCodeHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Participants_RoomId_Name_Role",
                table: "Participants",
                columns: new[] { "RoomId", "Name", "Role" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Votes_EstimationId_ParticipantId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_AccessCodeHash",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Participants_RoomId_Name_Role",
                table: "Participants");

            migrationBuilder.DropColumn(
                name: "IsRevealed",
                table: "Estimations");

            migrationBuilder.RenameColumn(
                name: "AccessCodeHash",
                table: "Rooms",
                newName: "HostTokenHash");

            migrationBuilder.AddColumn<int>(
                name: "RoundNumber",
                table: "Votes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoundNumber",
                table: "Estimations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Estimations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Votes_EstimationId_ParticipantId_RoundNumber",
                table: "Votes",
                columns: new[] { "EstimationId", "ParticipantId", "RoundNumber" },
                unique: true);
        }
    }
}
