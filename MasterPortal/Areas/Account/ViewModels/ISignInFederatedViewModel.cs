using System.Collections.Generic;
using System.Web.Mvc;

namespace Site.Areas.Account.ViewModels
{
	public interface ISignInFederatedViewModel
	{
		ModelErrorCollection Errors { get; }
		IEnumerable<IdentityProviderViewModel> IdentityProviders { get; set; }
		string IdentityProviderName { get; set; }
		string HomeRealmDiscoveryMetadataFeedUrl { get; set; }
	}
}
