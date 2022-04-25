﻿namespace Beis.HelpToGrow.Web.Models.Pricing
{
    public class FreeTrialViewModel
    {
        public long ProductPriceId { get; set; }

        public long ProductId { get; set; }

        public string Adb2CId { get; set; }

        public string ProductName { get; set; }

        public bool FreeTrialFlag { get; set; }

        public int FreeTrialTermNo { get; set; }

        public string FreeTrialTermUnit { get; set; }

        public bool FreeTrialPaymentUpfront { get; set; }

        public string FreeTrialEndActionDescription { get; set; }
    }
}