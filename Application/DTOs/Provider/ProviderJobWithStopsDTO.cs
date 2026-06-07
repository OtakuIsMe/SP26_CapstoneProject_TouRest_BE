namespace TouRest.Application.DTOs.Provider
{
    public class ProviderJobWithStopsDTO
    {
        public Guid ScheduleId { get; set; }
        public Guid ItineraryId { get; set; }
        public string ItineraryName { get; set; } = null!;
        public string AgencyName { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Spot { get; set; }
        public int SpotLeft { get; set; }
        public string Status { get; set; } = null!;
        public List<ProviderStopDetailDTO> Stops { get; set; } = [];
    }

    public class ProviderStopDetailDTO
    {
        public Guid StopId { get; set; }
        public string Name { get; set; } = null!;
        public int StopOrder { get; set; }
        public string? Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Guid? AssignedStaffId { get; set; }
        public string? AssignedStaffName { get; set; }
        public string? AssignedStaffEmail { get; set; }
        public List<StopActivityDTO> Activities { get; set; } = [];
    }

    public class StopActivityDTO
    {
        public Guid ActivityId { get; set; }
        public int ActivityOrder { get; set; }
        public string Name { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public long Price { get; set; }
        public string? Note { get; set; }
    }

    public class AssignStaffToStopRequest
    {
        public Guid ScheduleId { get; set; }
        public Guid StaffId { get; set; }
    }
}
