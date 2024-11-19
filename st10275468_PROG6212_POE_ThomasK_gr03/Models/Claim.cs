namespace st10275468_PROG6212_POE_ThomasK_gr03.Models
{
    public class Claim
    {
        public int claimID {  get; set; }

        public decimal claimAmount { get; set; }

        public DateTime claimMonth { get; set; }

        public DateTime submissionDate { get; set; }

        public string claimVerification { get; set; }
        public string claimStatus {  get; set; }


        public int userID {  get; set; }
        public User User { get; set; }

        public ICollection<Document> Documents { get; set; }

        public void VerifyClaim()
        {
            // Example logic for claim verification (customize as needed)
            if (claimAmount > 1000 && (DateTime.Now - submissionDate).Days <= 30)
            {
                claimVerification = "Verified";
            }
            else
            {
                claimVerification = "Failed Verification";
            }
        }
    }
}
