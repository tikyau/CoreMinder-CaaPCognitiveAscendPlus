using System;
using System.Web.UI.WebControls;
using Adxstudio.Xrm.Cms;

namespace Site.Controls
{
	public partial class Comments : PortalUserControl
	{
		//these properties may or may not be used after ratings have been refactored.

		//included to provide backwards-compatability with existing settings.
		protected bool EnableComments
		{
			get
			{
				if (Entity.LogicalName == "adx_webpage")
				{
					var currentWebPage = Entity;
					return currentWebPage != null && currentWebPage.GetAttributeValue<bool?>("adx_enablecomments").GetValueOrDefault(false);
				}
				return false;
			}
		}
		
		protected string VisitorID
		{
			get { return Context.Profile.UserName; }
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
				CommentsView.DataBind();
			}

			var commentAdapterFactory = new CommentDataAdapterFactory(Entity.ToEntityReference());

			var dataAdapter = commentAdapterFactory.GetAdapter(Portal, Request.RequestContext);

			if (dataAdapter == null)
			{
				Visible = false;

				return;
			}

			var commentPolicyReader = dataAdapter.GetCommentPolicyReader();

			CommentsView.Visible = (!commentPolicyReader.IsCommentPolicyNone) || EnableComments;

			NewCommentPanel.Visible = CommentsOpenToCurrentUser(commentPolicyReader);
		}

		protected void CreateCommentDataAdapter(object sender, ObjectDataSourceEventArgs e)
		{
			var commentAdapterFactory = new CommentDataAdapterFactory(Entity.ToEntityReference());

			e.ObjectInstance = commentAdapterFactory.GetAdapter(Portal, Request.RequestContext);
		}

		protected bool CommentsOpenToCurrentUser(ICommentPolicyReader policyReader)
		{
			if (policyReader.IsCommentPolicyOpen || policyReader.IsCommentPolicyModerated)
			{
				return true;
			}

			//to maintain compatibilty
			return Context.User != null
				&& Context.User.Identity.IsAuthenticated
				&& (policyReader.IsCommentPolicyOpenToAuthenticatedUsers) || EnableComments;
		}

		protected bool CommentsRequireAuthorInfo()
		{
			return Portal.User == null || Portal.User.LogicalName != "contact";
		}

	}
}
