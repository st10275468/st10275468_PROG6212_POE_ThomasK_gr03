/*  OpenAI.2024. Chat-GPT(Version 3.5).[Large language model]. Available at: https://chat.openai.com/[Accessed: 20 November 2024]. */
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

        /// <summary>
        /// Method created that automatically checks the claims against certain criteria. If a claim fails verification it can still be manually verified
        /// </summary>
        public void VerifyClaim()
        {
            DateTime claimMonthDay = new DateTime(claimMonth.Year, claimMonth.Month, 15);

            int days = (DateTime.Now - claimMonthDay).Days;

            if (claimAmount > 9000 && claimAmount < 25000 && days <= 45) {
               
                claimVerification = "Verified";

            }
            else
            {
                claimVerification = "Failed - Under Review";
            }
           
            
        }
    }
}
