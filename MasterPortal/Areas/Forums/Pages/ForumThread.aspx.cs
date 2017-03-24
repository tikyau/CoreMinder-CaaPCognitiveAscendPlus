using System;
using System.IO;
using System.Security;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Adxstudio.Xrm.Forums;
using Microsoft.Xrm.Client;
using Microsoft.Xrm.Portal;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Site.Pages;

namespace Site.Areas.Forums.Pages
{
	public partial class ForumThread : PortalPage
	{
		private readonly Lazy<IPortalContext> _portal = new Lazy<IPortalContext>(() => PortalCrmConfigurationManager.CreatePortalContext(), LazyThreadSafetyMode.None);

		protected void Page_Load(object sender, EventArgs e)
		{
			ForumControls.Visible = ForumPostCreateForm.Visible = Request.IsAuthenticated;
			
			AnonymousMessage.Visible = !Request.IsAuthenticated;

			ForumLockedPanel.Visible = false;

			if (Request.IsAuthenticated && _portal.Value.User != null)
			{
				var dataAdapter = CreateForumThreadDataAdapter();

				var hasAlert = dataAdapter.HasAlert(_portal.Value.User.ToEntityReference());

				var locked = dataAdapter.Select().Locked;

				AddAlert.Visible = !hasAlert;
				RemoveAlert.Visible = hasAlert;

				ForumControls.Visible = !locked;
				ForumPostCreateForm.Visible = !locked;
				LockedSnippet.Visible = locked;

				ForumLockedPanel.Visible = locked;
			}
		}

		protected void CreateForumThreadDataAdapter(object sender, ObjectDataSourceEventArgs args)
		{
			args.ObjectInstance = CreateForumThreadDataAdapter();
		}

		protected void CreatePost_Click(object sender, EventArgs args)
		{
			if (!Page.IsValid) return;

			var post = CreatePost();

			Response.Redirect(post.Url);
		}

		private IForumPost CreatePost()
		{
			var dataAdapter = CreateForumThreadDataAdapter();

			var postedOn = DateTime.UtcNow;
			var author = new ForumAuthorReference(_portal.Value.User.ToEntityReference());
			var thread = dataAdapter.Select();

			var postSubmission = new HtmlForumPostSubmission(string.Format("RE: {0}", thread.Name), NewForumPostContent.Text, postedOn, author);

			if (NewForumPostAttachment.HasFiles)
			{
				foreach (var postedFile in NewForumPostAttachment.PostedFiles)
				{
					using (var reader = new BinaryReader(postedFile.InputStream))
					{
						postSubmission.Attachments.Add(new ForumPostAttachment(
							postedFile.FileName,
							postedFile.ContentType,
							reader.ReadBytes(postedFile.ContentLength)));
					}
				}
			}

			return dataAdapter.CreatePost(postSubmission);
		}

		private IForumThreadDataAdapter CreateForumThreadDataAdapter()
		{
			return new ForumThreadDataAdapter(_portal.Value.Entity.ToEntityReference(), new PortalContextDataAdapterDependencies(_portal.Value, requestContext:Request.RequestContext));
		}

		protected void AddAlert_Click(object sender, EventArgs e)
		{
			if (!Request.IsAuthenticated)
			{
				return;
			}

			var user = _portal.Value.User;

			if (user == null)
			{
				return;
			}

			var dataAdapter = CreateForumThreadDataAdapter();

			dataAdapter.CreateAlert(user.ToEntityReference());

			Response.Redirect(Request.Url.PathAndQuery);
		}

		protected void RemoveAlert_Click(object sender, EventArgs e)
		{
			if (!Request.IsAuthenticated)
			{
				return;
			}

			var user = _portal.Value.User;

			if (user == null)
			{
				return;
			}

			var dataAdapter = CreateForumThreadDataAdapter();

			dataAdapter.DeleteAlert(user.ToEntityReference());

			Response.Redirect(Request.Url.PathAndQuery);
		}

		protected void MarkAsAnswer_OnCommand(object sender, CommandEventArgs e)
		{
			var forumPostId = Guid.Parse(e.CommandArgument.ToString());

			var dataAdapter = CreateForumThreadDataAdapter();

			dataAdapter.MarkAsAnswer(forumPostId);

			Response.Redirect(Request.Url.PathAndQuery);
		}

		protected void UnmarkAsAnswer_OnCommand(object sender, CommandEventArgs e)
		{
			var forumPostId = Guid.Parse(e.CommandArgument.ToString());

			var dataAdapter = CreateForumThreadDataAdapter();

			dataAdapter.UnMarkAsAnswer(forumPostId);

			Response.Redirect(Request.Url.PathAndQuery);
		}

		protected void UpdatePost_OnCommand(object sender, CommandEventArgs e)
		{
			var forumPostId = Guid.Parse(e.CommandArgument.ToString());
			var dataAdapter = CreateForumThreadDataAdapter();
			var forumPost = dataAdapter.SelectPost(forumPostId);

			if (forumPost == null)
			{
				throw new InvalidOperationException("Unable to retrieve control with ID {0}.".FormatWith(forumPostId));
			}

			if (!forumPost.CanEdit)
			{
				throw new SecurityException("The current user does not have the necessary permissions to update this forum post.");
			}

			var content = ((Control) sender).FindControl("ForumPostContentUpdate") as ITextControl;

			if (content == null)
			{
				throw new InvalidOperationException("Unable to find control ForumPostContentUpdate.");
			}

			dataAdapter.UpdatePost(new HtmlForumPostUpdate(((IForumPostInfo)forumPost).EntityReference, htmlContent: content.Text));

			Response.Redirect(Request.Url.PathAndQuery);
		}

		protected void ForumPosts_DataBound(object sender, EventArgs args)
		{
			var pager = ForumPosts.FindControl("ForumPostsPager") as DataPager;

			if (pager == null)
			{
				return;
			}

			pager.Visible = pager.PageSize < pager.TotalRowCount;
		}

		protected void ValidatePostContentLength(object source, ServerValidateEventArgs args)
		{
			var response = (RetrieveAttributeResponse) ServiceContext.Execute(new RetrieveAttributeRequest
			{
				EntityLogicalName = "adx_communityforumpost",
				LogicalName = "adx_content"
			});

			const int defaultMaxLength = 65536;

			var metadata = response.AttributeMetadata as MemoAttributeMetadata;
			var maxLength = metadata == null ? defaultMaxLength : metadata.MaxLength.GetValueOrDefault(defaultMaxLength);
			
			args.IsValid = args.Value == null || args.Value.Length <= maxLength;
		}
	}
}