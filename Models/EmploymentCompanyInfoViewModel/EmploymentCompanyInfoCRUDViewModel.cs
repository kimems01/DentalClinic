using System;
using System.ComponentModel.DataAnnotations;

namespace HMS.Models.EmploymentCompanyInfoViewModel
{
    public class EmploymentCompanyInfoCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public Int64 Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        [Display(Name = "Coverage Details")]
        public string CoverageDetails { get; set; }


        public static implicit operator EmploymentCompanyInfoCRUDViewModel(EmploymentCompanyInfo _EmploymentCompanyInfo)
        {
            return new EmploymentCompanyInfoCRUDViewModel
            {
                Id = _EmploymentCompanyInfo.Id,
                Name = _EmploymentCompanyInfo.Name,
                Address = _EmploymentCompanyInfo.Address,
                Phone = _EmploymentCompanyInfo.Phone,
                Email = _EmploymentCompanyInfo.Email,
                CoverageDetails = _EmploymentCompanyInfo.CoverageDetails,
                CreatedDate = _EmploymentCompanyInfo.CreatedDate,
                ModifiedDate = _EmploymentCompanyInfo.ModifiedDate,
                CreatedBy = _EmploymentCompanyInfo.CreatedBy,
                ModifiedBy = _EmploymentCompanyInfo.ModifiedBy,
                Cancelled = _EmploymentCompanyInfo.Cancelled,
            };
        }

        public static implicit operator EmploymentCompanyInfo(EmploymentCompanyInfoCRUDViewModel vm)
        {
            return new EmploymentCompanyInfo
            {
                Id = vm.Id,
                Name = vm.Name,
                Address = vm.Address,
                Phone = vm.Phone,
                Email = vm.Email,
                CoverageDetails = vm.CoverageDetails,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
