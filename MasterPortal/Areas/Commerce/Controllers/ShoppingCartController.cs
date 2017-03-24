using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;
using Adxstudio.Xrm.Commerce;
using Adxstudio.Xrm.Web.Mvc;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace Site.Areas.Commerce.Controllers
{
	public class ShoppingCartController : Controller
	{
		[NoCache]
		public ActionResult Status()
		{
			var dataAdapterDependencies = new PortalConfigurationDataAdapterDependencies(requestContext: Request.RequestContext);

			// Check for existence of shopping cart schema, and return empty cart if not found.
			try
			{
				using (var serviceContext = dataAdapterDependencies.GetServiceContext())
				{
					serviceContext.Execute(new RetrieveEntityRequest
					{
						LogicalName = "adx_shoppingcart",
						EntityFilters = EntityFilters.Entity
					});
				}
			}
			catch (FaultException<OrganizationServiceFault>)
			{
				return ShoppingCart(0);
			}
			
			var dataAdapter = new ShoppingCartDataAdapter(dataAdapterDependencies, HttpContext.Profile.UserName);

			var cart = dataAdapter.SelectCart();

			if (cart == null)
			{
				return ShoppingCart(0);
			}

			var cartItems = cart.GetCartItems().ToArray();

			return ShoppingCart(cartItems.Sum(item => item.Quantity));
		}

		public JsonResult ShoppingCart(decimal count)
		{
			return Json(new ShoppingCartStatus { Count = count }, JsonRequestBehavior.AllowGet);
		}

		public class ShoppingCartStatus
		{
			public decimal Count { get; set; }
		}
	}
}
