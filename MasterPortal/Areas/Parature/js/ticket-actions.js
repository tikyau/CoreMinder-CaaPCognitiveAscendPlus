(function ($) {

	function ticketActions(element) {
		this._element = $(element);
		this._url = this._element.data("url");
		this._ticketId = this._element.data("id");
	}

	$(document).ready(function () {
		$("#ticket-actions").each(function () {
			new ticketActions($(this)).render();
		});
	});

	ticketActions.prototype.render = function () {
		var $this = this;
		var $element = $this._element;
		var $actionLinks = $element.find("a.run-ticket-action");
		var $modal = $("#modal-ticket-action");
		var $submitButton = $modal.find(".btn-primary");

		$submitButton.off("click");
		$submitButton.on("click", function(e) {
			e.preventDefault();
			$this.submit();
		});

		$actionLinks.on("click", function (e) {
			e.preventDefault();
			var actionId = $(this).data("id");
			$modal.find(".modal-title").html($(this).html());
			$modal.data("action-id", actionId);
			$modal.modal('show');
		});

		$modal.on("hidden.bs.modal", function() {
			$modal.removeData("action-id");
			$("#action-comment").val('');
		});
	}

	ticketActions.prototype.submit = function () {
		var $this = this;
		var $element = $this._element;
		var url = $this._url;
		var $modal = $("#modal-ticket-action");
		var $submitButton = $modal.find(".btn-primary");
		var ticketId = $this._ticketId;
		var actionId = $modal.data("action-id");
		var comment = $("#action-comment").val();
		
		if (url == null || url == '') {
			var urlError = { Message: "System Error. The URL to the service for this Run Ticket Action request could not be determined." };
			onFail(urlError, $element);
			return;
		}

		if (!ticketId) {
			var ticketIdError = { Message: "System Error. The ticket ID for this Run Ticket Action request could not be determined." };
			onFail(ticketIdError, $element);
			return;
		}

		if (!actionId) {
			var actionIdError = { Message: "System Error. The action ID for this Run Ticket Action request could not be determined." };
			onFail(actionIdError, $element);
			return;
		}

		$submitButton.attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");

		var action = {}
		action.ticketid = ticketId;
		action.actionid = actionId;
		action.comment = comment;
		var json = JSON.stringify(action);

		$.ajax({
			url: url,
			type: 'POST',
			data: json,
			dataType: "json",
			contentType: 'application/json',
			cache: false
		}).done(function () {
			$modal.modal('hide');
			location.reload(true);
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