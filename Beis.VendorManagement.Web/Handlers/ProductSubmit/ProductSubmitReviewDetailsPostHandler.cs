namespace Beis.VendorManagement.Web.Handlers.ProductSubmit
{
    public class ProductSubmitReviewDetailsPostHandler : IRequestHandler<ProductSubmitReviewDetailsPostHandler.Context, string>
    {
        private readonly IProductRepository _productRepository;
        private readonly ICompanyUserRepository _companyUserRepository;
        private readonly IManageUsersRepository _manageUsersRepository;
        private readonly INotifyService _notifyService;
        private readonly ProductSubmitReviewDetailsPostHandlerOptions _options;

        public ProductSubmitReviewDetailsPostHandler(
            IProductRepository productRepository,
            ICompanyUserRepository companyUserRepository,
            IManageUsersRepository manageUsersRepository,
            INotifyService notifyService,
            IOptions<ProductSubmitReviewDetailsPostHandlerOptions> options)
        {
            _productRepository = productRepository;
            _companyUserRepository = companyUserRepository;
            _manageUsersRepository = manageUsersRepository;
            _notifyService = notifyService;
            _options = options.Value;
        }

        public async Task<string> Handle(Context request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductSingle(request.ProductSubmitReviewDetails.ProductId, request.Adb2CId);
            if (product != null)
            {
                product.status = (int)ProductStatus.InReview;
                await _productRepository.UpdateProduct(product);
            }

            var currentUser = await _companyUserRepository.GetUserIdByAdb2CUserId(request.Adb2CId);
            
            var emailAddresses = new List<string>
            {
                currentUser.email
            };

            if (!currentUser.primary_contact)
            {
                var primaryUser = await _manageUsersRepository.GetPrimaryUserAsync(currentUser.companyid);
                if (primaryUser != null)
                {
                    emailAddresses.Add(primaryUser.email);
                }
            }

            if (!string.IsNullOrWhiteSpace(_options.IdsEmail))
            {
                emailAddresses.Add(_options.IdsEmail);
            }
            
            await _notifyService.SendProductSubmittedConfirmationEmail(
                0, emailAddresses, _options.VendorProductSubmittedConfirmTemplateId, product?.product_name, currentUser.full_name, currentUser.companyid);

            return currentUser.email;
        }

        public class ProductSubmitReviewDetailsPostHandlerOptions
        {
            public string IdsEmail { get; set; }

            public string VendorProductSubmittedConfirmTemplateId { get; set; }
        }

        public struct Context : IRequest<string>
        {
            public string Adb2CId { get; set; }

            public ProductSubmitReviewDetailsViewModel ProductSubmitReviewDetails { get; set; }
        }
    }
}