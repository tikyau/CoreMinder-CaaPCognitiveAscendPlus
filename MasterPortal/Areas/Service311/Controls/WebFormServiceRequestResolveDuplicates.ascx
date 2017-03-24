<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WebFormServiceRequestResolveDuplicates.ascx.cs" Inherits="Site.Areas.Service311.Controls.WebFormServiceRequestResolveDuplicates" %>
<asp:Panel ID="PanelResolveDuplicates" runat="server" CssClass="row">
	<input type="hidden" ID="CurrentServiceRequestId" runat="server"/>
	<input type="hidden" ID="SelectedServiceRequestId" runat="server"/>
	<div class="col-sm-12">
		<div class="alert alert-warning">
			<adx:Snippet runat="server" SnippetName="311 Service Request Resolve Duplicates Summary" DefaultText="Similar service requests were submitted previously. Would you like to replace your submission by selecting a service request from the list or continue with your submission?" />
		</div>
		<div class="panel panel-default">
			<div class="panel-heading"><adx:Snippet runat="server" SnippetName="311 Service Request Resolve Duplicates Continue Existing" DefaultText="Continue with an existing service request" /></div>
			<div class="panel-body">
				<asp:PlaceHolder ID="DuplicateListPlaceholder" runat="server"></asp:PlaceHolder>
			</div>
		</div>
		<div class="panel panel-default">
			<div class="panel-heading"><adx:Snippet runat="server" SnippetName="311 Service Request Resolve Duplicates Continue My" DefaultText="Continue with my service request" /></div>
			<div class="panel-body">
				<asp:PlaceHolder ID="CurrentListPlaceholder" runat="server"></asp:PlaceHolder>
			</div>
		</div>
	</div>
</asp:Panel>

<section class="modal fade" id="confirmSubmit" tabindex="-1" role="dialog" aria-labelledby="confirmSubmitLabel" aria-hidden="true">
	<div class="modal-dialog">
		<div class="modal-content">
			<div class="modal-header">
				<h1 class="modal-title h4" id="confirmSubmitLabel"><adx:Snippet runat="server" SnippetName="311 Service Request Resolve Duplicates Confirmation Header" DefaultText="Confirmation" /></h1>
			</div>
			<div class="modal-body" id="continueExisting">
				<adx:Snippet runat="server" SnippetName="311 Service Request Resolve Duplicates Continue Existing Message" DefaultText="Abandon my new service request and continue with the selected service request." />
			</div>
			<div class="modal-body" id="continueNew">
				<adx:Snippet runat="server" SnippetName="311 Service Request Resolve Duplicates Continue New Message" DefaultText="Ignore the existing service requests and submit my new service request." />
			</div>
			<div class="modal-footer">
				<button type="button" class="btn btn-default" data-dismiss="modal"><adx:Snippet runat="server" SnippetName="311 Service Request Resolve Duplicates Confirmation Cancel" DefaultText="Cancel" /></button>
				<button type="button" class="btn btn-primary" data-dismiss="modal" id="confirmSubmitContinue"><adx:Snippet runat="server" SnippetName="311 Service Request Resolve Duplicates Confirmation Continue" DefaultText="Continue" /></button>
			</div>
		</div>
	</div>
</section>

<script type="text/javascript">
	function webFormClientValidate() {
		if ($('#confirmSubmit').attr("data-continue")) return true;

		if ($('#CurrentServiceRequestId').val() == $('#SelectedServiceRequestId').val()) {
			$('#continueExisting').hide();
			$('#continueNew').show();
		} else {
			$('#continueExisting').show();
			$('#continueNew').hide();
		}

		$('#confirmSubmit').modal();
		return false;
	}
</script>
