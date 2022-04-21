namespace Beis.HelpToGrow.Web.Models.Pricing
{
    using System.Collections.Generic;

    public class MetricDetailsViewModel
    {
        public long ProductPriceId { get; set; }

        public long ProductId { get; set; }

        public string Adb2CId { get; set; }

        public string ProductName { get; set; }
        
        public IList<PrimaryMetricDetailsViewModel> PrimaryMetricDetails{ get; set; }
        
        public IList<SecondaryMetricDetailsViewModel> SecondaryMetricDetails { get; set; }
    }

    public class PrimaryMetricDetailsViewModel
    {
        public string Description { get; set; }

        public decimal Amount { get; set; }
        
        public decimal PricePercentage { get; set; }
        
        public int NumberOfUsers{ get; set; }
    }

    public class SecondaryMetricDetailsViewModel
    {
        public string Description { get; set; }
        
        public decimal MetricNumber { get; set; }
        
        public string MetricUnit{ get; set; }
    }
}