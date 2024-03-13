using System;
using System.ComponentModel.DataAnnotations;

namespace HMS.Models.ProcedureCategoriesViewModel
{
    public class ProcedureCategoriesCRUDViewModel : EntityBase
    {
        [Display(Name = "SL")]
        [Required]
        public Int64 Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }


        public static implicit operator ProcedureCategoriesCRUDViewModel(ProcedureCategories _ProcedureCategories)
        {
            return new ProcedureCategoriesCRUDViewModel
            {
                Id = _ProcedureCategories.Id,
                Name = _ProcedureCategories.Name,
                Description = _ProcedureCategories.Description,
                CreatedDate = _ProcedureCategories.CreatedDate,
                ModifiedDate = _ProcedureCategories.ModifiedDate,
                CreatedBy = _ProcedureCategories.CreatedBy,
                ModifiedBy = _ProcedureCategories.ModifiedBy,
                Cancelled = _ProcedureCategories.Cancelled,
            };
        }

        public static implicit operator ProcedureCategories(ProcedureCategoriesCRUDViewModel vm)
        {
            return new ProcedureCategories
            {
                Id = vm.Id,
                Name = vm.Name,
                Description = vm.Description,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }
    }
}