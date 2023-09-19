using System.ComponentModel.DataAnnotations;

namespace Scradic.Core.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}