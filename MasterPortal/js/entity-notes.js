(function ($) {

	function entityNotes(element) {
		this._element = $(element);
		this._target = this._element.data("target") || {};
		this._attachmentSettings = this._element.data("attachmentsettings");
		this._serviceUrlGet = this._element.attr("data-url-get");
		this._serviceUrlAdd = this._element.attr("data-url-add");
		this._serviceUrlEdit = this._element.attr("data-url-edit");
		this._serviceUrlDelete = this._element.attr("data-url-delete");
		this._addEnabled = this._element.data("add-enabled");
		this._editEnabled = this._element.data("edit-enabled");
		this._deleteEnabled = this._element.data("delete-enabled");
		this._pageSize = this._element.attr("data-pagesize");
		this._orders = this._element.data("orders");
		this._addSuccess = false;
		this._editSuccess = false;
		this._deleteSuccess = false;
		var that = this;
		$(element).on("refresh", function (e, page) {
			that.load(page);
		});
	}

	$(document).ready(function () {
		$(".entity-notes").each(function () {
			new entityNotes($(this)).render();
		});
	});

	entityNotes.prototype.render = function () {
		var $this = this;
		var $element = $this._element;
		var $addNoteButton = $element.children(".note-actions").find("a.addnote");
		var $modalAddNote = $element.children(".modal-addnote").appendTo("body");
		var $modalAddNoteButton = $modalAddNote.find(".modal-footer .btn-primary");

		$this.load();

		if ($this._addEnabled) {
			$addNoteButton.on("click", function() {
				$modalAddNote.modal("show");
			});

			$modalAddNoteButton.on("click", function() {
				$this.addNote($modalAddNote);
			});

			$modalAddNote.on('hidden.bs.modal', function() {
				$modalAddNote.find("textarea").val('');
				$modalAddNote.find("input[type='file']").val('');
				$modalAddNote.find(".alert-danger.error").remove();
			});
		}
	}

	entityNotes.prototype.load = function (page) {
		var $this = this;
		var $element = $this._element;
		var $notes = $element.children(".notes");
		var $errorMessage = $element.children(".notes-error");
		var $emptyMessage = $element.children(".notes-empty");
		var $accessDeniedMessage = $element.children(".notes-access-denied");
		var $loadingMessage = $element.children(".notes-loading");
		var $pagination = $element.find(".notes-pagination");
		var serviceUrlGet = $this._serviceUrlGet;
		var regarding = $this._target;
		var orders = $this._orders;
		var defaultPageSize = $this._pageSize;
		
		$errorMessage.hide();
		$emptyMessage.hide();
		$accessDeniedMessage.hide();
		$notes.hide().empty();
		$loadingMessage.show();
		var pageNumber = $pagination.data("current-page");
		if (pageNumber == null || pageNumber == '') {
			pageNumber = 1;
		}
		page = page || pageNumber;
		var pageSize = $pagination.data("pagesize");
		if (pageSize == null || pageSize == '') {
			pageSize = defaultPageSize;
		}
		$this.getData(serviceUrlGet, regarding, orders, page, pageSize,
			function (data) {
				// done
				if (typeof data === typeof undefined || data === false || data == null) {
					$emptyMessage.fadeIn();
					return;
				}
				if (typeof data.Records !== typeof undefined && data.Records !== false && (data.Records == null || data.Records.length == 0)) {
					$emptyMessage.fadeIn();
					return;
				}
				if (typeof data.AccessDenied !== typeof undefined && data.AccessDenied !== false && data.AccessDenied) {
					$accessDeniedMessage.fadeIn();
					return;
				}
				
				var source = $("#notes-template").html();
				Handlebars.registerHelper('AttachmentUrlWithTimeStamp', function () {
					return this.AttachmentUrl + "?t=" + new Date().getTime(); //unique cache-busting query parameter
				});
				var template = Handlebars.compile(source);
				$notes.html(template(data));
				$notes.find("abbr.timeago").each(function () {
					var date = $(this).attr("title");
					var moment = window.moment;
					if (moment) {
						var dateFormat = dateFormatConverter.convert($element.closest("[data-dateformat]").data("dateformat") || "M/d/yyyy", dateFormatConverter.dotNet, dateFormatConverter.momentJs);
						var timeFormat = dateFormatConverter.convert($element.closest("[data-timeformat]").data("timeformat") || "h:mm tt", dateFormatConverter.dotNet, dateFormatConverter.momentJs);
						var datetimeFormat = dateFormat + ' ' + timeFormat;
						$(this).text(moment(date).format(datetimeFormat));
					}
				});
				
				$notes.find("abbr.timeago").timeago();
				$notes.fadeIn();
				$this.initializePagination(data);
				if ($this._editEnabled && $this._editEnabled != "False") {
					$this.addEditClickEventHandlers();
				}
				if ($this._deleteEnabled && $this._deleteEnabled != "False") {
					$this.addDeleteClickEventHandlers();
				}
			},
			function (jqXhr, textStatus, errorThrown) {
				// fail
				$errorMessage.find(".details").append(errorThrown);
				$errorMessage.show();
			},
			function () {
				// always
				$loadingMessage.hide();
			});
	}

	entityNotes.prototype.getData = function (url, regarding, orders, page, pageSize, done, fail, always) {
		done = $.isFunction(done) ? done : function () { };
		fail = $.isFunction(fail) ? fail : function () { };
		always = $.isFunction(always) ? always : function () { };
		if (!url || url == '') {
			always.call(this);
			fail.call(this, null, "error", "A required service url was not provided.");
			return;
		}
		if (!regarding) {
			always.call(this);
			fail.call(this, null, "error", "A required regarding EntityReference parameter was not provided.");
			return;
		}
		pageSize = pageSize || -1;
		var data = {};
		data.regarding = regarding;
		data.orders = orders;
		data.page = page;
		data.pageSize = pageSize;
		var jsonData = JSON.stringify(data);
		$.ajax({
			type: 'POST',
			dataType: "json",
			contentType: 'application/json',
			url: url,
			data: jsonData,
			global: false
		})
			.done(done)
			.fail(fail)
			.always(always);
	}

	entityNotes.prototype.addEditClickEventHandlers = function() {
		var $this = this;
		var $element = $this._element;
		var $modal = $element.children(".modal-editnote").appendTo("body");

		if ($modal.length == 0) {
			return;
		}

		var $file = $modal.find("input[type='file']");
		var $button = $modal.find("button.primary");

		if ($file.length > 0) {
			$file.on('change', function() {
				$modal.find(".attachment").remove();
			});
		}

		$button.unbind("click");
		$button.on("click", function (e) {
			e.preventDefault();
			$this.updateNote($modal);
		});

		$modal.on('hidden.bs.modal', function () {
			$modal.find(".alert-danger.error").remove();
			$modal.find("textarea").val('');
			$modal.find("input[type='file']").val('');
			$modal.find(".alert-danger.error").remove();
			$modal.data("id", "");
		});

		$element.find(".edit-link").on("click", function (e) {
			e.preventDefault();
			var $note = $(this).closest(".note");
			var id = $note.data("id");
			var text = $note.data("unformattedtext") || "";
			var subject = $note.data("subject") || "";
			var isPrivate = $note.data("isprivate");
			var hasAttachment = $note.data("hasattachment");
			var attachmentFileName = $note.data("attachmentfilename");
			var attachmentFileSize = $note.data("attachmentfilesize");
			var attachmentUrl = $note.data("attachmenturl");
			var attachmentIsImage = $note.data("attachmentisimage");

			if (!id || id == '') {
				console.log("Failed to launch edit note dialog. Data parameter 'id' is null.");
				return;
			}

			$modal.data("id", id);
			$modal.data("subject", subject);
			$modal.find("textarea").val(text);
			if (isPrivate) {
				$modal.find("input[type='checkbox']").prop("checked", true);
			}
			var $fileContainer = $file.parent();
			if (hasAttachment) {
				attachmentUrl += "?t=" + new Date().getTime(); //unique cache-busting query parameter
				var $attachment = $modal.find(".attachment");
				if ($attachment.length == 0) {
					$attachment = $("<div class='attachment clearfix'></div>");
				}
				var $linkContainer = $("<div class='link'></div>");
				var $link = $("<a target='_blank'></a>").attr("href", attachmentUrl).html("<span class='fa fa-file' aria-hidden='true'></span> " + attachmentFileName + " (" + attachmentFileSize + ")");
				$linkContainer.html($link);
				if (attachmentIsImage) {
					var $imageLink = $("<a target='_blank' class='thumbnail'></a>").attr("href", attachmentUrl);
					var $image = $("<img />").attr("src", attachmentUrl);
					var $thumbnail = $("<div class='img col-md-4'></div>");
					$thumbnail.append($imageLink.html($image));
					$attachment.html($thumbnail).append($linkContainer);
					$fileContainer.prepend($attachment);
				} else {
					$attachment.html($linkContainer);
					$fileContainer.prepend($attachment);
				}
			}
			$modal.modal();
		});
	}

	entityNotes.prototype.addDeleteClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var url = $this._serviceUrlDelete;

		var $modal = $element.children(".modal-deletenote").appendTo("body");

		if ($modal.length == 0) {
			return;
		}

		$modal.on('hidden.bs.modal', function () {
			$modal.find(".alert-danger.error").remove();
		});

		$element.find(".delete-link").on("click", function (e) {
			e.preventDefault();
			var $note = $(this).closest(".note");
			var id = $note.data("id");
			if (!id || id == '') {
				console.log("Failed to launch delete note dialog. Data parameter 'id' is null.");
				return;
			}
			var $button = $modal.find(".modal-footer button.primary");
			$button.unbind("click");
			$button.on("click", function () {
				$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
				var data = {};
				data.id = id;
				var jsonData = JSON.stringify(data);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: jsonData
				}).done(function () {
					$this._deleteSuccess = true;
					$element.trigger("refresh");
					$modal.modal("hide");
				}).fail(function (jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					onFail(error, $modal);
				}).always(function () {
					$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
				});
			});
			$modal.modal();
		});
	}

	entityNotes.prototype.addNote = function ($modal) {
		var $this = this;
		var $element = $this._element;
		var target = $this._target;
		var url = $this._serviceUrlAdd;
		var $button = $modal.find(".modal-footer button.primary");
		var noteText = $modal.find("textarea").val();
		var isPrivate = false;
		var file = null;
		
		if (url == null || url == '') {
			var urlError = { Message: "System Error", InnerError: { Message: "The URL to the service for this Add Note Request could not be determined." } };
			onFail(urlError, $modal);
			return;
		}
		
		if (noteText == null || noteText.length <= 0) {
			return;
		}

		var $isPrivate = $modal.find("input[type='checkbox']");

		if ($isPrivate.length > 0) {
			isPrivate = $isPrivate.prop('checked');
		}

		var $file = $modal.find("input[type='file']");

		$button.attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");

		if (window.FormData !== undefined)
		{
			if ($file.length > 0) {
				if (typeof ($file)[0].files !== typeof undefined && ($file)[0].files.length > 0) {
					file = ($file)[0].files[0];
				}
			}

			var formData = new FormData();
			formData.append("regardingEntityLogicalName", target.LogicalName);
			formData.append("regardingEntityId", target.Id);
			formData.append("text", noteText);
			formData.append("isPrivate", isPrivate);
			formData.append("file", file);
			formData.append("attachmentSettings", JSON.stringify($this._attachmentSettings));

			$.ajax({
				url: url,
				type: 'POST',
				data: formData,
				mimeType: "multipart/form-data",
				contentType: false,
				cache: false,
				processData: false
			}).done(function () {
				$this._addSuccess = true;
				$element.trigger("refresh");
				$modal.modal("hide");
			}).fail(function (jqXhr) {
				var contentType = jqXhr.getResponseHeader("content-type");
				var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
				onFail(error, $modal);
			}).always(function () {
				$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
			});
		}
		else
		{
			onFail({ Message: "Your browser does not support FormData." }, $modal);
		}
	}

	entityNotes.prototype.updateNote = function ($modal) {
		var $this = this;
		var $element = $this._element;
		var url = $this._serviceUrlEdit;
		var $button = $modal.find(".modal-footer button.primary");
		var noteText = $modal.find("textarea").val();
		var subject = $modal.data("subject") || "";
		var isPrivate = false;
		var file = null;

		if (url == null || url == '') {
			var urlError = { Message: "System Error", InnerError: { Message: "The URL to the service for this Update Note Request could not be determined." } };
			onFail(urlError, $modal);
			return;
		}

		if (noteText == null || noteText.length <= 0) {
			return;
		}

		var $isPrivate = $modal.find("input[type='checkbox']");

		if ($isPrivate.length > 0) {
			isPrivate = $isPrivate.prop('checked');
		}

		var $file = $modal.find("input[type='file']");

		$button.attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");

		if (window.FormData !== undefined) {
			if ($file.length > 0) {
				if (typeof ($file)[0].files !== typeof undefined && ($file)[0].files.length > 0) {
					file = ($file)[0].files[0];
				}
			}

			var id = $modal.data("id");

			if (!id || id == '') {
				var idError = { Message: "System Error", InnerError: { Message: "Failed to determine record ID." } };
				onFail(idError, $modal);
				return;
			}
			
			var formData = new FormData();
			formData.append("id", id);
			formData.append("text", noteText);
			formData.append("subject", subject);
			formData.append("isPrivate", isPrivate);
			formData.append("file", file);
			formData.append("attachmentSettings", JSON.stringify($this._attachmentSettings));

			$.ajax({
				url: url,
				type: 'POST',
				data: formData,
				mimeType: "multipart/form-data",
				contentType: false,
				cache: false,
				processData: false
			}).done(function () {
				$this._editSuccess = true;
				$element.trigger("refresh");
				$modal.modal("hide");
			}).fail(function (jqXhr) {
				var contentType = jqXhr.getResponseHeader("content-type");
				var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
				onFail(error, $modal);
			}).always(function () {
				$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
			});
		}
		else {
			onFail({ Message: "Your browser does not support FormData." }, $modal);
		}
	}

	entityNotes.prototype.initializePagination = function (data) {
		// requires ~/js/jquery.bootstrap-pagination.js
		if (typeof data === typeof undefined || data === false || data == null) {
			return;
		}

		if ((typeof data.PageSize === typeof undefined || data.PageSize === false || data.PageSize == null) ||
		(typeof data.PageCount === typeof undefined || data.PageCount === false || data.PageCount == null) ||
		(typeof data.PageNumber === typeof undefined || data.PageNumber === false || data.PageNumber == null) ||
		(typeof data.ItemCount === typeof undefined || data.ItemCount === false || data.ItemCount == null)) {
			return;
		}

		var $this = this;
		var $element = $this._element;
		var $pagination = $element.find(".notes-pagination");

		if (data.PageCount <= 1) {
			$pagination.hide();
			return;
		}

		$pagination
			.data("pagesize", data.PageSize)
			.data("pages", data.PageCount)
			.data("current-page", data.PageNumber)
			.data("count", data.ItemCount)
			.unbind("click")
			.pagination({
				total_pages: $pagination.data("pages"),
				current_page: $pagination.data("current-page"),
				callback: function (event, pg) {
					event.preventDefault();
					var $li = $(event.target).closest("li");
					if ($li.not(".disabled").length > 0 && $li.not(".active").length > 0) {
						$this.load(pg);
					}
				}
			})
			.show();
	}

	function onFail(error, $modal) {
		if (typeof error !== typeof undefined && error !== false && error != null) {
			console.log(error);

			var $body = $modal.find(".modal-body");

			var $error = $modal.find(".alert-danger.error");

			if ($error.length == 0) {
				$error = $("<div></div>").addClass("alert alert-block alert-danger error clearfix");
			} else {
				$error.empty();
			}

			if (typeof error.InnerError !== typeof undefined && typeof error.InnerError.Message !== typeof undefined && error.InnerError.Message !== false && error.InnerError.Message != null) {
				if (typeof error.InnerError.Message === 'number') {
					$error.append("<p><span class='fa fa-exclamation-triangle' aria-hidden='true'></span> " + error.InnerError.Message + " Error</p>");
				} else {
					$error.append("<p><span class='fa fa-exclamation-triangle' aria-hidden='true'></span> " + error.InnerError.Message + "</p>");
				}
			} else if (typeof error.Message !== typeof undefined && error.Message !== false && error.Message != null) {
				if (typeof error.Message === 'number') {
					$error.append("<p><span class='fa fa-exclamation-triangle' aria-hidden='true'></span> " + error.Message + " Error</p>");
				} else {
					$error.append("<p><span class='fa fa-exclamation-triangle' aria-hidden='true'></span> " + error.Message + "</p>");
				}
			}

			$body.prepend($error);
		}
	}
}(jQuery));