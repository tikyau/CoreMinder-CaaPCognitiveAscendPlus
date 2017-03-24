using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Adxstudio.Xrm.Account;

namespace Site.Areas.Account.ViewModels
{
	public class RedeemViewModel : SignUpLocalViewModel, ISignInFederatedViewModel
	{
		private readonly Lazy<ModelErrorCollection> _errors = new Lazy<ModelErrorCollection>(() => new ModelErrorCollection());

		public ModelErrorCollection Errors
		{
			get { return _errors.Value; }
		}

		public IEnumerable<IdentityProviderViewModel> IdentityProviders { get; set; }
		public string IdentityProviderName { get; set; }
		public string HomeRealmDiscoveryMetadataFeedUrl { get; set; }

		public string InvitationCode { get; set; }
		public InvitationType InvitationType { get; set; }
		public bool Redeemed { get; set; }
		public bool UserExists { get; set; }
		public string SignInUsername { get; set; }
		public string SignInPassword { get; set; }
		public string ChallengeQuestion { get; set; }
		public string ChallengeAnswer { get; set; }

		public string wa { get; set; }
		public string openauthresult { get; set; }
		public string wresult { get; set; }
	}
}
