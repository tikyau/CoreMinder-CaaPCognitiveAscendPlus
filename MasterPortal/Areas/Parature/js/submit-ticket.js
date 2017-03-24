(function ($) {

	function submitTicket(element) {
		this._element = $(element);
		this._url = this._element.data("url");
	}

	$(document).ready(function () {
		$(".submit-parature-ticket").each(function () {
			new submitTicket($(this)).render();
		});
	});

	submitTicket.prototype.render = function () {
		var $this = this;
		var $element = $this._element;
		var $submitButton = $element.find(".submit-ticket");
		
		$submitButton.on("click", function () {
			$this.submit();
		});
	}

	submitTicket.prototype.submit = function () {
		var $this = this;
		var $element = $this._element;
		var url = $this._url;
		var $submitButton = $element.find(".submit-ticket");
		var summaryText = $element.find("#ticketsummary").val();
		var detailsText = $element.find("#ticketdetails").val();
		var priority = $element.find("#ticketpriority").val();
		
		if (url == null || url == '') {
			var urlError = { Message: "System Error. The URL to the service for this Submit Support Ticket request could not be determined." };
			onFail(urlError, $element);
			return;
		}

		if (summaryText == null || summaryText.length <= 0) {
			return;
		}

		if (detailsText == null || detailsText.length <= 0) {
			return;
		}

		$submitButton.attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");

		var ticket = {}
		ticket.details = detailsText;
		ticket.summary = summaryText;
		ticket.priority = priority;
		var json = JSON.stringify(ticket);

		$.ajax({
			url: url,
			type: 'POST',
			data: json,
			dataType: "json",
			contentType: 'application/json',
			cache: false
		}).done(function (data) {
			if (data && data.number) {
				$element.find(".success-message .ticket-number").text(data.number);
			}
			$element.find(".error-message").hide();
			$element.find("form.ticket-form").hide();
			$element.find(".success-message").show();
		}).fail(function (jqXhr) {
			var contentType = jqXhr.getResponseHeader("content-type");
			var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
			onFail(error, $element);
		}).always(function () {
			$submitButton.removeAttr("disabled", "disabled").find("i.fa-spin").remove();
		});
	}

	function onFail(error, $element) {
		if (typeof error !== typeof undefined && error !== false && error != null) {
			console.log(error);

			var $error = $element.find(".error-message");
			var create = false;
			if ($error.length == 0) {
				create = true;
				$error = $("<div></div>").addClass("error-message alert alert-block alert-danger clearfix");
			} else {
				$error.hide().empty();
			}
			
			if (typeof error.Message !== typeof undefined && error.Message !== false && error.Message != null) {
				if (typeof error.Message === 'number') {
					$error.append("<p><span class='fa fa-exclamation-triangle' aria-hidden='true'></span> " + error.Message + " Error</p>");
				} else {
					$error.append("<p><span class='fa fa-exclamation-triangle' aria-hidden='true'></span> " + error.Message + "</p>");
				}
			}

			if (create) {
				$element.prepend($error);
			}

			$error.show();
		}
	}

}(jQuery));