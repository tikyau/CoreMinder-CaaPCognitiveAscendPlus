(function($) {

	function entityAssociate(element) {
		this._element = $(element);
		this._associate = this._element.data("associate") || {};
		this._url = this._element.attr("data-url");
		this._parent = this._element.parent(".entity-grid");
		this._success = false;
	}

	$(document).ready(function() {
		$(".entity-associate").each(function() {
			new entityAssociate($(this)).render();
		});
	});

	entityAssociate.prototype.render = function () {
		var $this = this;
		var $element = $this._element;
		var $selection = $element.find(".panel-body.selected-records");
		var $grid = $element.find(".entity-grid");
		var $button = $element.find(".modal-footer .btn-primary");
		var $modal = $element.find(".modal-associate");
		var $parent = $this._parent;
		
		$button.on("click", function () {
			$modal.find(".alert-danger.error").remove();
			$this.associate();
		});

		$grid.children(".view-grid").on("click", "tr", function () {
			var $tr = $(this);
			var entity = $(this).attr("data-entity");
			var id = $(this).attr("data-id");
			var name = $(this).attr("data-name");
			var $existingItem = $selection.find("div[data-entity='" + entity + "'][data-id='" + id + "']");
			var exists = $existingItem.length > 0;

			if (!$tr.hasClass("selected")) {
				$existingItem.remove();
				return;
			}
			
			if (exists) {
				return;
			}

			var $item = $("<div class='item pull-left btn btn-default'></div>")
				.attr("data-entity", entity)
				.attr("data-id", id)
				.attr("data-name", name)
				.append($("<span class='name'></span>").html(name));
			
			var $delete = $("<span class='remove'><span class='fa fa-times' aria-hidden='true'></span></span>").on("click", function () {
				$item.remove();
				$tr.find("td:first").empty();
				$tr.removeClass("selected").removeClass("info");
			});

			$item.append($delete);
			$selection.append($item);
		});

		$modal.on('show.bs.modal', function () {
			$grid.trigger("refresh");
		});

		$modal.on('hidden.bs.modal', function () {
			$modal.find(".alert-danger.error").remove();
			$selection.empty();
			$grid.children(".view-toolbar").find(".view-search input.query").val('');
			$grid.children(".view-grid").find("tbody tr.selected").each(function () {
				$(this).find("td:first").empty();
				$(this).removeClass("selected").removeClass("info");
			});
			if ($this._success) {
				$parent.trigger("refresh");
			}
		});
	}

	entityAssociate.prototype.associate = function () {
		var $this = this;
		var $element = $this._element;
		var associate = $this._associate;
		var url = $this._url;
		var $selection = $element.find(".panel-body.selected-records");
		var $button = $element.find(".modal-footer button.primary");
		var $modal = $element.find(".modal-associate");
		
		if (url == null || url == '') {
			var urlError = { Message: "System Error", InnerError: { Message: "The URL to the service for this Associate Request could determined." } };
			onFail(urlError, $modal);
			return;
		}

		var items = $.map($selection.find("div.item"), function (e) {
			return { LogicalName: $(e).attr("data-entity"), Id: $(e).attr("data-id"), Name: $(e).attr("data-name") }
		});

		if (items.length <= 0) {

			return;
		}

		$button.attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
			
		var request = associate;
		request.RelatedEntities = items;
		console.log(request);
		var data = JSON.stringify(request);
			
		$.ajax({
			type: "POST",
			contentType: "application/json",
			url: url,
			data: data
		}).done(function () {
			$this._success = true;
			$modal.modal("hide");
		}).fail(function (jqXhr) {
			var contentType = jqXhr.getResponseHeader("content-type");
			var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
			onFail(error, $modal);
		}).always(function () {
			$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
		});
	}

	function onFail(error, $modalElement) {
		if (typeof error !== typeof undefined && error !== false && error != null) {
			console.log(error);

			var $body = $modalElement.find(".modal-body");

			var $error = $modalElement.find(".alert-danger.error");

			if ($error.length == 0) {
				$error = $("<div></div>").addClass("alert alert-block alert-danger error clearfix");
			} else {
				$error.empty();
			}

			if (typeof error.Message !== typeof undefined && error.Message !== false && error.Message != null) {
				if (typeof error.Message === 'number') {
					$error.append("<p><span class='fa fa-exclamation-triangle' aria-hidden='true'></span> " + error.Message + " Error</p>");
				} else {
					$error.append("<p><span class='fa fa-exclamation-triangle' aria-hidden='true'></span> " + error.Message + "</p>");
				}
			}

			if (typeof error.InnerError !== typeof undefined && error.InnerError !== false && error.InnerError != null) {
				$error.append("<p>" + error.InnerError.Message + "</p>");
				if (typeof error.InnerError.StackTrace !== typeof undefined && error.InnerError.StackTrace !== false && error.InnerError.StackTrace != null) {
					$error.append($("<textarea></textarea>").text(error.InnerError.StackTrace));
				}
			}

			$body.prepend($error);
		}
	}

}(jQuery));