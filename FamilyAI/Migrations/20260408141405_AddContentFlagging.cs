using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FamilyAI.Migrations
{
    /// <inheritdoc />
    public partial class AddContentFlagging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GlobalFlagRules",
                schema: "Chat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalFlagRules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParentFlagRules",
                schema: "Chat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentUserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    GlobalRuleId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentFlagRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParentFlagRules_GlobalFlagRules_GlobalRuleId",
                        column: x => x.GlobalRuleId,
                        principalSchema: "Chat",
                        principalTable: "GlobalFlagRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ParentFlagRules_Users_ParentUserId",
                        column: x => x.ParentUserId,
                        principalSchema: "Chat",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ParentScanSettings",
                schema: "Chat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentUserId = table.Column<int>(type: "int", nullable: false),
                    Sensitivity = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParentScanSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParentScanSettings_Users_ParentUserId",
                        column: x => x.ParentUserId,
                        principalSchema: "Chat",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThreadScanResults",
                schema: "Chat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThreadId = table.Column<int>(type: "int", nullable: false),
                    ScannedByParentId = table.Column<int>(type: "int", nullable: false),
                    ScannedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsFlagged = table.Column<bool>(type: "bit", nullable: false),
                    AiSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RuleSetHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadScanResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThreadScanResults_Threads_ThreadId",
                        column: x => x.ThreadId,
                        principalSchema: "Chat",
                        principalTable: "Threads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ThreadScanResults_Users_ScannedByParentId",
                        column: x => x.ScannedByParentId,
                        principalSchema: "Chat",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThreadFlagDetails",
                schema: "Chat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThreadScanResultId = table.Column<int>(type: "int", nullable: false),
                    MessageId = table.Column<int>(type: "int", nullable: true),
                    RuleType = table.Column<int>(type: "int", nullable: false),
                    RuleValue = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MatchedExcerpt = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Source = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadFlagDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThreadFlagDetails_ChatLogs_MessageId",
                        column: x => x.MessageId,
                        principalSchema: "Chat",
                        principalTable: "ChatLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ThreadFlagDetails_ThreadScanResults_ThreadScanResultId",
                        column: x => x.ThreadScanResultId,
                        principalSchema: "Chat",
                        principalTable: "ThreadScanResults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "Chat",
                table: "GlobalFlagRules",
                columns: new[] { "Id", "Description", "IsDeleted", "Type", "Value" },
                values: new object[,]
                {
                    { 1, "Direct reference to suicide", false, 0, "suicide" },
                    { 2, "Self-harm phrase", false, 0, "kill myself" },
                    { 3, "Self-harm phrase", false, 0, "self harm" },
                    { 4, "Self-harm phrase", false, 0, "cutting myself" },
                    { 5, "General drug reference", false, 0, "drugs" },
                    { 6, "Cannabis reference", false, 0, "marijuana" },
                    { 7, "Cannabis slang", false, 0, "weed" },
                    { 8, "Hard drug reference", false, 0, "cocaine" },
                    { 9, "Hard drug reference", false, 0, "heroin" },
                    { 10, "Hard drug reference", false, 0, "meth" },
                    { 11, "Hard drug reference", false, 0, "fentanyl" },
                    { 12, "Alcohol reference", false, 0, "alcohol" },
                    { 13, "Sharing personal location information", false, 0, "my address" },
                    { 14, "Sharing personal contact information", false, 0, "my phone number" },
                    { 15, "Sharing school information with strangers", false, 0, "my school" },
                    { 16, "Any discussion of self-harm, suicidal thoughts, or hurting oneself", false, 1, "Self-harm or suicide" },
                    { 17, "Discussion of using, obtaining, or glorifying drugs or alcohol", false, 1, "Drug or alcohol use" },
                    { 18, "Threats of violence, planning to hurt someone, or glorifying violence", false, 1, "Violence or physical threats" },
                    { 19, "Sexually explicit topics, pornography, or adult themes inappropriate for children", false, 1, "Sexual or adult content" },
                    { 20, "Bullying others, being bullied, or discussions of targeted harassment", false, 1, "Bullying or harassment" },
                    { 21, "Sharing home address, phone number, school name, or other identifying information", false, 1, "Sharing personal information" },
                    { 22, "Adult asking child for personal info, photos, or to meet in person", false, 1, "Online predator or grooming behavior" },
                    { 23, "Discussion of obtaining or using weapons", false, 1, "Weapons" },
                    { 24, "Messages with a threatening, hostile, or intimidating tone directed at a person", false, 2, "Threatening or aggressive" },
                    { 25, "Messages with a sexually suggestive or inappropriate tone for a child", false, 2, "Sexually suggestive" },
                    { 26, "Messages expressing extreme despair, hopelessness, or emotional crisis", false, 2, "Deeply distressed or hopeless" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParentFlagRules_GlobalRuleId",
                schema: "Chat",
                table: "ParentFlagRules",
                column: "GlobalRuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentFlagRules_ParentUserId",
                schema: "Chat",
                table: "ParentFlagRules",
                column: "ParentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ParentScanSettings_ParentUserId",
                schema: "Chat",
                table: "ParentScanSettings",
                column: "ParentUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadFlagDetails_MessageId",
                schema: "Chat",
                table: "ThreadFlagDetails",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadFlagDetails_ThreadScanResultId",
                schema: "Chat",
                table: "ThreadFlagDetails",
                column: "ThreadScanResultId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadScanResults_ScannedByParentId",
                schema: "Chat",
                table: "ThreadScanResults",
                column: "ScannedByParentId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadScanResults_ThreadId",
                schema: "Chat",
                table: "ThreadScanResults",
                column: "ThreadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ThreadFlagDetails",
                schema: "Chat");

            migrationBuilder.DropTable(
                name: "ParentFlagRules",
                schema: "Chat");

            migrationBuilder.DropTable(
                name: "ParentScanSettings",
                schema: "Chat");

            migrationBuilder.DropTable(
                name: "ThreadScanResults",
                schema: "Chat");

            migrationBuilder.DropTable(
                name: "GlobalFlagRules",
                schema: "Chat");
        }
    }
}
