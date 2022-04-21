namespace Beis.HelpToGrow.Web.Models
{
    public class RedemptionUrlViewModel
    {
        public long ProductId { get; set; }

        //public string Adb2CId { get; set; }

        public string RedemptionUrl { get; set; }

        public string ProductName { get; set; }

        public bool ShowValidationError { get; set; }
    }
}