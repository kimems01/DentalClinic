using System;

namespace HMS.Models
{
    public class Procedures : EntityBase
    {
        public Int64 Id { get; set; }
        public Int64 ProcedureCategoryId { get; set; }
        public string PaymentItemCode { get; set; }
        public string ProcedureName { get; set; }
        public string Unit { get; set; }
        public double UnitPrice { get; set; }
        public string ReferenceRange { get; set; }
        public bool Status { get; set; }
    }
}
