using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.ViewModels; 
public class LoginViewModel {
    [Required]
    [DisplayName("User Name:")]
    public string UserName { get; set; }
    [Required] 
    public string Password { get; set; }
    public string ReturnUrl { get; set; }
    public bool RememberMe { get; set;}

}
