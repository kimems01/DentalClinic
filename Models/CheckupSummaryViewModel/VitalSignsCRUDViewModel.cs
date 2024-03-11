using HMS.Models.CheckupSummaryViewModel;
using System;
using System.ComponentModel.DataAnnotations;

namespace HMS.Models.VitalSignsViewModel
{
    public class VitalSignsCRUDViewModel : EntityBase
    {
        public Int64 CheckupSummaryId { get; set; }
        [Display(Name = "BP Systolic")]
        public decimal? BPSystolic { get; set; }
        [Display(Name = "BP Diastolic")]
        public decimal? BPDiastolic { get; set; }
        [Display(Name = "Respiration Rate")]
        public decimal? RespirationRate { get; set; }
        [Display(Name = "Temperature(°F)")]
        public decimal? Temperature { get; set; }
        [Display(Name = "Pulse Rate")]
        public decimal? PulseRate { get; set; }
        [Display(Name = "Weight(Kgs)")]
        public decimal? Weight { get; set; }
        [Display(Name = "Height(Metres)")]
        public decimal? Height { get; set; }
        [Display(Name = "Spo2")]
        public decimal? Spo2 { get; set; }
        [Display(Name = "Nursing Notes")]
        public string NursingNotes { get; set; }



        public static implicit operator VitalSignsCRUDViewModel(CheckupSummary _VitalSigns)
        {
            return new VitalSignsCRUDViewModel
            {
                CheckupSummaryId = _VitalSigns.Id,
                BPSystolic = _VitalSigns.BPSystolic,
                BPDiastolic = _VitalSigns.BPDiastolic,
                RespirationRate = _VitalSigns.RespirationRate,
                Temperature = _VitalSigns.Temperature,
                PulseRate = _VitalSigns.PulseRate,
                Weight = _VitalSigns.Weight,
                Height = _VitalSigns.Height,
                Spo2 = _VitalSigns.Spo2,
                NursingNotes = _VitalSigns.NursingNotes,
                CreatedDate = _VitalSigns.CreatedDate,
                ModifiedDate = _VitalSigns.ModifiedDate,
                CreatedBy = _VitalSigns.CreatedBy,
                ModifiedBy = _VitalSigns.ModifiedBy,
                Cancelled = _VitalSigns.Cancelled,

            };
        }

        public static implicit operator CheckupSummary(VitalSignsCRUDViewModel vm)
        {
            return new CheckupSummary
            {
                Id = vm.CheckupSummaryId,
                BPSystolic = vm.BPSystolic,
                BPDiastolic = vm.BPDiastolic,
                RespirationRate = vm.RespirationRate,
                Temperature = vm.Temperature,
                PulseRate = vm.PulseRate,
                Weight = vm.Weight,
                Height = vm.Height,
                Spo2 = vm.Spo2,
                NursingNotes = vm.NursingNotes,
                CreatedDate = vm.CreatedDate,
                ModifiedDate = vm.ModifiedDate,
                CreatedBy = vm.CreatedBy,
                ModifiedBy = vm.ModifiedBy,
                Cancelled = vm.Cancelled,
            };
        }

        public static implicit operator VitalSignsCRUDViewModel(CheckupSummaryCRUDViewModel _VitalSigns)
        {
            return new VitalSignsCRUDViewModel
            {
                CheckupSummaryId = _VitalSigns.Id,
                BPSystolic = _VitalSigns.BPSystolic,
                BPDiastolic = _VitalSigns.BPDiastolic,
                RespirationRate = _VitalSigns.RespirationRate,
                Temperature = _VitalSigns.Temperature,
                PulseRate = _VitalSigns.PulseRate,
                Weight = _VitalSigns.Weight,
                Height = _VitalSigns.Height,
                Spo2 = _VitalSigns.Spo2,
                NursingNotes = _VitalSigns.NursingNotes,
                CreatedDate = _VitalSigns.CreatedDate,
                ModifiedDate = _VitalSigns.ModifiedDate,
                CreatedBy = _VitalSigns.CreatedBy,
                ModifiedBy = _VitalSigns.ModifiedBy,
                Cancelled = _VitalSigns.Cancelled,
            };
        }
    }
}
