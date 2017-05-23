using System.ComponentModel.DataAnnotations;

namespace chatbot_iSAS.Models
{
    public class User
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}