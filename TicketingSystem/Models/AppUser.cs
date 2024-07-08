using Microsoft.AspNetCore.Identity;

namespace TicketingSystem.Models {
    public class AppUser : IdentityUser {
        
        public string DefaultRoleId { get; set; }
        public AppRole DefaultRole { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
     
    }
}
