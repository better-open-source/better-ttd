using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer.DataAccess.Entities
{
    [Table("UserLogins")]
    public class UserLogin : Entity<long>
    {
        [MaxLength(50)]
        [Required]
        public string SubjectId { get; set; }

        [Required]
        [MaxLength(250)]
        public string LoginProvider { get; set; }

        [Required]
        [MaxLength(250)]
        public string ProviderKey { get; set; }
    }
}