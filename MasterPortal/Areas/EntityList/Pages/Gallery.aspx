<%@ Page Language="C#" MasterPageFile="~/MasterPages/WebForms.master" AutoEventWireup="true" CodeBehind="Gallery.aspx.cs" Inherits="Site.Areas.EntityList.Pages.Gallery" %>
<%@ OutputCache CacheProfile="User" %>
<%@ Import Namespace="Adxstudio.Xrm.Web.Mvc.Html" %>
<%@ Import Namespace="Site.Areas.EntityList.Helpers" %>

<asp:Content ContentPlaceHolderID="Head" runat="server">
	<link rel="stylesheet" href="<%: Url.Content("~/Areas/EntityList/css/lightbox.css") %>"/>
	<link rel="stylesheet" href="<%: Url.Content("~/Areas/EntityList/css/gallery.css") %>"/>
	<%: Html.PackageRepositoryLink(Url) %>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageHeader" runat="server">
	<div class="page-header">
		<div class="pull-right package-installer">
			<a class="btn btn-success" href="<%: Html.PackageRepositoryInstallUrl(Url) %>"><span class="fa fa-plus-circle" aria-hidden="true"></span> <%: Html.SnippetLiteral("Gallery/PackageRepository/Install") ?? "Install Gallery" %></a>
		</div>
		<h1><%: Html.TextAttribute("adx_name") %></h1>
	</div>
</asp:Content>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
	<%: Html.HtmlAttribute("adx_copy", cssClass: "page-copy") %>
	
	<adx:EntityList ID="EntityListControl" IsGallery="True" ListCssClass="table table-striped" DefaultEmptyListText="There are no items to display." ClientIDMode="Static" runat="server" LanguageCode="<%$ SiteSetting: Language Code, 0 %>" PortalName="<%$ SiteSetting: Language Code %>" />
	
	<script id="gallery-template" type="text/x-handlebars-template">
		<div class="row gallery">
			<div class="col-sm-4">
				<div class="content-panel panel panel-default">
					<div class="list-group gallery-category-list">
						<a class="list-group-item" data-gallery-nav="category" data-gallery-nav-value=""><%: Html.SnippetLiteral("Gallery/PackageRepository/Categories/All") ?? "All" %></a>
						{{#if HasFeatured}}
							<a class="list-group-item {{#iffeatured ActiveCategory}}active{{/iffeatured}}" data-gallery-nav="category" data-gallery-nav-value="Featured"><%: Html.SnippetLiteral("Gallery/PackageRepository/Categories/Featured") ?? "Featured" %></a>
						{{/if}}
					</div>
				</div>
				{{#if NonFeaturedCategories}}
					<div class="content-panel panel panel-default">
						<div class="panel-heading">
							<h4><%: Html.SnippetLiteral("Gallery/PackageRepository/Categories/Header") ?? "Categories" %></h4>
						</div>
						<div class="list-group gallery-category-list">
							{{#each NonFeaturedCategories}}
								<a class="list-group-item {{#ifeq this ../ActiveCategory}}active{{/ifeq}}" data-gallery-nav="category" data-gallery-nav-value="{{this}}">{{this}}</a>
							{{/each}}
						</div>
					</div>
				{{/if}}
				<div class="content-panel panel panel-default">
					<div class="panel-heading">
						<h4><%: Html.SnippetLiteral("Gallery/PackageRepository/RespositoryURL/Header") ?? "Gallery Install URL" %></h4>
					</div>
					<div class="panel-body">
						<div>
							<input id="repository-url" class="repository-url form-control" type="text" value="<%: Html.PackageRepositoryUrl(Url) %>" readonly="readonly" />
							<div class="input-group-btn" style="display: none;">
								<a class="btn btn-default zeroclipboard" data-clipboard-target="repository-url" title="<%: Html.SnippetLiteral("Gallery/PackageRepository/RespositoryURL/CopyToClipboard") ?? "Copy URL to clipboard" %>">
									<span class="fa fa-clipboard" aria-hidden="true"></span>
								</a>
							</div>
						</div>
					</div>
				</div>
			</div>
			<div class="col-sm-8">
				{{#if VisiblePackages}}
					<ul class="list-unstyled gallery-items">
						{{#with FeaturedPackage}}
							<li>
								<div class="media item featured">
									{{#if Icon}}
										<a class="media-left" href="{{URL}}">
											<img class="media-object gallery-icon" src="{{ Icon.URL }}" alt="{{ Icon.Description }}" />
										</a>
									{{else}}
										<a class="media-left" href="{{URL}}">
											<div class="media-object gallery-icon placeholder"></div>
										</a>
									{{/if}}
									<div class="media-body">
										<div class="gallery-item-heading">
											<div class="gallery-item-title">
												<h3 class="name"><a href="{{URL}}">{{DisplayName}}</a></h3>
											</div>
											<div class="metadata">
												<span class="label label-info">{{Version}}</span>
												{{#if IsFeatured}}
													<span class="label label-warning"><%: Html.SnippetLiteral("Gallery/PackageRepository/Categories/Featured") ?? "Featured" %></span>
												{{/if}}
											</div>
										</div>
										<div class="summary">
											{{Summary}}
										</div>
										{{#if Images}}
											<div class="row">
												{{#take3 Images}}
													<div class="col-sm-4">
														<a class="thumbnail" href="{{URL}}" title="{{Description}}" data-lightbox="{{../URI}}"><img src="{{URL}}" alt="{{Description}}" /></a>
													</div>
												{{/take3}}
											</div>
										{{/if}}
										<div class="tags">
											<span class="label label-default">{{PublisherName}}</span>
											{{#each NonFeaturedCategories}}
												<span class="label label-success">{{this}}</span>
											{{/each}}
										</div>
									</div>
								</div>
							</li>
						{{/with}}
						{{#each NonFeaturedPackages}}
							<li>
								<div class="media item">
									{{#if Icon}}
										<a class="media-left" href="{{URL}}">
											<img class="media-object gallery-icon" src="{{ Icon.URL }}" alt="{{ Icon.Description }}" />
										</a>
									{{else}}
										<a class="media-left" href="{{URL}}">
											<div class="media-object gallery-icon placeholder"></div>
										</a>
									{{/if}}
									<div class="media-body">
										<div class="gallery-item-heading">
											<div class="gallery-item-title">
												<h3 class="name"><a href="{{URL}}">{{DisplayName}}</a></h3>
											</div>
											<div class="metadata">
												<span class="label label-info">{{Version}}</span>
												{{#if IsFeatured}}
													<span class="label label-warning"><%: Html.SnippetLiteral("Gallery/PackageRepository/Categories/Featured") ?? "Featured" %></span>
												{{/if}}
											</div>
										</div>
										<div class="summary">
											{{Summary}}
										</div>
										{{#if Images}}
											<div class="row">
												{{#take4 Images}}
													<div class="col-sm-3">
														<a class="thumbnail" href="{{URL}}" title="{{Description}}" data-lightbox="{{../URI}}"><img src="{{URL}}" alt="{{Description}}" /></a>
													</div>
												{{/take4}}
											</div>
										{{/if}}
										<div class="tags">
											<span class="label label-default">{{PublisherName}}</span>
											{{#each NonFeaturedCategories}}
												<span class="label label-success">{{this}}</span>
											{{/each}}
										</div>
									</div>
								</div>
							</li>
						{{/each}}
					</ul>
				{{else}}
					<div class="alert alert-block alert-info">
						<p><%: Html.HtmlSnippet("Gallery/PackageRepository/NoPackages", defaultValue: "No items were found for the current view.") %></p>
					</div>
				{{/if}}
			</div>
		</div>
	</script>
	
	<script id="gallery-error-template" type="text/x-handlebars-template">
		<div class="gallery">
			<div class="alert alert-block alert-danger">
				<p><%: Html.HtmlSnippet("Gallery/PackageRepository/LoadError", defaultValue: "There was an error loading the data for this gallery.") %></p>
			</div>
		</div>
	</script>
</asp:Content>

<asp:Content ContentPlaceHolderID="Scripts" runat="server">
	<script src="<%: Url.Content("~/Areas/EntityList/js/lightbox-2.6.min.js") %>"></script>
	<script src="<%: Url.Content("~/Areas/EntityList/js/ZeroClipboard.min.js") %>"></script>
	<script type="text/javascript">
		ZeroClipboard.config({ moviePath: '<%: Url.Content("~/Areas/EntityList/swf/ZeroClipboard.swf") %>', activeClass: 'active', hoverClass: 'hover' });
	</script>
	<script src="<%: Url.Content("~/Areas/EntityList/js/gallery.js") %>"></script>
</asp:Content>
