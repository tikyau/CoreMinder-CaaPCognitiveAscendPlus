using System;
using System.ComponentModel.DataAnnotations;

namespace Site.Areas.Setup.Models
{
	public class SetupViewModel
	{
		[Display(Name = "Organization Service URL")]
		[DataType(DataType.Url)]
		[Required]
		public Uri OrganizationServiceUrl { get; set; }

		[Display(Name = "Username")]
		[Required]
		public string Username { get; set; }

		[Display(Name = "Password")]
		[DataType(DataType.Password)]
		[Required]
		public string Password { get; set; }

		[Display(Name = "Website")]
		[UIHint("DropDownList")]
		[Required]
		public Guid Website { get; set; }
	}
}