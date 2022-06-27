namespace Beis.VendorManagement.Web.Extensions
{
	internal static class AdditionalCostExtensions
	{
		internal static EnumAdditionalCostType GetAdditionalCostType(this additional_cost additionalCostDetail)
        {
            return !Enum.IsDefined(typeof(EnumAdditionalCostType), additionalCostDetail.additional_cost_type_id)
                ? EnumAdditionalCostType.None
                : (EnumAdditionalCostType)additionalCostDetail.additional_cost_type_id;
        }

        public static IEnumerable<AdditionalCostDetail> FilterAdditionalCosts(this AdditionalCostsViewModel additionalCostsViewModel, EnumAdditionalCostType additionalCostType)
        {
            return additionalCostsViewModel.AdditionalCosts
                .Where(x => x.AdditionalCostType == additionalCostType)
                .ToList(); // View enumerates this several times
        }
    }
}