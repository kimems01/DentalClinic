using System;
using System.ComponentModel.DataAnnotations;
namespace HMS.Models.ProcedureViewModel;

public class ProceduresCRUDViewModel : EntityBase
{
    [Display(Name = "SL")]
    [Required]
    public Int64 Id { get; set; }
    [Display(Name = "Procedure Category")]
    [Required]
    public Int64 ProcedureCategoryId { get; set; }
    public string ProcedureCategoryName { get; set; }
    public string PaymentItemCode { get; set; }
    [Display(Name = "Procedure Name")]
    [Required]
    public string ProcedureName { get; set; }
    public string Unit { get; set; }
    [Display(Name = "Unit Price")]
    public double UnitPrice { get; set; }
    [Display(Name = "Reference Range")]
    public string ReferenceRange { get; set; }
    public bool Status { get; set; }
  

    public static implicit operator ProceduresCRUDViewModel(Procedures _Procedures)
    {
        return new ProceduresCRUDViewModel
        {
            Id = _Procedures.Id,
            ProcedureCategoryId = _Procedures.ProcedureCategoryId,
            PaymentItemCode = _Procedures.PaymentItemCode,
            ProcedureName = _Procedures.ProcedureName,
            Unit = _Procedures.Unit,
            UnitPrice = _Procedures.UnitPrice,
            ReferenceRange = _Procedures.ReferenceRange,
            Status = _Procedures.Status,
            CreatedDate = _Procedures.CreatedDate,
            ModifiedDate = _Procedures.ModifiedDate,
            CreatedBy = _Procedures.CreatedBy,
            ModifiedBy = _Procedures.ModifiedBy,
            Cancelled = _Procedures.Cancelled,

        };
    }

    public static implicit operator Procedures(ProceduresCRUDViewModel vm)
    {
        return new Procedures
        {
            Id = vm.Id,
            ProcedureCategoryId = vm.ProcedureCategoryId,
            PaymentItemCode = vm.PaymentItemCode,
            ProcedureName = vm.ProcedureName,
            Unit = vm.Unit,
            UnitPrice = vm.UnitPrice,
            ReferenceRange = vm.ReferenceRange,
            Status = vm.Status,
            CreatedDate = vm.CreatedDate,
            ModifiedDate = vm.ModifiedDate,
            CreatedBy = vm.CreatedBy,
            ModifiedBy = vm.ModifiedBy,
            Cancelled = vm.Cancelled,
        };
    }
}
