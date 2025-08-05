using System;

namespace PatientApp.Shared
{
    public class Patient
    {
        public int Id { get; set; }
        public string Initials { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public DateTime DateTimeAdded { get; set; }
        public string Pill { get; set; } = string.Empty;
        public DateTime? PillAllocationTime { get; set; }
    }
}