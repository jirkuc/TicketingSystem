using TicketingSystem.Enums;

namespace TicketingSystem.Models; 
public class Ticket {
    public int Id { get; set; }
    public string ProblemDescription { get; set; }
    public DateTime CreatedDate { get; set; }
    public Priority Priority { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public ReportSource ReportSource { get; set; }
    public string? ReportSourceName { get; set; } //
    public TicketState TicketState { get; set; }
    public string? TicketStateName { get; set; } //
    public AppUser CreatedByUser { get; set; }
    public string CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; } //
    public AppRole CreatedByRole { get; set; }
    public string CreatedByRoleId { get; set; }
    public string CreatedByRoleName {  get; set; } //
    public bool ClosedFlag { get; set; }
    public DateTime? ClosedDate { get; set; }
    public AppUser? ClosedByUser { get; set; }
    public string? ClosedByUserId { get; set; }
    public string? ClosedByUserName { get; set; } //
    public AppRole? ClosedByRole { get; set; }
    public string? ClosedByRoleId { get; set; }
    public string? ClosedByRoleName { get; set; } //
    public DateTime LastModifiedDate { get; set; }
    public int OutageTime { get; set; }
    public int CurrentActivityNumber { get; set; }
    public string? LatestStatusComment { get; set; }
    public AppUser LastModifiedByUser { get; set; }
    public string LastModifiedByUserId { get; set; }
    public string LastModifiedByUserName { get; set; } //
    public AppRole LastModifiedByRole { get;set; }
    public string LastModifiedByRoleId { get;set; }
    public string LastModifiedByRoleName { get; set; } //
    public List<TicketActivity> TicketActivities { get; set; }
}
