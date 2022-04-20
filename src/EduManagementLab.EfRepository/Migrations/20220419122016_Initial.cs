using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduManagementLab.EfRepository.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tools",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeepLinkingLaunchUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeploymentId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentityServerClientId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LaunchUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoginUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tools", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Displayname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResourceLinks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ToolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceLinks_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ResourceLinks_Tools_ToolId",
                        column: x => x.ToolId,
                        principalTable: "Tools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseMemberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EnrolledDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseMemberships_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseMemberships_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseLineItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResourceLinkId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseLineItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseLineItems_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseLineItems_ResourceLinks_ResourceLinkId",
                        column: x => x.ResourceLinkId,
                        principalTable: "ResourceLinks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LineItemResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MembershipId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseLineItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<decimal>(type: "decimal", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineItemResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineItemResults_CourseLineItems_CourseLineItemId",
                        column: x => x.CourseLineItemId,
                        principalTable: "CourseLineItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LineItemResults_CourseMemberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "CourseMemberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseLineItems_CourseId",
                table: "CourseLineItems",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseLineItems_ResourceLinkId",
                table: "CourseLineItems",
                column: "ResourceLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMemberships_CourseId",
                table: "CourseMemberships",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseMemberships_UserId",
                table: "CourseMemberships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LineItemResults_CourseLineItemId",
                table: "LineItemResults",
                column: "CourseLineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_LineItemResults_MembershipId",
                table: "LineItemResults",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceLinks_CourseId",
                table: "ResourceLinks",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceLinks_ToolId",
                table: "ResourceLinks",
                column: "ToolId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineItemResults");

            migrationBuilder.DropTable(
                name: "CourseLineItems");

            migrationBuilder.DropTable(
                name: "CourseMemberships");

            migrationBuilder.DropTable(
                name: "ResourceLinks");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Tools");
        }
    }
}
