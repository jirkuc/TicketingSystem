using TicketingSystem.Enums;
using TicketingSystem.Models;

namespace TicketingSystem.DTO; 
public class TicketActivityDTO {
    public int Id { get; set; }
    public int TicketId { get; set; }
    public int ActivityNumber { get; set; }
    public DateTime ActivityDate { get; set; }
    public string ActivityName { get; set; }
    public string StatusComment { get; set; }
    public string ActivityUserId { get; set; }
    public string ActivityUserName { get; set; }
    public string ActivityRoleId { get; set; }
    public string ActivityRoleName { get; set; }
    public int CurrentOutageTime { get; set; }
    public TicketState OldTicketState { get; set; }
    public TicketState NewTicketState { get; set; }
    public InternalFlag InternalFlag { get; set; }
}
