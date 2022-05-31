namespace Beis.VendorManagement.Web.Models.Pricing
{
    public class MinimumCommitmentViewModel : BaseViewModel
    {
        public long ProductPriceId { get; set; }

        public long ProductId { get; set; }

        public string Adb2CId { get; set; }

        public string ProductName { get; set; }

        public bool CommitmentFlag { get; set; }

        public int CommitmentNo { get; set; }

        public string CommitmentUnit { get; set; }

        public int MinNoUsers { get; set; }
    }
}