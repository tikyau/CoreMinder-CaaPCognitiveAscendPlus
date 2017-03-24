using System;
using System.IO;
using System.Threading;
using System.Web.UI.WebControls;
using Adxstudio.Xrm.Forums;
using Adxstudio.Xrm.Web.Mvc.Html;
using Microsoft.Xrm.Portal;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Site.Pages;

namespace Site.Areas.Forums.Pages
{
	public partial class Forum : PortalPage
	{
		private readonly Lazy<IPortalContext> _portal = new Lazy<IPortalContext>(() => PortalCrmConfigurationManager.CreatePortalContext(), LazyThreadSafetyMode.None);

		protected void Page_Load(object sender, EventArgs e)
		{
			ForumControls.Visible = ForumThreadCreateForm.Visible = Request.IsAuthenticated;
			AnonymousMessage.Visible = !Request.IsAuthenticated;
		}

		protected void CreateForumDataAdapter(object sender, ObjectDataSourceEventArgs args)
		{
			args.ObjectInstance = CreateForumDataAdapter();
		}

		protected void CreateThread_Click(object sender, EventArgs args)
		{
			if (!Page.IsValid) return;

			var thread = CreateThread();

			if (NewForumThreadSubscribe.Checked)
			{
				var threadDataAdapter = new ForumThreadDataAdapter(thread, CreateForumDataAdapterDependencies());

				threadDataAdapter.CreateAlert(thread.Author.EntityReference);
			}

			Response.Redirect(thread.Url);
		}

		private IForumThread CreateThread()
		{
			Guid threadTypeId;

			if (!Guid.TryParse(NewForumThreadType.SelectedValue, out threadTypeId))
			{
				throw new InvalidOperationException("Unable to parse forum thread type ID.");
			}

			var postedOn = DateTime.UtcNow;
			var author = new ForumAuthorReference(_portal.Value.User.ToEntityReference());
			var threadType = new ForumThreadTypeReference(new EntityReference("adx_forumthreadtype", threadTypeId));

			var threadSubmission = new ForumThreadSubmission(NewForumThreadName.Text, postedOn, author, threadType);
			var postSubmission = new HtmlForumPostSubmission(NewForumThreadName.Text, NewForumThreadContent.Text, postedOn, author);

			if (NewForumThreadAttachment.HasFiles)
			{
				foreach (var postedFile in NewForumThreadAttachment.PostedFiles)
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

			var dataAdapter = CreateForumDataAdapter();

			return dataAdapter.CreateThread(threadSubmission, postSubmission);
		}

		private IForumDataAdapter CreateForumDataAdapter()
		{
			return new ForumDataAdapter(_portal.Value.Entity.ToEntityReference(), CreateForumDataAdapterDependencies());
		}

		private IDataAdapterDependencies CreateForumDataAdapterDependencies()
		{
			return new PortalContextDataAdapterDependencies(
				_portal.Value,
				new PaginatedLatestPostUrlProvider("page", Html.IntegerSetting("Forums/PostsPerPage").GetValueOrDefault(20)),
				requestContext: Request.RequestContext);
		}

		protected void ForumThreads_DataBound(object sender, EventArgs args)
		{
			var pager = ForumThreads.FindControl("ForumThreadsPager") as DataPager;

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