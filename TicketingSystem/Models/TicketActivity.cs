using TicketingSystem.Enums;

namespace TicketingSystem.Models;
public class TicketActivity {
    public int Id { get; set; }
    public Ticket Ticket { get; set; }
    public int TicketId { get; set; }
    public int ActivityNumber { get; set; }
    public DateTime ActivityDate { get; set; }
    public string ActivityName { get; set; }
    public string StatusComment { get; set; }
    public AppUser ActivityUser { get; set; }
    public string ActivityUserId { get; set; }
    public string ActivityUserName { get; set; }
    public AppRole ActivityRole { get; set; }
    public string ActivityRoleId { get; set; }
    public string ActivityRoleName { get; set; }
    public int CurrentOutageTime { get; set; }
    public TicketState OldTicketState { get; set; }
    public string? OldTicketStateName { get; set; }
    public TicketState NewTicketState { get; set; }
    public string? NewTicketStateName { get; set; }
    public InternalFlag InternalFlag { get; set; }
    public string? DisplayOutageTime {
        get {
            return $"{CurrentOutageTime / 60:00}:{CurrentOutageTime % 60:00}";
        }
    }
}
