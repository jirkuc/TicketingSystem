using TicketingSystem.Models;

namespace TicketingSystem.ViewModels;
public class AppUserDropdownViewModel {
    public IEnumerable<AppUser> Users { get; set; }

    public AppUserDropdownViewModel() {
        Users = new List<AppUser>();
    }
}

