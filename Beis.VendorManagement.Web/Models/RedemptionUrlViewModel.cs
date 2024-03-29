﻿namespace Beis.VendorManagement.Web.Models
{
    public class RedemptionUrlViewModel : BaseViewModel
    {
        public long ProductId { get; set; }

        public string RedemptionUrl { get; set; }

        public string ProductName { get; set; }

        public bool ShowValidationError { get; set; }
    }
}