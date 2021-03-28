using System.ComponentModel.DataAnnotations;

namespace IdentityServer.DataAccess.Entities
{
    public abstract class Entity<T>
    {
        [Key]
        public T Id { get; set; }
    }
}