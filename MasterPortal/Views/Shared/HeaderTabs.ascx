﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<%@ Import namespace="Adxstudio.Xrm.Web.Mvc.Html" %>
<%@ Import Namespace="DevTrends.MvcDonutCaching" %>

<% var viewSupportsDonuts = ((bool?)ViewBag.ViewSupportsDonuts).GetValueOrDefault(false); %>
<% var relatedWebsites = Html.RelatedWebsites(linkTitleSiteSettingName:"Site Name"); %>
<% var searchEnabled = Html.BooleanSetting("Search/Enabled").GetValueOrDefault(true); %>
<% var searchUrl = searchEnabled ? Html.SiteMarkerUrl("Search") : null; %>
<% var searchFilterOptions = searchEnabled ? Html.SearchFilterOptions().ToArray() : Enumerable.Empty<KeyValuePair<string, string>>().ToArray(); %>
<% var searchFilterDefaultText = searchEnabled ? Html.SnippetLiteral("Default Search Filter Text", "All") : null; %>
<% var searchFilterLabel = searchEnabled ? Html.SnippetLiteral("Header/Search/Filter/Label", "Search Filter") : null; %>
<% var searchLabel = searchEnabled ? Html.SnippetLiteral("Header/Search/Label", "Search") : null; %>
<% var searchToolTip = searchEnabled ? Html.SnippetLiteral("Header/Search/ToolTip", "Search") : null; %>
<% var isAuthenticated = Request.IsAuthenticated; %>
<% var signInUrl = !isAuthenticated ? Html.Action("SignInUrl", "Layout", new { area = "Portal" }, viewSupportsDonuts) : null; %>
<% var signInEnabled = !isAuthenticated && !string.IsNullOrWhiteSpace(Url.SignInUrl()); %>
<% var signInLabel = !isAuthenticated ? Html.SnippetLiteral("links/login", "Sign In") : null; %>
<% var signOutUrl = isAuthenticated ? Html.Action("SignOutUrl", "Layout", new { area = "Portal" }, viewSupportsDonuts) : null; %>
<% var signOutLabel = isAuthenticated ? Html.SnippetLiteral("links/logout", "Sign Out") : null; %>
<% var registrationEnabled = !isAuthenticated && Url.RegistrationEnabled(); %>
<% var registerUrl = registrationEnabled ? Html.Action("RegisterUrl", "Layout", new { area = "Portal" }, viewSupportsDonuts) : null; %>
<% var registerLabel = registrationEnabled ? Html.SnippetLiteral("links/register", "Register") : null; %>

<div class="header-navbar navbar navbar-default navbar-static-top" role="navigation">
	<div class="container">
		<div class="navbar-header">
			<button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#header-navbar-collapse">
				<span class="sr-only">Toggle navigation</span>
				<span class="icon-bar"></span>
				<span class="icon-bar"></span>
				<span class="icon-bar"></span>
			</button>
			<div class="navbar-left visible-xs">
				<%: Html.HtmlSnippet("Mobile Header") %>
			</div>
		</div>
		<div id="header-navbar-collapse" class="navbar-collapse collapse">
			<div class="navbar-left hidden-xs">
				<%: Html.HtmlSnippet("Navbar Left") %>
			</div>
			<div class="navbar-right hidden-xs">
				<%: Html.HtmlSnippet("Navbar Right") %>
			</div>
			<ul class="nav navbar-nav navbar-right">
				<% if (isAuthenticated) { %>
					<li class="dropdown">
						<a href="#" class="dropdown-toggle" data-toggle="dropdown">
							<span class="fa fa-user" aria-hidden="true"></span>
							<span class="username"><%: Html.AttributeLiteral(Html.PortalUser(), "fullname") %></span>
							<span class="caret"></span>
						</a>
						<ul class="dropdown-menu">
							<% if (Html.BooleanSetting("Header/ShowAllProfileNavigationLinks").GetValueOrDefault(true)) { %>
								<% var profileNavigation = Html.WebLinkSet("Profile Navigation"); %>
								<% if (profileNavigation != null) { %>
									<% foreach (var webLink in profileNavigation.WebLinks) { %>
										<%: Html.WebLinkListItem(webLink, false, false, maximumWebLinkChildDepth: 1) %>
									<% } %>
								<% } %>
							<% } else { %>
								<li><a href="<%: Html.SiteMarkerUrl("Profile") %>"><%: Html.SnippetLiteral("Profile Link Text", "Profile") %></a></li>
							<% } %>
							<li role="separator" class="divider"></li>
							<li>
								<a href="<%: signOutUrl %>">
									<span class="fa fa-sign-out" aria-hidden="true"></span>
									<%: signOutLabel %>
								</a>
							</li>
						</ul>
					</li>
				<% } else { %>
					<% if (signInEnabled) { %>
						<li>
							<a href="<%: signInUrl %>">
								<span class="fa fa-sign-in" aria-hidden="true"></span>
								<%: signInLabel %>
							</a>
						</li>
					<% } %>
					<% if (registrationEnabled) { %>
						<li>
							<a href="<%: registerUrl %>"><%: registerLabel %></a>
						</li>
					<% } %>
				<% } %>
			</ul>
			<ul class="nav navbar-nav navbar-left">
				<% if (relatedWebsites.Any) { %>
					<li class="dropdown">
						<a href="#" class="dropdown-toggle" data-toggle="dropdown">
							<span class="fa fa-globe" aria-hidden="true"></span> <%: relatedWebsites.Current.Title %> <span class="caret"></span>
						</a>
						<ul class="dropdown-menu" role="menu">
							<% foreach (var relatedWebsiteLink in relatedWebsites.Links) { %>
								<li><a href="<%: relatedWebsiteLink.Url %>"><%: relatedWebsiteLink.Title %></a></li>
							<% } %>
						</ul>
					</li>
				<% } %>
				<% var shoppingCartUrl = Html.SiteMarkerUrl("Shopping Cart"); %>
				<% if (!string.IsNullOrEmpty(shoppingCartUrl)) { %>
					<li class="shopping-cart-status" data-href="<%: Url.Action("Status", "ShoppingCart", new {area = "Commerce", __portalScopeId__ = Html.Website().EntityReference.Id}) %>">
						<a href="<%: shoppingCartUrl %>">
							<span class="fa fa-shopping-cart" aria-hidden="true"></span>
							<%: Html.SnippetLiteral("Shopping Cart Status Link Text", "Cart") %>
							<span class="count">(<span class="value"></span>)</span>
						</a>
					</li>
				<% } %>
			</ul>
			<% if (searchEnabled) { %>
				<form class="navbar-form navbar-left navbar-search" method="GET" action="<%: searchUrl %>" role="search">
					<label for="q-xs" class="sr-only"><%: searchLabel %></label>
					<div class="input-group">
						<% if (searchFilterOptions.Any()) { %>
							<div class="btn-group btn-select input-group-btn" data-target="#filter-xs" data-focus="#q-xs">
								<button id="search-filter-xs" type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">
									<span class="selected"><%: searchFilterDefaultText %></span>
									<span class="caret"></span>
								</button>
								<ul class="dropdown-menu" role="menu" aria-labelledby="search-filter-xs">
									<li>
										<a data-value=""><%: searchFilterDefaultText %></a>
									</li>
									<% foreach (var option in searchFilterOptions) { %>
										<li>
											<a data-value="<%: option.Value %>"><%: option.Key %></a>
										</li>
									<% } %>
								</ul>
							</div>
							<label for="filter-xs" class="sr-only"><%: searchFilterLabel %></label>
							<select id="filter-xs" name="filter" class="btn-select" aria-hidden="true" data-query="filter">
								<option value="" selected="selected"><%: searchFilterDefaultText %></option>
								<% foreach (var option in searchFilterOptions) { %>
									<option value="<%: option.Value %>"><%: option.Key %></option>
								<% } %>
							</select>
						<% } %>
						<input type="text" class="form-control" id="q-xs" name="q" placeholder="<%: searchLabel %>" title="<%: searchLabel %>" data-query="q">
						<div class="input-group-btn">
							<button type="submit" class="btn btn-default" title="<%: searchToolTip %>"><span class="fa fa-search" aria-hidden="true"></span></button>
						</div>
					</div>
				</form>
			<% } %>
			<% Html.RenderAction("HeaderPrimaryNavigationXs", "Layout", new { area = "Portal" }, true); %>
		</div>
	</div>
</div>
<div class="masthead tabs well hidden-xs" role="banner">
	<div class="container">
		<%: Html.HtmlSnippet("Header") %>
		<% Html.RenderAction("HeaderPrimaryNavigationTabs", "Layout", new { area = "Portal" }, true); %>
	</div>
</div>
