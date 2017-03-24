using System;
using System.Web.UI.WebControls;
using Microsoft.Xrm.Portal.Web.UI.WebControls;
using Recaptcha;
using Site.Pages;

namespace Site.Areas.ContactUs.Pages
{
    public partial class ContactUs : PortalPage
    {
        private RecaptchaControl _recaptchaImage;

        public RecaptchaControl RecaptchaImage
        {
            get { return _recaptchaImage ?? (_recaptchaImage = FormView.FindControl("RecaptchaImage") as RecaptchaControl); }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            RecaptchaImage.Enabled = RecaptchaImage.Visible = RecaptchaImage.PublicKey != "null" && RecaptchaImage.PrivateKey != "null";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void OnItemInserted(object sender, CrmEntityFormViewInsertedEventArgs e)
        {
            FormView.Visible = false;
            ConfirmationMessage.Visible = true;
        }

        protected void RecaptchaValidator_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (RecaptchaImage != null)
            {
                RecaptchaImage.Validate(); // initializes the IsValid property
                e.IsValid = RecaptchaImage.IsValid;
            }
        }
    }
}