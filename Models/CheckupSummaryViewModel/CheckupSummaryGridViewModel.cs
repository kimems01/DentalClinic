using System;

namespace HMS.Models.CheckupSummaryViewModel
{
    public class CheckupSummaryGridViewModel : EntityBase
    {
        public Int64 Id { get; set; }
        public Int64 PatientId { get; set; }
        public string VisitId { get; set; }
        public Int64 DoctorId { get; set; }
        public string PatientType { get; set; }
        public string Symptoms { get; set; }
        public string Diagnosis { get; set; }
        public string HPI { get; set; }
        public string VitalSigns { get; set; }
        public string PhysicalExamination { get; set; }
        public string Comments { get; set; }
        public DateTime CheckupDate { get; set; }
        public DateTime NextVisitDate { get; set; }
        public string Advice { get; set; }
        public decimal? BPSystolic { get; set; }
        public decimal? BPDiastolic { get; set; }
        public decimal? RespirationRate { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? PulseRate { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Spo2 { get; set; }
        public decimal? Height { get; set; }
        public string NursingNotes { get; set; }
    }
}
