namespace st10275468_PROG6212_POE_ThomasK_gr03.Models
{
    public class Claim
    {
        public int claimID {  get; set; }

        public decimal claimAmount { get; set; }

        public DateTime claimMonth { get; set; }

        public DateTime submissionDate { get; set; }

       public string claimStatus {  get; set; }

        public int userID {  get; set; }
        public User User { get; set; }

        public ICollection<Document> Documents { get; set; }

    }
}
