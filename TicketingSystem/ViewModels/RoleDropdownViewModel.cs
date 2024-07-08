using TicketingSystem.Models;

namespace TicketingSystem.ViewModels;
public class RoleDropdownViewModel {
    public IEnumerable<AppRole> Roles { get; set; }

    public RoleDropdownViewModel() {
        Roles = new List<AppRole>();
    }
}

