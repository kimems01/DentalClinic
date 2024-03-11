using System;
using System.ComponentModel.DataAnnotations;

namespace HMS.Models.FamilyInfoViewModel
{
    public class FamilyInfoCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public Int64 Id { get; set; }
        [Required]
        public string Name { get; set; }


        public static implicit operator FamilyInfoCRUDViewModel(FamilyInfo _FamilyInfo)
        {
            return new FamilyInfoCRUDViewModel
            {
                Id = _FamilyInfo.Id,
                Name = _FamilyInfo.Name,
                CreatedDate = _FamilyInfo.CreatedDate,
                ModifiedDate = _FamilyInfo.ModifiedDate,
                CreatedBy = _FamilyInfo.CreatedBy,
                ModifiedBy = _FamilyInfo.ModifiedBy,
                Cancelled = _FamilyInfo.Cancelled,
            };
        }

        public static implicit operator FamilyInfo(FamilyInfoCRUDViewModel vm)
        {
            return new FamilyInfo
            {
                Id = vm.Id,
                Name = vm.Name,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}
