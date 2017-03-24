<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Adxstudio.Xrm.Products.IProduct>" %>

<% using (Ajax.BeginForm("CreateReview", "Products", new { productid = Model.Entity.Id }, new AjaxOptions { UpdateTargetId = "create-review", OnFailure = "reviewFailure", OnComplete = "reviewCreated" }, new { @class = "form-horizontal" }))
{ %>
	<fieldset>
		<legend>
			<%: Html.SnippetLiteral("Product Review Create Title", "Submit a Review") %>
		</legend>
		<%= Html.ValidationSummary(string.Empty, new {@class = "alert alert-block alert-danger"}) %>
		<% if (!Request.IsAuthenticated)
		{ %>
			<div class="form-group">
				<label class="col-sm-3 control-label required" for="reviewerName"><%: Html.SnippetLiteral("Product Create Review Reviewer Name Label", "Nickname") %></label>
				<div class="col-sm-9">
					<%= Html.TextBox("reviewerName", string.Empty, new {@maxlength = "100", @class = "form-control"}) %>
				</div>
			</div>
			<div class="form-group">
				<label class="col-sm-3 control-label required" for="reviewerEmail"><%: Html.SnippetLiteral("Product Create Review Reviewer Email Label", "E-mail") %></label>
				<div class="col-sm-9">
					<%= Html.TextBox("reviewerEmail", string.Empty, new {@maxlength = "200", @class = "form-control"}) %>
					<span class="help-inline"> (This will not be displayed on the review)</span>
				</div>
			</div>
		<% } else { %>
			<div class="form-group">
				<label class="col-sm-3 control-label required" for="reviewerName"><%: Html.SnippetLiteral("Product Create Review Reviewer Name Label", "Nickname") %></label>
				<div class="col-sm-9">
					<%= Html.TextBox("reviewerName", Html.AttributeLiteral(Html.PortalUser(), "nickname"), new {@maxlength = "100", @class = "form-control"}) %>
				</div>
			</div>
		<% } %>
		<div class="form-group">
			<label class="col-sm-3 control-label" for="reviewerLocation"><%: Html.SnippetLiteral("Product Create Review Reviewer Location Label", "Location") %></label>
			<div class="col-sm-9">
				<%= Html.TextBox("reviewerLocation", string.Empty, new {@maxlength = "100", @class = "form-control"}) %>
			</div>
		</div>
		<div class="form-group">
			<label class="col-sm-3 control-label required" for="rating"><%: Html.SnippetLiteral("Product Create Review Rating Label", "Rating") %></label>
			<div class="col-sm-9">
				<%= Html.Hidden("maximumRatingValue", 5) %>
				<%= Html.Hidden("rating", 0) %>
				<div class="rateit" data-rateit-resetable="false" data-rateit-step="1" data-rateit-min="0" data-rateit-max="5" data-rateit-backingfld="#rating"></div>
			</div>
		</div>
		<div class="form-group">
			<div class="col-sm-offset-3 col-sm-9">
				<p>
					<%: Html.SnippetLiteral("Product Review Recommend Text", "Would you recommend this product to a friend?") %>
				</p>
				<div class="radio">
					<label>
						<%= Html.RadioButton("recommend", true, true) %> Yes
					</label>
				</div>
				<div class="radio">
					<label>
						<%= Html.RadioButton("recommend", false, false) %> No
					</label>
				</div>
			</div>
		</div>
		<div class="form-group">
			<label class="col-sm-3 control-label required" for="title"><%: Html.SnippetLiteral("Product Create Review Title Label", "Title") %></label>
			<div class="col-sm-9">
				<%= Html.TextBox("title", string.Empty, new {@maxlength = "150", @class = "form-control"}) %>
			</div>
		</div>
		<div class="form-group">
			<label class="col-sm-3 control-label" for="content"><%: Html.SnippetLiteral("Product Create Review Content Label", "Review") %></label>
			<div class="col-sm-9">
				<%= Html.TextArea("content", string.Empty, new {@rows = "10", @maxlength = "10000", @class = "form-control"}) %>
			</div>
		</div>
		<div class="form-group">
			<div class="col-sm-offset-3 col-sm-9">
				<input id="submit-review" class="btn btn-primary" type="submit" value="<%: Html.SnippetLiteral("Product Review Submit Button Text", "Submit Review") %>" />
				<a id="cancel-review" href="#cancel" class="cancel btn btn-default"><%: Html.SnippetLiteral("Product Review Cancel Button Text", "Cancel") %></a>
			</div>
		</div>
	</fieldset>
<% } %>

