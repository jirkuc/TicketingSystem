using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ITS_Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITS_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ITS_RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITS_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ITS_RoleClaims_ITS_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "ITS_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ITS_Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DefaultRoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "BIT", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITS_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ITS_Users_ITS_Roles_DefaultRoleId",
                        column: x => x.DefaultRoleId,
                        principalTable: "ITS_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ITS_Tickets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProblemDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportSource = table.Column<int>(type: "int", nullable: false),
                    ReportSourceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketState = table.Column<int>(type: "int", nullable: false),
                    TicketStateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedByRoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedByRoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClosedFlag = table.Column<bool>(type: "bit", nullable: false),
                    ClosedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ClosedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClosedByRoleId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ClosedByRoleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OutageTime = table.Column<int>(type: "int", nullable: false),
                    CurrentActivityNumber = table.Column<int>(type: "int", nullable: false),
                    LatestStatusComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastModifiedByUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedByRoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastModifiedByRoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITS_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ITS_Tickets_ITS_Roles_ClosedByRoleId",
                        column: x => x.ClosedByRoleId,
                        principalTable: "ITS_Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ITS_Tickets_ITS_Roles_CreatedByRoleId",
                        column: x => x.CreatedByRoleId,
                        principalTable: "ITS_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ITS_Tickets_ITS_Roles_LastModifiedByRoleId",
                        column: x => x.LastModifiedByRoleId,
                        principalTable: "ITS_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ITS_Tickets_ITS_Users_ClosedByUserId",
                        column: x => x.ClosedByUserId,
                        principalTable: "ITS_Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ITS_Tickets_ITS_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "ITS_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ITS_Tickets_ITS_Users_LastModifiedByUserId",
                        column: x => x.LastModifiedByUserId,
                        principalTable: "ITS_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ITS_UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITS_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ITS_UserClaims_ITS_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "ITS_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ITS_UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITS_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_ITS_UserLogins_ITS_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "ITS_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ITS_UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITS_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_ITS_UserRoles_ITS_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "ITS_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ITS_UserRoles_ITS_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "ITS_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ITS_UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITS_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_ITS_UserTokens_ITS_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "ITS_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "ITS_TicketActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    ActivityNumber = table.Column<int>(type: "int", nullable: false),
                    ActivityDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActivityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivityUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ActivityUserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivityRoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ActivityRoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentOutageTime = table.Column<int>(type: "int", nullable: false),
                    OldTicketState = table.Column<int>(type: "int", nullable: false),
                    OldTicketStateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewTicketState = table.Column<int>(type: "int", nullable: false),
                    NewTicketStateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InternalFlag = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ITS_TicketActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ITS_TicketActivities_ITS_Roles_ActivityRoleId",
                        column: x => x.ActivityRoleId,
                        principalTable: "ITS_Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ITS_TicketActivities_ITS_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "ITS_Tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ITS_TicketActivities_ITS_Users_ActivityUserId",
                        column: x => x.ActivityUserId,
                        principalTable: "ITS_Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ITS_RoleClaims_RoleId",
                table: "ITS_RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "ITS_Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ITS_TicketActivities_ActivityRoleId",
                table: "ITS_TicketActivities",
                column: "ActivityRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ITS_TicketActivities_ActivityUserId",
                table: "ITS_TicketActivities",
                column: "ActivityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ITS_TicketActivities_TicketId",
                table: "ITS_TicketActivities",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_ITS_Tickets_ClosedByRoleId",
                table: "ITS_Tickets",
                column: "ClosedByRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ITS_Tickets_ClosedByUserId",
                table: "ITS_Tickets",
                column: "ClosedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ITS_Tickets_CreatedByRoleId",
                table: "ITS_Tickets",
                column: "CreatedByRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ITS_Tickets_CreatedByUserId",
                table: "ITS_Tickets",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ITS_Tickets_LastModifiedByRoleId",
                table: "ITS_Tickets",
                column: "LastModifiedByRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ITS_Tickets_LastModifiedByUserId",
                table: "ITS_Tickets",
                column: "LastModifiedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ITS_UserClaims_UserId",
                table: "ITS_UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ITS_UserLogins_UserId",
                table: "ITS_UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ITS_UserRoles_RoleId",
                table: "ITS_UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "ITS_Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_ITS_Users_DefaultRoleId",
                table: "ITS_Users",
                column: "DefaultRoleId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "ITS_Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ITS_RoleClaims");

            migrationBuilder.DropTable(
                name: "ITS_TicketActivities");

            migrationBuilder.DropTable(
                name: "ITS_UserClaims");

            migrationBuilder.DropTable(
                name: "ITS_UserLogins");

            migrationBuilder.DropTable(
                name: "ITS_UserRoles");

            migrationBuilder.DropTable(
                name: "ITS_UserTokens");

            migrationBuilder.DropTable(
                name: "ITS_Tickets");

            migrationBuilder.DropTable(
                name: "ITS_Users");

            migrationBuilder.DropTable(
                name: "ITS_Roles");
        }
    }
}
