(function ($) {

	function ticketAddAttachments(element) {
		this._element = $(element);
		this._url = this._element.data("url");
		this._ticketId = this._element.data("id");
		this._files = null;
	}

	$(document).ready(function () {
		$(".ticket-attachments").each(function () {
			new ticketAddAttachments($(this)).render();
		});
	});

	ticketAddAttachments.prototype.render = function () {
		var $this = this;
		var $element = $this._element;
		var url = $this._url;
		var ticketId = $this._ticketId;
		var $modal = $element.children(".modal-ticket-add-attachments");
		var $addAttachmentButton = $element.find(".add-ticket-attachment");
		var $submitButton = $modal.find(".btn-primary");
		
		$element.find(".error-message").remove();
		$addAttachmentButton.removeAttr("disabled");

		if (url == null || url == '') {
			var urlError = { Message: "System Error. The URL to the service for this Add Ticket Attachments request could not be determined." };
			onFail(urlError, $element);
			$addAttachmentButton.attr("disabled", "disabled");
			return;
		}

		if (!ticketId) {
			var ticketIdError = { Message: "System Error. The ticket ID for this Add Ticket Attachments request could not be determined." };
			onFail(ticketIdError, $element);
			$addAttachmentButton.attr("disabled", "disabled");
			return;
		}

		$submitButton.off("click");
		$submitButton.on("click", function (e) {
			e.preventDefault();
			$this.submit();
		});

		$addAttachmentButton.on("click", function (e) {
			e.preventDefault();
			$modal.modal('show');
		});

		$modal.on("hidden.bs.modal", function () {
			$modal.find("input[type='file']").val('');
			$modal.find("input[type='file']").parents('.btn-file').siblings(".file-status").find("ul").empty();
			$modal.find(".error-message").remove();
			$this._files = null;
		});

		$(document).on('change', '.btn-file :file', function () {
			var input = $(this);
			var files = input.get(0).files;
			var $fileStatusList = $(this).parents('.btn-file').siblings(".file-status").find("ul");
			$fileStatusList.empty();
			if (!files || !files.length) {
				$this._files = null;
				return;
			}
			for (var i = 0; i < files.length; i++) {
				var name = files[i].name.replace(/\\/g, '/').replace(/.*\//, '');
				var size = files[i].size + " bytes";
				var $item = $("<li></li>").html("<span class='fa fa-file-o' aria-hidden='true'></span> " + name + " (" + size + ")").appendTo($fileStatusList);
				var $remove = $("<a href='#'><span class='fa fa-times' aria-hidden='true'></span></a>").attr("data-index", i);
				$item.append("&nbsp;").append($remove);
				$remove.on("click", function(e) {
					e.preventDefault();
					$(this).parents("li").remove();
					var index = $(this).data("index");
					var newFiles = [];
					var newFilesIndex = 0;
					for (var j = 0; j < files.length; j++) {
						if (j != index) {
							newFiles[newFilesIndex] = files[j];
							newFilesIndex++;
						}
					}
					$this._files = newFiles;
				});
			}
			$this._files = files;
		});
	}

	ticketAddAttachments.prototype.submit = function () {
		var $this = this;
		var $element = $this._element;
		var url = $this._url;
		var $modal = $element.children(".modal-ticket-add-attachments");
		var $modalBody = $element.find(".modal-body");
		var $submitButton = $modal.find(".btn-primary");
		var ticketId = $this._ticketId;
		var files = $this._files;
		
		if (url == null || url == '') {
			var urlError = { Message: "System Error. The URL to the service for this Add Ticket Attachments request could not be determined." };
			onFail(urlError, $modalBody);
			return;
		}

		if (!ticketId) {
			var ticketIdError = { Message: "System Error. The ticket ID for this Add Ticket Attachments request could not be determined." };
			onFail(ticketIdError, $modalBody);
			return;
		}

		if (typeof files === typeof undefined || files == null || files.length == 0) {
			return;
		}

		$submitButton.attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
		$modal.find("input[type='file']").attr("disabled", "disabled");

		if (window.FormData !== undefined)
		{
			var formData = new FormData();
			formData.append("ticketid", ticketId);

			$.each(files, function(index, file) {
				formData.append("files", file, file.name.replace(/\\/g, '/').replace(/.*\//, ''));
			});
			
			$.ajax({
				url: url,
				type: 'POST',
				data: formData,
				mimeType: "multipart/form-data",
				contentType: false,
				cache: false,
				processData: false
			}).done(function () {
				$modal.modal('hide');
				location.reload(true);
			}).fail(function (jqXhr) {
				var contentType = jqXhr.getResponseHeader("content-type");
				var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
				onFail(error, $modalBody);
			}).always(function () {
				$submitButton.removeAttr("disabled").find("i.fa-spin").remove();
				$modal.find("input[type='file']").removeAttr("disabled");
			});
		}
		else
		{
			onFail({ Message: "Your browser does not support FormData." }, $modalBody);
		}
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