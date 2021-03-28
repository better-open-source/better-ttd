using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer.DataAccess.Entities
{
    [Table("Claims")]
    public class UserClaim : Entity<long>
    {         
        [MaxLength(50)]
        [Required]
        public string SubjectId { get; set; }

        [Required]
        [MaxLength(250)]
        public string ClaimType { get; set; }

        [Required]
        [MaxLength(250)]
        public string ClaimValue { get; set; }

        public UserClaim(string claimType, string claimValue)
        {
            ClaimType = claimType;
            ClaimValue = claimValue;
        }

        // ReSharper disable once UnusedMember.Global
        public UserClaim()
        {
            // leave empty
        }
    }
}