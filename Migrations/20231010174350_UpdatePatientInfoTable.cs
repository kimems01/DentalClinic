using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMS.Migrations
{
    public partial class UpdatePatientInfoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MotherName",
                table: "PatientInfo",
                newName: "Surname");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "PatientInfo",
                newName: "Residence");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "PatientInfo",
                newName: "PaymentMode");

            migrationBuilder.RenameColumn(
                name: "FatherName",
                table: "PatientInfo",
                newName: "OtherNames");

            migrationBuilder.AddColumn<long>(
                name: "EmploymentCompany",
                table: "PatientInfo",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Family",
                table: "PatientInfo",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianName",
                table: "PatientInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianPhone",
                table: "PatientInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GuardianRelationship",
                table: "PatientInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "InsuranceCompany",
                table: "PatientInfo",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NationalID",
                table: "PatientInfo",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmploymentCompany",
                table: "PatientInfo");

            migrationBuilder.DropColumn(
                name: "Family",
                table: "PatientInfo");

            migrationBuilder.DropColumn(
                name: "GuardianName",
                table: "PatientInfo");

            migrationBuilder.DropColumn(
                name: "GuardianPhone",
                table: "PatientInfo");

            migrationBuilder.DropColumn(
                name: "GuardianRelationship",
                table: "PatientInfo");

            migrationBuilder.DropColumn(
                name: "InsuranceCompany",
                table: "PatientInfo");

            migrationBuilder.DropColumn(
                name: "NationalID",
                table: "PatientInfo");

            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "PatientInfo",
                newName: "MotherName");

            migrationBuilder.RenameColumn(
                name: "Residence",
                table: "PatientInfo",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "PaymentMode",
                table: "PatientInfo",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "OtherNames",
                table: "PatientInfo",
                newName: "FatherName");
        }
    }
}
