using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Site.Areas.Account.ViewModels
{
	public class SignInFederatedViewModel : ISignInFederatedViewModel
	{
		private readonly Lazy<ModelErrorCollection> _errors = new Lazy<ModelErrorCollection>(() => new ModelErrorCollection());

		public ModelErrorCollection Errors
		{
			get { return _errors.Value; }
		}

		public IEnumerable<IdentityProviderViewModel> IdentityProviders { get; set; }

		public string IdentityProviderName { get; set; }

		public string HomeRealmDiscoveryMetadataFeedUrl { get; set; }
	}
}
