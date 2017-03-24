using System;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Adxstudio.Xrm.Blogs;
using Adxstudio.Xrm.Cms;
using Microsoft.Xrm.Portal;
using Microsoft.Xrm.Portal.Configuration;
using Microsoft.Xrm.Portal.Web;
using Microsoft.Xrm.Sdk;
using Site.MasterPages;
using OrganizationServiceContextExtensions = Adxstudio.Xrm.Cms.OrganizationServiceContextExtensions;
using PortalContextDataAdapterDependencies = Adxstudio.Xrm.Blogs.PortalContextDataAdapterDependencies;

namespace Site.Areas.Blogs.MasterPages
{
	public partial class Blogs : PortalMasterPage
	{
		private readonly Lazy<IPortalContext> _portal = new Lazy<IPortalContext>(() => PortalCrmConfigurationManager.CreatePortalContext(), LazyThreadSafetyMode.None);

		private const string _blogQueryStringField = "blog";
		private const string _searchQueryStringField = "q";

		protected bool SearchIsVisible
		{
			get
			{
				var settingDataAdapter = new SettingDataAdapter(new PortalContextDataAdapterDependencies(_portal.Value, requestContext: Request.RequestContext));
				return settingDataAdapter.GetBooleanValue("blogs/displaySearch") ?? true;
			}
		}

		protected void Page_Load(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
				return;
			}

			Guid blogId;

			BlogSearchFilters.Visible = TryGetBlogScope(out blogId);

			BlogSearch.Visible = SearchIsVisible;

			var q = Request.QueryString[_searchQueryStringField];

			if (!string.IsNullOrEmpty(q))
			{
				QueryText.Text = q;
			}

			QueryText.Attributes["onkeydown"] = OnEnterKeyDownThenClick(SearchButton);
		}

		protected void SearchButton_Click(object sender, EventArgs e)
		{
			var serviceContext = _portal.Value.ServiceContext;

			var searchPage = OrganizationServiceContextExtensions.GetPageBySiteMarkerName(serviceContext, _portal.Value.Website, "Blog Search");

			if (searchPage == null)
			{
				return;
			}

			var url = OrganizationServiceContextExtensions.GetUrl(serviceContext, searchPage);

			if (url == null)
			{
				return;
			}

			var urlBuilder = new UrlBuilder(url);

			urlBuilder.QueryString.Set(_searchQueryStringField, QueryText.Text);

			Guid blogId;

			if (BlogSearchFilters.Visible && BlogSearchFilterOptions.SelectedValue == "blog" && TryGetBlogScope(out blogId))
			{
				urlBuilder.QueryString.Set(_blogQueryStringField, blogId.ToString());
			}

			Response.Redirect(urlBuilder.PathWithQueryString);
		}

		private static string OnEnterKeyDownThenClick(Control button)
		{
			if (button == null) return string.Empty;

			return string.Format(@"
				if(!event.ctrlKey && !event.shiftKey && event.keyCode == 13) {{
					document.getElementById('{0}').click();
					return false;
				}}
				return true; ",
				button.ClientID);
		}

		protected void CreateBlogDataAdapter(object sender, ObjectDataSourceEventArgs e)
		{
			Guid blogId;

			e.ObjectInstance = TryGetBlogIdFromQueryString(out blogId)
				? new BlogDataAdapter(new EntityReference("adx_blog", blogId), new PortalContextDataAdapterDependencies(_portal.Value, requestContext: Request.RequestContext))
				: new SiteMapNodeBlogDataAdapter(System.Web.SiteMap.CurrentNode, new PortalContextDataAdapterDependencies(_portal.Value, requestContext: Request.RequestContext)) as IBlogDataAdapter;
		}

		private bool TryGetBlogIdFromQueryString(out Guid blogId)
		{
			blogId = default(Guid);

			var blogQuery = Request.QueryString[_blogQueryStringField];

			if (string.IsNullOrEmpty(blogQuery))
			{
				return false;
			}

			return Guid.TryParse(blogQuery, out blogId);
		}

		private bool TryGetBlogScope(out Guid blogId)
		{
			if (TryGetBlogIdFromQueryString(out blogId))
			{
				return true;
			}

			var entity = _portal.Value.Entity;

			if (entity == null)
			{
				return false;
			}

			if (entity.LogicalName == "adx_blog")
			{
				blogId = entity.Id;

				return true;
			}

			if (entity.LogicalName == "adx_blogpost")
			{
				var blogReference = _portal.Value.Entity.GetAttributeValue<EntityReference>("adx_blogid");

				if (blogReference != null)
				{
					blogId = blogReference.Id;

					return true;
				}
			}

			return false;
		}
	}
}