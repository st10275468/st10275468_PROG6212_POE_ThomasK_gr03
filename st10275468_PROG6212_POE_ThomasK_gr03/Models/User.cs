using System.ComponentModel.DataAnnotations;

namespace st10275468_PROG6212_POE_ThomasK_gr03.Models
{
    public class User
    {
        [Key]
        public int userID {  get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string surname {  get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        public string role { get; set; }

        public ICollection<Claim> Claims { get; set; } = new List<Claim>();

    }
}
