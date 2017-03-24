using Adxstudio.Xrm.Products;

namespace Site.Areas.Products.ViewModels
{
	public class ProductViewModel
	{
		public ProductImageGalleryViewModel ImageGalleryNodes { get; set; }

		public IProduct Product { get; set; }

		public bool UserHasReviewed { get; set; }
	}
}