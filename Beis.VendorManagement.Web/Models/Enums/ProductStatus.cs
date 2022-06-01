namespace Beis.VendorManagement.Web.Models.Enums
{
    public enum ProductStatus
    {
        [EnumDisplayName(DisplayName = "Incomplete")]
        Incomplete = 1,

        [EnumDisplayName(DisplayName = "In Review")]
        InReview = 10,
        
        [EnumDisplayName(DisplayName = "Approved")]
        Approved = 50,
        
        [EnumDisplayName(DisplayName = "Not In Scheme")]
        NotInScheme = 1000
    }
}