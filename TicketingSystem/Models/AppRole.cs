using Microsoft.AspNetCore.Identity;

namespace TicketingSystem.Models {
    public class AppRole : IdentityRole {
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
