namespace st10275468_PROG6212_POE_ThomasK_gr03.Models
{
    public class Document
    {
        public int documentID {  get; set; }

        public string path { get; set; }

        public int claimID { get; set; }
        public Claim Claim { get; set; }

        public int userID {  get; set; }
        public User User { get; set; }



    }
}
