using System.ComponentModel;
using TicketingSystem.Enums;
using TicketingSystem.Models;

namespace TicketingSystem.DTO;
public class TicketDTO {
    [DisplayName("Ticket Number:")]
    public int Id { get; set; }
    [DisplayName("Problem Description:")]
    public string ProblemDescription { get; set; }
    [DisplayName("Created Date:")]
    public DateTime CreatedDate { get; set; }
    public Priority Priority { get; set; }
    [DisplayName("Report Source:")]
    public ReportSource ReportSource { get; set; }
    [DisplayName("Current State:")]
    public TicketState TicketState { get; set; }
    public string? CreatedByUserId { get; set; }
    [DisplayName("Created By User:")]
    public string? CreatedByUserName { get; set; }
    public string? CreatedByRoleId { get; set; }
    [DisplayName("Created By Role:")]
    public string? CreatedByRoleName { get; set; }
    [DisplayName("Ticket Closed:")]
    public bool ClosedFlag { get; set; }
    [DisplayName("Closed Date:")]
    public DateTime? ClosedDate { get; set; }
    public string? ClosedByUserId { get; set; }
    [DisplayName("Closed By User:")]
    public string? ClosedByUserName { get; set; }
    public string? ClosedByRoleId { get; set; }
    [DisplayName("Closed By Role:")]
    public string? ClosedByRoleName { get; set; }
    [DisplayName("Last Update Date:")]
    public DateTime LastModifiedDate { get; set; }
    [DisplayName("Current Ticket Time:")]
    public int OutageTime { get; set; }
    public int CurrentActivityNumber { get; set; }
    public string? LastModifiedByUserId { get; set; }
    [DisplayName("Last Modified By User:")]
    public string? LastModifiedByUserName { get; set; }
    public string? LastModifiedByRoleId { get; set; }
    [DisplayName("Last Modified By Role:")]
    public string? LastModifiedByRoleName { get; set; }
    [DisplayName("Customer Name:")]
    public string? CustomerName { get; set; }
    [DisplayName("Latest Status Comment:")]
    public string? LatestStatusComment { get; set; }
    public string? DisplayOutageTime {
        get {
            return $"{OutageTime / 60:00}:{OutageTime % 60:00}";
        }
    }
    public string? ReportSourceName { get; set; }
    public string? TicketStateName { get; set; }
    public List<TicketActivity>? TicketActivities { get; set; }
}
