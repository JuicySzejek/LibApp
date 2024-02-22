using LibApp.Models;
using System.ComponentModel.DataAnnotations;

namespace LibApp.ViewModels
{
  
    public class CustomerViewModel
    {
        public User User { get; set; }
        public IEnumerable<MembershipType> ?MembershipTypes { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Email doesnt match with requirements")]
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
    
}
