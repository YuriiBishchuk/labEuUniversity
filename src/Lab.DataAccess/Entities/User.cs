using System.ComponentModel.DataAnnotations;

namespace Lab.DataAccess.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        [MaxLength]
        public string? Token { get; set; }
        public string? Role { get; set; }
        public string? Email { get; set; }
        [MaxLength]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public List<ToDo> ToDos { get; set; }
    }
}
