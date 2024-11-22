using Konnect.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace Konnect.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string Name { get; set; }    
        public bool IsBlocked { get; set; } = false;
        public DateTime LastSeen { get; set; } = DateTime.Parse(DateTimeFormat.FormatString(DateTime.Now));

        [NotMapped]
        public string Interval { get; set; }
        // Index attribute ensures uniqueness
        [Index(IsUnique = true)]
        public override string Email { get; set; }
    }
}
