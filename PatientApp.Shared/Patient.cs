using System;

namespace PatientApp.Shared
{
    public enum Pill
    {
        None,
        Red,
        Blue
    }

    public class Patient
    {
        public Guid Id { get; set; }
        public string Initials { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public DateTime AddedAt { get; set; }
        public Pill Pill { get; set; } = Pill.None;
        public DateTime? AllocatedAt { get; set; }
    }
}
