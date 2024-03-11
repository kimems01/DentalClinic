using System;
using System.ComponentModel.DataAnnotations;

namespace HMS.Models.PatientInfoViewModel
{
    public class PatientInfoCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public Int64 Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string PatientCode { get; set; }
        [Display(Name = "Other Names")]
        [Required]
        public string OtherNames { get; set; }
        [Display(Name = "Surname")]
        [Required]
        public string Surname { get; set; }
        public string FullName { get; set; }
        [Display(Name = "Marital Status")]
        public string MaritalStatus { get; set; }
        public string Gender { get; set; }
        [Display(Name = "Spouse Name")]
        public string SpouseName { get; set; }
        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }
        [Display(Name = "Date Of Birth")]
        public DateTime? DateOfBirth { get; set; } = DateTime.Today;
        [Display(Name = "Registration Fee")]
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

        [Display(Name = "National ID")]
        public int NationalID { get; set; }
        public Int64? Family { get; set; }
        [Display(Name = "Employment Company")]
        public Int64? EmploymentCompany { get; set; }
        [Display(Name = "Insurance Company")]
        public Int64? InsuranceCompany { get; set; }
        public string Residence { get; set; }
        [Display(Name = "Payment Mode")]
        public string PaymentMode { get; set; }
        [Display(Name = "Guardian Name")]
        public string GuardianName { get; set; }
        [Display(Name = "Guardian Phone")]
        public string GuardianPhone { get; set; }
        [Display(Name = "Guardian Relationship")]
        public string GuardianRelationship { get; set; }

        public static implicit operator PatientInfoCRUDViewModel(PatientInfo _PatientInfo)
        {
            return new PatientInfoCRUDViewModel
            {
                Id = _PatientInfo.Id,
                ApplicationUserId = _PatientInfo.ApplicationUserId,
                PatientCode = _PatientInfo.PatientCode,
                OtherNames = _PatientInfo.OtherNames,
                Surname = _PatientInfo.Surname,
                MaritalStatus = _PatientInfo.MaritalStatus,
                Gender = _PatientInfo.Gender,
                SpouseName = _PatientInfo.SpouseName,
                BloodGroup = _PatientInfo.BloodGroup,
                DateOfBirth = _PatientInfo.DateOfBirth,
                RegistrationFee = _PatientInfo.RegistrationFee,
                Phone = _PatientInfo.Phone,
                Email = _PatientInfo.Email,
                Address = _PatientInfo.Address,
                Country = _PatientInfo.Country,
                Agreement = _PatientInfo.Agreement,
                Remarks = _PatientInfo.Remarks,
                PasswordHash = _PatientInfo.PasswordHash,
                ConfirmPassword = _PatientInfo.ConfirmPassword,
                OldPassword = _PatientInfo.OldPassword,
                ProfilePicture = _PatientInfo.ProfilePicture,
                NationalID = _PatientInfo.NationalID,
                Family = _PatientInfo.Family,
                EmploymentCompany = _PatientInfo.EmploymentCompany,
                InsuranceCompany = _PatientInfo.InsuranceCompany,
                Residence = _PatientInfo.Residence,
                PaymentMode = _PatientInfo.PaymentMode,
                GuardianName = _PatientInfo.GuardianName,
                GuardianPhone = _PatientInfo.GuardianPhone,
                GuardianRelationship = _PatientInfo.GuardianRelationship,
                CreatedDate = _PatientInfo.CreatedDate,
                ModifiedDate = _PatientInfo.ModifiedDate,
                CreatedBy = _PatientInfo.CreatedBy,
                ModifiedBy = _PatientInfo.ModifiedBy,
                Cancelled = _PatientInfo.Cancelled,

            };
        }

        public static implicit operator PatientInfo(PatientInfoCRUDViewModel vm)
        {
            return new PatientInfo
            {
                Id = vm.Id,
                ApplicationUserId = vm.ApplicationUserId,
                PatientCode = vm.PatientCode,
                OtherNames = vm.OtherNames,
                Surname = vm.Surname,
                MaritalStatus = vm.MaritalStatus,
                Gender = vm.Gender,
                SpouseName = vm.SpouseName,
                BloodGroup = vm.BloodGroup,
                DateOfBirth = vm.DateOfBirth,
                RegistrationFee = vm.RegistrationFee,
                Phone = vm.Phone,
                Email = vm.Email,
                Address = vm.Address,
                Country = vm.Country,
                Agreement = vm.Agreement,
                Remarks = vm.Remarks,
                PasswordHash = vm.PasswordHash,
                ConfirmPassword = vm.ConfirmPassword,
                OldPassword = vm.OldPassword,
                ProfilePicture = vm.ProfilePicture,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                NationalID = vm.NationalID,
                Family = vm.Family,
                EmploymentCompany = vm.EmploymentCompany,
                InsuranceCompany = vm.InsuranceCompany,
                Residence = vm.Residence,
                PaymentMode = vm.PaymentMode,
                GuardianName = vm.GuardianName,
                GuardianPhone = vm.GuardianPhone,
                GuardianRelationship = vm.GuardianRelationship,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,

            };
        }
    }
}
