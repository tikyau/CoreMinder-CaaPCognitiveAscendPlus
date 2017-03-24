using System.Collections.Generic;
using Adxstudio.Xrm.Products;

namespace Site.Areas.Products.ViewModels
{
	public class ProductImageGalleryViewModel
	{
		public List<IProductImageGalleryNode> ImageGalleryNodes { get; set; }

		public IProduct Product { get; set; }
	}
}