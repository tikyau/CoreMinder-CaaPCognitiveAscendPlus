<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Rating.ascx.cs" Inherits="Site.Controls.Rating" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<asp:UpdatePanel ID="RatingUpdatePanel" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="False" class="rating">
	<ContentTemplate>
		<ajax:Rating ID="RatingStars"
			CssClass="rating-stars"
			AutoPostBack="true"
			OnChanged="Rating_Changed"
			MaxRating="5"
			StarCssClass="rating-star"
			EmptyStarCssClass="rating-star-empty"
			FilledStarCssClass="rating-star-filled"
			WaitingStarCssClass="rating-star-saved"
			runat="server" />
		<span class="rating-count badge"><asp:Literal ID="Count" Text="0" runat="server" /></span>
	</ContentTemplate>
	<Triggers>
		<asp:AsyncPostBackTrigger ControlID="RatingStars" EventName="Changed" />
	</Triggers>
</asp:UpdatePanel>
