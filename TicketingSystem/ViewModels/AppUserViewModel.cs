using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TicketingSystem.Models;

namespace TicketingSystem.ViewModels;
public class AppUserViewModel {
    private string password;

    [DisplayName("User Name:")]
    public string UserName { get; set; }
    [DisplayName("Email:")]
    [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
    public string Email { get; set; }
    [DisplayName("Password:")]
    public string Password { get; set; }

    [DisplayName("Default Role:")]
    public string DefaultRoleId { get; set; }
    [DisplayName("Default Role Name:")]
    public string? DefaultRoleName { get; set; }
    [DisplayName("Phone Number:")]
    public string? Id { get; set; }
    public string? NormalizedUserName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? CreateDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string? ConcurrencyStamp { get; set; }
}
