using System;

namespace HMS.Models.EmploymentCompanyInfoViewModel
{
    public class EmploymentCompanyInfoGridViewModel : EntityBase
    {
        public Int64 Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string CoverageDetails { get; set; }
    }
}

