using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HR.LeaveManagement.Identity.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedUserPasswordHashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00b6c0e0-3db0-4a73-99d9-cbd0ffe50f66",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEE+MrcGhp8VKzwF8LsgrWNAj30L8IU7KeVedXJVtBW3PcGj/uodEbjg9MjijOmgO1Q==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3e41553c-7bad-439e-8578-060629ef3efe",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAwXoFG+oXbSJCiLWr8pun1gsFh66/nZEHgs4LsofDYMTzi1MYUQ2Q+9VTlPFTh6cQ==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ec7eff12-ed15-4d11-a89c-bf0be9913144",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDF3GdvaVZU7v5b8FYL9MShcPZ6Tqvawsf67yfLWwot63gzy0FEVZ/xdfFlR4DJ93A==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00b6c0e0-3db0-4a73-99d9-cbd0ffe50f66",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEICbmZyvU0eRhWd6JLqb0/XER3OsSKN/Qri/eVWRfsJvGV5eSI3UcpvrxeU5BCKzBg==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3e41553c-7bad-439e-8578-060629ef3efe",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAECWgdMFEwZRh5VV0TGmZ2vDeE2UmciQieURubqlFwzHpkfwPHx9xdtdtEA6+gLIt3g==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "ec7eff12-ed15-4d11-a89c-bf0be9913144",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDAPBazKxc/ro5Tj7kqTQaMskAAUMv4PT2Bn+Lu0tbd/5PSMre2axWPhiCOLgp7kbA==");
        }
    }
}
