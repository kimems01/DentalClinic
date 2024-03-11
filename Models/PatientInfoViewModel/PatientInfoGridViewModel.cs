using System;

namespace HMS.Models.PatientInfoViewModel
{
    public class PatientInfoGridViewModel : EntityBase
    {
        public Int64 Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string OtherNames { get; set; }
        public string Surname { get; set; }
        public string MaritalStatus { get; set; }
        public string Gender { get; set; }
        public string SpouseName { get; set; }
        public string BloodGroup { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public double? RegistrationFee { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public bool? Agreement { get; set; }
        public string Remarks { get; set; }
        public string PasswordHash { get; set; }
        public string ConfirmPassword { get; set; }
        public string OldPassword { get; set; }
        public string ProfilePicture { get; set; }

        public int NationalID { get; set; }
        public Int64? Family { get; set; }
        public Int64? EmploymentCompany { get; set; }
        public Int64? InsuranceCompany { get; set; }
        public string Residence { get; set; }
        public string PaymentMode { get; set; }
        public string GuardianName { get; set; }
        public string GuardianPhone { get; set; }
        public string GuardianRelationship { get; set; }
    }
}
