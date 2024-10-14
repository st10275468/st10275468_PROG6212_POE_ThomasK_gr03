namespace st10275468_PROG6212_POE_ThomasK_gr03.Models
{
    public class User
    {
        public int userID {  get; set; }

        public string name { get; set; }

        public string surname {  get; set; }

        public string email { get; set; }

        public string password { get; set; }

        public string role { get; set; }

        public ICollection<Claim> Claims { get; set; }

    }
}
