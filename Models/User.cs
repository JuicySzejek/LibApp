using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LibApp.Models
{
    public class User : IdentityUser
    { 
        [Required(ErrorMessage ="Please enter customer's name")]
        [StringLength(100)]
        public string Name { get; set; }
        public string? LastName { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime ?Birthdate { get; set; }

        [Display(Name = "Membership Type")]
        public int MembershipTypeId { get; set; }
        public MembershipType ?MembershipType { get; set; }
        public bool HasNewsletterSubscribed { get; set; }
    }
}
