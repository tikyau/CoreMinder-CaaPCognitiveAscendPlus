(function ($) {

	var actionTargetEntityForm = 0;
	var actionTargetWebPage = 1;
	var actionTargetUrl = 2;

	function entityGrid(element) {
		this._element = $(element);
		this._layouts = this._element.data("view-layouts");
		this._gridClass = this._element.data("grid-class");
		this._serviceUrlForGet = this._element.data("get-url");
		this._deferLoading = this._element.data("defer-loading");
		this._enableActions = this._element.data("enable-actions");
		this._selectMode = this._element.data("select-mode");
		this._columnWidthStyle = this._element.data("column-width-style");
		this._applyRelatedRecordFilter = this._element.data("apply-related-record-filter");
		this._filterRelationshipName = this._element.data("filter-relationship-name");
		this._filterEntityName = this._element.data("filter-entity-name");
		this._filterAttributeName = this._element.data("filter-attribute-name");
		this._allowFilterOff = this._element.data("allow-filter-off");
		this._toggleFilterText = this._element.data("toggle-filter-text");
		this._userIsAuthenticated = this._element.data("user-isauthenticated");
		this._userParentAccountName = this._element.data("user-parent-account-name");
		this._metaFilter = null;
		var that = this;

		$(element).on("refresh", function () {
			that.load();
		});

		$(element).on("metafilter", function (e, metaFilter) {
			that._metaFilter = metaFilter;
			that.load();
		});
	}

	$(document).ready(function () {
		$(".entity-grid").each(function () {
			new entityGrid($(this)).render();
		});

		$(".entitylist").on("click.entitylist", ".entitylist-filter-option-breaker a", function(e) {
			e.preventDefault();
			e.stopPropagation();

			var collapsed = $(this).data("collapsed");

			var ul = $(this).closest("ul");

			var pageSize = ul.data("pagesize");

			ul.children("li.entitylist-filter-option").each(function (i) {
				if (i >= pageSize) {
					if (collapsed) {
						$(this).attr("aria-hidden", "false");
					} else {
						$(this).attr("aria-hidden", "true");
					}
				}
			});

			$(this).find(".fa").toggleClass("fa-caret-down", collapsed).toggleClass("fa-caret-up", !collapsed);
			$(this).find(".expand-label").text(collapsed ? "Less" : "More");

			$(this).data("collapsed", !collapsed);
		});
	});

	entityGrid.prototype.render = function (forceLoad) {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var cssClass = $this._gridClass;
		var selectedViewId = $element.data("selected-view");
		var deferLoading = $this._deferLoading;
		var enableActions = $this._enableActions;
		var allowFilterOff = $this._allowFilterOff;
		var toggleFilterText = $this._toggleFilterText;
		var selectMode = $this._selectMode;
		var columnWidthStyle = $this._columnWidthStyle;
		var userIsAuthenticated = $this._userIsAuthenticated;
		var userParentAccountName = $this._userParentAccountName;
		var activeLayoutIndex = 0;
		if (typeof selectedViewId !== typeof undefined && selectedViewId !== false && selectedViewId != null && selectedViewId != '') {
			activeLayoutIndex = indexOf(layouts, "Id", selectedViewId);
			if (activeLayoutIndex == -1) {
				activeLayoutIndex = 0;
			}
		}
		this._activeLayoutIndex = activeLayoutIndex;
		var layout = layouts[activeLayoutIndex];
		var queryString = URI(window.location).search(true);
		var iframed = inIframe();
		$this._metaFilter = queryString[layout.Configuration.FilterSettings.FilterQueryStringParameterName || "mf"] || null;
		var enableFilter = (userIsAuthenticated && userParentAccountName && userParentAccountName != '' && ((layout.Configuration.FilterPortalUserAttributeName != null && layout.Configuration.FilterPortalUserAttributeName != '') && (layout.Configuration.FilterAccountAttributeName != null && layout.Configuration.FilterAccountAttributeName != '')));
		var $container = $element.children(".view-grid");
		$container.empty();
		$element.children(".view-toolbar").remove();
		var $toolbar = $("<div></div>").addClass("view-toolbar").addClass("grid-actions").addClass("clearfix");
		var addToolbar = false;
		if (allowFilterOff) {
			var $toggleFilter = $("<a></a>")
				.attr("href", "#")
				.addClass("toggle-related-filter")
				.data("placement", "bottom")
				.data("toggle", "tooltip")
				.addClass("btn btn-default")
				.addClass("active")
				.html("<span class='fa fa-filter' aria-hidden='true'></span>")
				.on("click", function(e) {
					e.preventDefault();
					$(this).toggleClass("active");
					$this.load(0, $element.children(".view-toolbar").find(".view-filter-select").data("filter"), !$(this).hasClass("active"));
				});
			if (toggleFilterText && toggleFilterText != '') {
				$toggleFilter.attr("title", toggleFilterText).tooltip();
			}
			$toggleFilter.prependTo($toolbar);
			addToolbar = true;
		}
		var search = layout.Configuration.Search;
		if (search.Enabled) {
			var $searchContainer = $("<div></div>").addClass("input-group").addClass("pull-right").addClass("view-search").addClass("entitylist-search");
			var $searchButton = $("<button></button>")
				.attr("type", "button")
				.addClass("btn")
				.addClass("btn-default")
				.append($("<span></span>").addClass("sr-only").html("Search Results"))
				.append($("<span></span>").addClass("fa").addClass("fa-search"))
				.on("click", function (e) {
					e.preventDefault();
					$this.load(1);
				});
			var $searchButtonGroup = $("<div></div>").addClass("input-group-btn").append($searchButton);
			var $searchInput = $("<input/>")
				.attr("placeholder", search.PlaceholderText)
				.data("toggle", "tooltip")
				.attr("title", search.TooltipText)
				.tooltip()
				.addClass("query")
				.addClass("form-control")
				.val("")
				.on("keypress", function(e) {
					var keyCode = e.keyCode ? e.keyCode : e.which;
					if (keyCode == '13') {
						e.preventDefault();
						$searchButton.trigger("click");
						$(this).trigger("focus");
					}
				});
			$searchContainer.append($searchInput).append($searchButtonGroup).prependTo($toolbar);
			addToolbar = true;
		}
		if (enableActions) {
			window.addEventListener('message', function (e) {
				var origin = null;
				if (!window.location.origin) {
					origin = window.location.protocol + "//" + window.location.hostname + (window.location.port ? ':' + window.location.port : ''); // IE does not set origin
				} else {
					origin = window.location.origin;
				}
				if (e.origin != origin) {
					return;
				}
				if (e.data == 'Success') {
					$element.children(".modal-form").modal('hide');
					$element.trigger("refresh");
				}
			});
		}
		$element.children(".modal-form-details").on('hidden.bs.modal.entitygrid', function () {
			var $iframe = $(this).find("iframe");
			$iframe.attr("src", "");
		});
		$element.children(".modal-form-edit").on('hidden.bs.modal.entitygrid', function () {
			var $iframe = $(this).find("iframe");
			$iframe.attr("src", "");
		});
		if (enableActions && layout.Configuration.InsertActionLink.Enabled && layout.Configuration.InsertActionLink.URL) {
			var $insertLink = $("<a></a>").attr("href", getUrlFromActionLink(layout.Configuration.InsertActionLink)).addClass("btn").addClass("btn-primary").addClass("pull-right").addClass("action").html(layout.Configuration.InsertActionLink.Label).attr("title", layout.Configuration.InsertActionLink.Tooltip);
			if (iframed) $insertLink.attr("target", "_top");
			$insertLink.prependTo($toolbar);
			addToolbar = true;
		}
		if (enableActions) {
			// Loop through the ViewActionLinks and output download links only at this time.
			$.each(layout.Configuration.ViewActionLinks, function (i, viewActionLink) {
				if (!viewActionLink.Enabled)
					return;
				switch (viewActionLink.Type) {
					case 3: //Insert Action
						var href = "#";

						var refEntityName = $element.data("ref-entity");
						var refEntityId = $element.data("ref-id");
						var refRelationship = $element.data("ref-rel");
						var refRelationshipRole = $element.data("ref-rel-role");
						var refQueryString = "";

						if (refEntityName && refEntityId && refRelationship) {
							refQueryString += "refentity=" + refEntityName + "&refid=" + refEntityId + "&refrel=" + refRelationship;
							if (refRelationshipRole) {
								refQueryString += "&refrelrole=" + refRelationshipRole;
							}
						}

						if (viewActionLink.Target == actionTargetWebPage || viewActionLink.Target == actionTargetUrl && viewActionLink.URL != null) {
							href = URI(getUrlFromActionLink(viewActionLink)).toString();
							var character = href.indexOf("?") >= 0 ? "&" : "?";
							href += character + refQueryString;
						}

						var $insertActionLink = $("<a></a>").attr("href", href).addClass("btn").addClass("btn-primary").addClass("pull-right").addClass("action").html(viewActionLink.Label).attr("title", viewActionLink.Tooltip);
						if (href != "#" && iframed) $insertActionLink.attr("target", "_top");
						if (viewActionLink.Target == actionTargetEntityForm && viewActionLink.EntityForm) {
							var $insertModal = $element.children(".modal-form-insert");
							$insertModal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
								var $iframe = $(this).find("iframe");
								$iframe.attr("src", "");
								$insertActionLink.trigger("focus");
							});
							$insertActionLink.on("click", function (e) {
								e.preventDefault();
								var $iframe = $insertModal.find("iframe");
								var page = $iframe.data("page");
								var entityformid = viewActionLink.EntityForm.Id.toString();
								var src = page + "?entityformid=" + entityformid + "&languagecode=" + layout.Configuration.LanguageCode;
								src += "&" + refQueryString;

								$iframe.attr("src", src);
								$iframe.on("load", function () {
									$insertModal.find(".form-loading").hide();
									$insertModal.find("iframe").contents().find("#EntityFormControl").show();
								});
								$insertModal.find(".form-loading").show();
								$insertModal.find("iframe").contents().find("#EntityFormControl").hide();
								$insertModal.modal();
							});
						}
						$insertActionLink.prependTo($toolbar);
						addToolbar = true;
						break;
					case 5: //Associate Action
						var $associateLink = $("<a></a>").attr("href", "#").addClass("btn").addClass("btn-info").addClass("pull-right").addClass("action").html(viewActionLink.Label).attr("title", viewActionLink.Tooltip);
						var $associateModal = $element.find(".associate-lookup .modal-associate");
						$associateModal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
							$associateLink.trigger("focus");
						});
						$associateLink.on("click", function (e) {
							e.preventDefault();
							$associateModal.modal();
						});
						$associateLink.prependTo($toolbar);
						addToolbar = true;
						break;
					case 8: //Download Action
						var $downloadButton = $("<a></a>").attr("href", "#")
							.addClass("entitylist-download")
							.addClass("btn")
							.addClass("btn-info")
							.addClass("pull-right")
							.addClass("action")
							.attr("title", viewActionLink.Tooltip)
							.html(viewActionLink.Label);
						// Add download click event handler.
						$downloadButton.on("click", function (e) {
							e.preventDefault();
							$downloadButton.attr("disabled", "disabled");

							var data = {};
							data.viewName = layout.ViewName;
							data.columns = layout.Columns;
							data.base64SecureConfiguration = layout.Base64SecureConfiguration;
							data.sortExpression = layout.SortExpression;
							data.search = $searchInput ? $searchInput.val() : null;
							data.filter = $this.getCurrentFilter();
							data.metaFilter = $this._metaFilter;
							data.page = $element.children(".view-pagination").data("current-page");
							data.pageSize = $element.children(".view-pagination").data("pagesize");
							
							var jsonData = JSON.stringify(data);
							$.ajax({
								type: 'POST',
								dataType: "json",
								contentType: 'application/json; charset=utf-8',
								url: viewActionLink.URL.Path,
								data: jsonData,
								global: false,
								success: function(result) {
									if (result.success) {
										window.location = viewActionLink.URL.Path + "?key=" + result.sessionKey;
									}
								}
							}).fail(function (jqXhr) {
								var contentType = jqXhr.getResponseHeader("content-type");
								var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
								displayErrorAlert(error, $element);
							}).always(function () {
								$downloadButton.removeAttr("disabled", "disabled");
							});
						});
						$downloadButton.prependTo($toolbar);
						addToolbar = true;
						break;
				}
			});
		}
		if (enableFilter) {
			var $filterSelectionContainer = $("<ul></ul>").addClass("view-filter-select").addClass("nav").addClass("nav-pills").addClass("pull-left").data("filter", "user");
			var $filterSelectionDropdown = $("<li></li>").addClass("dropdown");
			var $filterSelectionLink = $("<a></a>")
				.addClass("dropdown-toggle")
				.attr("data-toggle", "dropdown")
				.attr("href", "#")
				.append($("<span></span>").addClass("fa").addClass("fa-filter").attr('aria-hidden', 'true'))
				.append($("<span></span>").addClass("title").html(" " + layout.Configuration.FilterByUserOptionLabel))
				.append($("<span></span>").addClass("caret").attr('aria-hidden', 'true'))
				.dropdown();
			$filterSelectionDropdown.append($filterSelectionLink);
			var $filterSelectionDropdownMenu = $("<ul></ul>").addClass("dropdown-menu");
			var $liUserFilter = $("<li></li>");
			$liUserFilter.addClass("active");
			var $userFilterLink = $("<a></a>")
				.attr("href", "#")
				.addClass("filter-user")
				.html(layout.Configuration.FilterByUserOptionLabel)
				.on("click", function (e) {
					e.preventDefault();
					$filterSelectionContainer.find("li").removeClass("active");
					$(this).closest("li").toggleClass("active");
					$filterSelectionContainer.data("filter", "user");
					$this.changeFilter();
				});
			$liUserFilter.append($userFilterLink).appendTo($filterSelectionDropdownMenu);
			var $liUserAccountFilter = $("<li></li>");
			var $userAccountFilterLink = $("<a></a>")
				.attr("href", "#")
				.addClass("filter-account")
				.html(userParentAccountName)
				.on("click", function (e) {
					e.preventDefault();
					$filterSelectionContainer.find("li").removeClass("active");
					$(this).closest("li").toggleClass("active");
					$filterSelectionContainer.data("filter", "account");
					$this.changeFilter();
				});
			$liUserAccountFilter.append($userAccountFilterLink).appendTo($filterSelectionDropdownMenu);
			$filterSelectionDropdown.append($filterSelectionDropdownMenu).appendTo($filterSelectionContainer);
			$filterSelectionContainer.prependTo($toolbar);
			addToolbar = true;
		}
		if (layouts.length > 1) {
			var currentViewDisplayName = layout.Configuration.ViewDisplayName;
			if (currentViewDisplayName == null || currentViewDisplayName == "") {
				currentViewDisplayName = layout.ViewName;
			}
			var $viewSelectionContainer = $("<ul></ul>").addClass("view-select").addClass("nav").addClass("nav-pills").addClass("pull-left");
			var $viewSelectionDropdown = $("<li></li>").addClass("dropdown");
			var $viewSelectionLink = $("<a></a>")
				.addClass("dropdown-toggle")
				.attr("data-toggle", "dropdown")
				.attr("href", "#")
				.append($("<span></span>").addClass("fa").addClass("fa-list").attr('aria-hidden', 'true'))
				.append($("<span></span>").addClass("title").html(" " + currentViewDisplayName))
				.append($("<span></span>").addClass("caret").attr('aria-hidden', 'true'))
				.dropdown();
			$viewSelectionDropdown.append($viewSelectionLink);
			var $viewSelectionDropdownMenu = $("<ul></ul>").addClass("dropdown-menu");
			$.each(layouts, function (i, layoutItem) {
				var $li = $("<li></li>");
				if (i == activeLayoutIndex) {
					$li.addClass("active");
				}
				var viewDisplayName = layoutItem.Configuration.ViewDisplayName;
				if (viewDisplayName == null || viewDisplayName == "") {
					viewDisplayName = layoutItem.ViewName;
				}
				var $a = $("<a></a>")
					.attr("href", "#")
					.data("id", layoutItem.Id)
					.html(viewDisplayName)
					.on("click", function (e) {
						e.preventDefault();
						var id = $(this).data("id");
						$this.changeView(id);
					});
				$li.append($a).appendTo($viewSelectionDropdownMenu);
			});
			$viewSelectionDropdown.append($viewSelectionDropdownMenu).appendTo($viewSelectionContainer);
			$viewSelectionContainer.prependTo($toolbar);
			addToolbar = true;
		}
		if (addToolbar) {
			$element.prepend($toolbar);
		}
		if (layout.Columns.length == 0) {
			return;
		}
		var $table = $("<table></table>").attr("aria-live", "polite").attr("aria-relevant", "additions");
		$table.addClass("table");
		$table.addClass(cssClass);
		if (selectMode == "Single" || selectMode == "Multiple") {
			$table.addClass("table-hover");
		}
		if (columnWidthStyle == "Pixels") {
			$table.attr("style", "width:" + layout.ColumnsTotalWidth + "px !important; max-width:" + layout.ColumnsTotalWidth + "px !important;");
		}
		$table.data("sort-expression", layout.SortExpression);
		var $thead = $("<thead></thead>");
		var $tbody = $("<tbody></tbody>");
		var $tr = $("<tr></tr>");
		var sorts = parseSortExpression(layout.SortExpression);

		$.each(layout.Columns, function (i, column) {
			var $th = $("<th></th>")
				.data("field", column.LogicalName);
			if (columnWidthStyle == "Pixels") {
				$th.attr("style", "width:" + column.Width + "px;");
			} else {
				$th.attr("style", "width:" + column.WidthAsPercent + "%;");
			}
			if (!column.SortDisabled) {
				$th.addClass("sort-enabled").data("sort-name", column.LogicalName);
				var $a = $("<a></a>").attr("href", "#");
				var sortIndex = indexOf(sorts, "name", column.LogicalName);
				if (sortIndex != -1) {
					var sort = sorts[sortIndex];
					if (sort.direction.toLowerCase().match("asc") != -1) {
						$a.append(column.Name + " ").append($("<span></span>").addClass("fa").addClass("fa-arrow-up").attr('aria-hidden', 'true'))
							.append("<span class='sr-only sort-hint'>. Activate to sort in descending order</span>");
						$th.data("sort-dir", "ASC").attr("aria-sort", "ascending")
							.addClass("sort")
							.addClass("sort-asc");
					} else {
						$a.append(column.Name + " ").append($("<span></span>").append("Activate to sort in ascending order").addClass("fa").addClass("fa-arrow-down").attr('aria-hidden', 'true'))
							.append("<span class='sr-only sort-hint'>. Activate to sort in ascending order</span>");
						$th.data("sort-dir", "DESC").attr("aria-sort", "descending")
							.addClass("sort")
							.addClass("sort-desc");
					}
				} else {
					$a.html(column.Name).append("<span class='sr-only sort-hint'>. Activate to sort in descending order</span>");
				}
				$a.appendTo($th);
			} else {
				$th.html(column.Name);
				$th.addClass("sort-disabled");
			}
			$th.appendTo($tr);
		});
		$tr.appendTo($thead);
		$thead.appendTo($table);
		$tbody.appendTo($table);
		$table.appendTo($container);
		$this.addSortEventHandlers();
		if (!deferLoading) {
			$this.load();
		} else if (forceLoad) {
			$this.load();
		}
	}

	entityGrid.prototype.changeView = function (id) {
		var $this = this;
		var $element = $this._element;
		$element.data("selected-view", id);
		$this.render(true);
	}

	entityGrid.prototype.changeFilter = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var selectedViewId = $element.data("selected-view");
		var $filterSelect = $element.children(".view-toolbar").find(".view-filter-select");
		var activeLayoutIndex = 0;
		if (typeof selectedViewId !== typeof undefined && selectedViewId !== false && selectedViewId != null && selectedViewId != '') {
			activeLayoutIndex = indexOf(layouts, "Id", selectedViewId);
			if (activeLayoutIndex == -1) {
				activeLayoutIndex = 0;
			}
		}
		this._activeLayoutIndex = activeLayoutIndex;
		var layout = layouts[activeLayoutIndex];
		var userIsAuthenticated = $this._userIsAuthenticated;
		var userParentAccountName = $this._userParentAccountName;
		var enableFilter = (userIsAuthenticated && userParentAccountName && userParentAccountName != '' && ((layout.Configuration.FilterPortalUserAttributeName != null && layout.Configuration.FilterPortalUserAttributeName != '') || (layout.Configuration.FilterAccountAttributeName != null && layout.Configuration.FilterAccountAttributeName != '')));
		if (!enableFilter) {
			return;
		}
		var currentFilter = $this.getCurrentFilter();
		var currentFilterName;
		if (currentFilter == "user") {
			currentFilterName = layout.Configuration.FilterByUserOptionLabel;
		} else {
			currentFilterName = userParentAccountName;
		}
		$filterSelect.find("li.dropdown").find("span.title").html(" " + currentFilterName);
		$this.load(0, currentFilter);
	}

	entityGrid.prototype.getCurrentFilter = function() {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var selectedViewId = $element.data("selected-view");
		var $filterSelect = $element.children(".view-toolbar").find(".view-filter-select");
		var activeLayoutIndex = 0;
		if (typeof selectedViewId !== typeof undefined && selectedViewId !== false && selectedViewId != null && selectedViewId != '') {
			activeLayoutIndex = indexOf(layouts, "Id", selectedViewId);
			if (activeLayoutIndex == -1) {
				activeLayoutIndex = 0;
			}
		}
		this._activeLayoutIndex = activeLayoutIndex;
		var layout = layouts[activeLayoutIndex];
		var userIsAuthenticated = $this._userIsAuthenticated;
		var userParentAccountName = $this._userParentAccountName;
		var defaultFilter = "user";

		if (!userIsAuthenticated) {
			return null;
		}

		if (layout.Configuration.FilterPortalUserAttributeName == null || layout.Configuration.FilterPortalUserAttributeName == '') {
			if (!userParentAccountName || userParentAccountName == '' || layout.Configuration.FilterAccountAttributeName == null || layout.Configuration.FilterAccountAttributeName == '') {
				return null;
			} else {
				return "account";
			}
		}

		if (!userParentAccountName || userParentAccountName == '' || layout.Configuration.FilterAccountAttributeName == null || layout.Configuration.FilterAccountAttributeName == '') {
			return defaultFilter;
		}

		var currentFilter = $filterSelect.data("filter") || defaultFilter;
		if (currentFilter == "user" || currentFilter == "account") {
			return currentFilter;
		}
		return defaultFilter;
	}

	entityGrid.prototype.isRelatedRecordFilterEnabled = function() {
		var $this = this;
		var $element = $this._element;
		var applyRelatedRecordFilter;
		if ($element.children(".view-toolbar").find(".toggle-related-filter").length == 0) {
			applyRelatedRecordFilter = $this._applyRelatedRecordFilter || false;
		} else {
			applyRelatedRecordFilter = $element.children(".view-toolbar").find(".toggle-related-filter").hasClass("active");
		}
		return applyRelatedRecordFilter;
	}

	entityGrid.prototype.load = function (page, filter, relatedRecordfilterOff, callingElement) {
		var $this = this;
		var $element = $this._element;
		var $table = $element.children(".view-grid").find("table");
		var $tbody = $("<tbody></tbody>");
		var $errorMessage = $element.children(".view-error");
		var $emptyMessage = $element.children(".view-empty");
		var $accessDeniedMessage = $element.children(".view-access-denied");
		var $loadingMessage = $element.children(".view-loading");
		var $pagination = $element.children(".view-pagination");
		var $search = $element.children(".view-toolbar").find(".entitylist-search input.query");
		var sortExpression = $table.data("sort-expression");
		var serviceUrlGet = $this._serviceUrlForGet;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var configuration = layout.Configuration;
		var selectMode = $this._selectMode;
		var enableActions = $this._enableActions;
		var applyRelatedRecordFilter;
		var filterRelationshipName = $this._filterRelationshipName;
		var filterEntityName = $this._filterEntityName;
		var filterAttributeName = $this._filterAttributeName;
		var filterValue = null;
		var entityName = $element.closest(".entity-form").children("input[type='hidden'][id$='EntityName']").val();
		var entityId = $element.closest(".entity-form").children("input[type='hidden'][id$='EntityID']").val();
		var metaFilter = $this._metaFilter;

		$element.children(".view-grid").find("tbody").remove();

		$errorMessage.hide().prop("aria-hidden", true);
		$emptyMessage.hide().prop("aria-hidden", true);
		$accessDeniedMessage.hide().prop("aria-hidden", true);

		$loadingMessage.show().prop("aria-hidden", false);

		$pagination.data("pagesize", configuration.PageSize);
		var pageNumber = $pagination.data("current-page");
		if (pageNumber == null || pageNumber == '') {
			pageNumber = 1;
		}
		filter = filter || $this.getCurrentFilter();
		page = page || pageNumber;
		var pageSize = $pagination.data("pagesize");
		if (pageSize == null || pageSize == '') {
			pageSize = 10;
		}
		var search = $search.val();
		if (typeof search === typeof undefined || search === false) {
			search = null;
		}

		if (typeof relatedRecordfilterOff === typeof undefined) {
			applyRelatedRecordFilter = $this.isRelatedRecordFilterEnabled();
		} else {
			applyRelatedRecordFilter = !relatedRecordfilterOff;
		}
		if (applyRelatedRecordFilter) {
			if (filterEntityName && entityName && entityName != '' && filterEntityName == entityName && entityId && entityId != '') {
				filterValue = entityId;
			} else if (filterAttributeName && filterAttributeName != '') {
				var $dependentField = $("#" + filterAttributeName);
				if ($dependentField.length > 0) {
					var dependentValue = $dependentField.val();
					if (dependentValue && dependentValue != '') {
						filterValue = dependentValue;
					}
				}
			}
		}

		$this.hidePagination();

		$this.getData(serviceUrlGet, configuration, layout.Base64SecureConfiguration, sortExpression, filter, metaFilter, search, page, pageSize, applyRelatedRecordFilter, filterRelationshipName, filterEntityName, filterAttributeName, filterValue,
			function (data) {
				// done
				$tbody.empty();
				if (typeof data === typeof undefined || data === false || data == null) {
					$emptyMessage.fadeIn().prop("aria-hidden", false);
					$element.trigger("loaded");
					return;
				}
				if (typeof data.Records !== typeof undefined && data.Records !== false && (data.Records == null || data.Records.length == 0)) {
					$emptyMessage.fadeIn().prop("aria-hidden", false);
					$element.trigger("loaded");
					return;
				}
				if (typeof data.AccessDenied !== typeof undefined && data.AccessDenied !== false && data.AccessDenied) {
					$accessDeniedMessage.fadeIn().prop("aria-hidden", false);
					$element.trigger("loaded");
					return;
				}
				var columns = $.map($table.find("th"), function (e) {
					return $(e).data('field');
				});

				var nameColumn = columns.length == 0 ? "" : columns[0] == "col-select" ? columns[1] : columns[0];

				$element.data("total-record-count", data.ItemCount);

				for (var i = 0; i < data.Records.length; i++) {
					var record = data.Records[i];
					var nameAttributeIndex = indexOf(record.Attributes, "Name", nameColumn);
					var name = "";
					if (nameAttributeIndex != -1) {
						name = record.Attributes[nameAttributeIndex].DisplayValue;
					}
					var $tr = $("<tr></tr>")
						.attr("data-id", record.Id)
						.attr("data-entity", configuration.EntityName)
						.attr("data-name", name)
						.on("focus", function() {
							$(this).addClass("active");
						})
						.on("blur", function() {
							$(this).removeClass("active");
						});

					var select = $.noop;
					if (selectMode == "Multiple") {
						select = function() {
							var selected = $(this).attr("aria-checked") === "true";
							$(this).children("td:first").find(".fa-fw").toggleClass("fa-check", !selected);
							$(this).toggleClass("selected").toggleClass("info").attr("aria-checked", !selected);
						};
						$tr.on("click", select).on("keypress", function(e) {
							var keyCode = e.keyCode ? e.keyCode : e.which;
							if (keyCode == '13' || keyCode == '32') {
								e.preventDefault();
								select.apply(this, arguments);
							}
						}).attr("tabindex", 0).attr("role", "checkbox").attr("aria-checked", "false");
					} else if (selectMode == "Single") {
						select = function() {
							var selected = $(this).attr("aria-checked") === "true";

							var $selectedRows = $table.find("tr.selected");
							$selectedRows.each(function() {
								$(this).children("td:first").find(".fa-fw").removeClass("fa-check");
								$(this).removeClass("selected").removeClass("info").attr("aria-checked", false);
							});

							if (!selected) {
								$(this).children("td:first").find(".fa-fw").addClass("fa-check");
								$(this).addClass("selected").addClass("info").attr("aria-checked", true);
							}
						};
						$tr.on("click", select).on("keypress", function(e) {
							var keyCode = e.keyCode ? e.keyCode : e.which;
							if (keyCode == '13' || keyCode == '32') {
								e.preventDefault();
								select.apply(this, arguments);
							}
						}).attr("tabindex", 0).attr("role", "radio").attr("aria-checked", "false");
					}

					for (var j = 0; j < columns.length; j++) {
						var found = false;
						for (var k = 0; k < record.Attributes.length; k++) {
							var attribute = record.Attributes[k];
							if (attribute.Name == columns[j]) {
								var html = $this.formatAttributeValue(attribute, attribute.AttributeMetadata, j, record) || attribute.DisplayValue;
								var $td = $("<td></td>")
									.attr("data-type", attribute.Type)
									.attr("data-attribute", attribute.Name)
									.attr("data-value", typeof attribute.Value === 'object' ? JSON.stringify(attribute.Value) : attribute.Value)
									.html(html);
								if (attribute.AttributeMetadata) {
									$td.data("attribute-metadata", attribute.AttributeMetadata);
								}
								$tr.append($td);
								found = true;
								break;
							}
						}
						if (!found) {
							if (columns[j] == "col-action") {
								var $actions = $("<td></td>");
								var $dropdown = $("<div class='dropdown action'></div>");
								$("<a class='btn btn-default btn-xs'><span class='fa fa-chevron-circle-down' aria-hidden='true'></span></a>")
									.attr("href", "#")
									.attr("data-toggle", "dropdown")
									.appendTo($dropdown);
								var $dropdownMenu = $("<ul class='dropdown-menu' role='menu'><ul>");
								// start - hack to fix an issue with table container's overflow scroll affecting dropdown menu
								$dropdownMenu.css("position", "fixed").appendTo($dropdown);
								$dropdown.on("show.bs.dropdown", function () {
									var rect = $(this).get(0).getBoundingClientRect();
									var $dropdownmenu = $(this).find(".dropdown-menu:first");
									var docWidth = $(window).width();
									var isDropdownMenuVisible = (rect.left + $dropdownmenu.outerWidth() <= docWidth);
									if (isDropdownMenuVisible) {
										$dropdownmenu.css("left", rect.left).css("top", rect.top + $(this).outerHeight());
									} else {
										$dropdownmenu.css("left", rect.left + rect.width - $dropdownmenu.outerWidth()).css("top", rect.top + $(this).outerHeight());
									}
								});
								$(window).scroll(function () {
									var $openDropdown = $element.find(".dropdown.action.open");
									if ($openDropdown.length == 0) {
										return;
									}
									$openDropdown.removeClass("open");
								});
								// end - hack
								var iframed = inIframe();
								if (enableActions && layout.Configuration.ItemActionLinks && layout.Configuration.ItemActionLinks.length > 0) {
									var actions = layout.Configuration.ItemActionLinks;
									for (var l = 0; l < actions.length; l++) {
										var action = actions[l];
										switch (action.Type) {
										case 1:
											if (action.Enabled) {
												var href = "#";
												if ((action.Target == actionTargetWebPage || action.Target == actionTargetUrl) && action.URL != null) {
													href = URI(getUrlFromActionLink(action)).addSearch(action.QueryStringIdParameterName, record.Id.toString());
												}
												var $li = $("<li role='presentation'></li>");
												var $a = $("<a class='details-link' role='menuitem' tabindex='-1'></a>").attr("href", href).attr("title", action.Tooltip).html(action.Label);
												if (href != "#" && iframed) $a.attr("target", "_top");
												if (action.Target == actionTargetEntityForm && action.EntityForm) {
													$a.addClass("launch-modal").attr("data-entityformid", action.EntityForm.Id);
												}
												$li.append($a).appendTo($dropdownMenu);
											}
											break;
										case 2:
											if (action.Enabled && record.CanWrite) {
												var hrefEdit = "#";
												if ((action.Target == actionTargetWebPage || action.Target == actionTargetUrl) && action.URL != null) {
													hrefEdit = URI(getUrlFromActionLink(action)).addSearch(action.QueryStringIdParameterName, record.Id.toString());
												}
												var $liEdit = $("<li role='presentation'></li>");
												var $aEdit = $("<a class='edit-link' role='menuitem' tabindex='-1'></a>").attr("href", hrefEdit).attr("title", action.Tooltip).html(action.Label);
												if (hrefEdit != "#" && iframed) $aEdit.attr("target", "_top");
												if (action.Target == actionTargetEntityForm && action.EntityForm) {
													$aEdit.addClass("launch-modal").attr("data-entityformid", action.EntityForm.Id);
												}
												$liEdit.append($aEdit).appendTo($dropdownMenu);
											}
											break;
										case 4:
											if (action.Enabled && record.CanDelete) {
												$("<li role='presentation'></li>")
													.append($("<a class='delete-link' role='menuitem' tabindex='-1' href='#'></a>").attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 6:
											if (action.Enabled && record.CanAppend) {
												$("<li role='presentation'></li>")
													.append($("<a class='disassociate-link' role='menuitem' tabindex='-1' href='#'></a>").attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 7:
											if (action.URL) {
												$("<li role='presentation'></li>")
													.append($("<a class='workflow-link' role='menuitem' tabindex='-1' href='#'></a>").attr("title", action.Tooltip).attr("data-workflowid", action.Workflow.Id).attr("data-url", getUrlFromActionLink(action)).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 9:
											if (action.Enabled && record.CanWrite && record.StateCode == "0") {
												$("<li role='presentation'></li>")
													.append($("<a class='close-case-link' role='menuitem' tabindex='-1' href='#'></a>")
														.attr("data-url", getUrlFromActionLink(action))
														.attr("data-redirecturl", getRedirectUrlFromActionLink(action))
														.attr("data-resolution", action.DefaultResolution)
														.attr("data-description", action.DefaultResolutionDescription)
														.attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 10:
											if (action.Enabled && record.CanWrite && record.StateCode == "0") {
												$("<li role='presentation'></li>")
													.append($("<a class='qualify-lead-link' role='menuitem' tabindex='-1' href='#'></a>")
														.attr("data-url", getUrlFromActionLink(action))
														.attr("data-redirecturl", getRedirectUrlFromActionLink(action))
														.attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 11:
											if (action.Enabled && record.CanWrite && record.StateCode == "1") {
												$("<li role='presentation'></li>")
													.append($("<a class='convert-quote-link' role='menuitem' tabindex='-1' href='#'></a>")
														.attr("data-url", getUrlFromActionLink(action))
														.attr("data-redirecturl", getRedirectUrlFromActionLink(action))
														.attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 12:
											if (action.Enabled && record.CanWrite && record.StateCode != "2") {
												$("<li role='presentation'></li>")
													.append($("<a class='convert-order-link' role='menuitem' tabindex='-1' href='#'></a>")
														.attr("data-url", getUrlFromActionLink(action))
														.attr("data-redirecturl", getRedirectUrlFromActionLink(action))
														.attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 13:
											if (action.Enabled && record.CanWrite) {
												$("<li role='presentation'></li>")
													.append($("<a class='calculate-opportunity-link' role='menuitem' tabindex='-1' href='#'></a>")
														.attr("data-url", getUrlFromActionLink(action))
														.attr("data-redirecturl", getRedirectUrlFromActionLink(action))
														.attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 14:
											if (action.Enabled && record.CanWrite && record.StateCode == "0") {
												$("<li role='presentation'></li>")
													.append($("<a class='resolve-case-link' role='menuitem' tabindex='-1' href='#'></a>")
														.attr("data-url", getUrlFromActionLink(action))
														.attr("data-redirecturl", getRedirectUrlFromActionLink(action))
														.attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 15:
											if (action.Enabled && record.CanWrite && record.StateCode != "0") {
												$("<li role='presentation'></li>")
													.append($("<a class='reopen-case-link' role='menuitem' tabindex='-1' href='#'></a>")
														.attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 16:
											if (action.Enabled && record.CanWrite && record.StateCode == "0") {
												$("<li role='presentation'></li>")
													.append($("<a class='cancel-case-link' role='menuitem' tabindex='-1' href='#'></a>")
														.attr("data-url", getUrlFromActionLink(action))
														.attr("data-redirecturl", getRedirectUrlFromActionLink(action))
														.attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 17:
											if (action.Enabled && record.CanWrite && record.StateCode != "1") {
												$("<li role='presentation'></li>")
													.append($("<a class='deactivate-link' role='menuitem' tabindex='-1' href='#'></a>")
														.attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 18:
											if (action.Enabled && record.CanWrite && record.StateCode != "0") {
												$("<li role='presentation'></li>")
													.append($("<a class='activate-link' role='menuitem' tabindex='-1' href='#'></a>")
														.attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 19:
											if (action.Enabled && record.CanWrite && record.StateCode == "0") {
												$("<li role='presentation'></li>")
													.append($("<a class='activate-quote-link' role='menuitem' tabindex='-1' href='#'></a>")
														.attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 20:
											if (action.Enabled && record.CanWrite && record.StateCode == "0" && record.StatusCode != "2") {
												$("<li role='presentation'></li>")
													.append($("<a class='set-opportunity-on-hold-link' role='menuitem' tabindex='-1' href='#'></a>")
														.attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 21:
											if (action.Enabled && record.CanWrite && record.StateCode == "0") {
												$("<li role='presentation'></li>")
													.append($("<a class='win-opportunity-link' role='menuitem' tabindex='-1' href='#'></a>")
														.attr("data-url", getUrlFromActionLink(action))
														.attr("data-redirecturl", getRedirectUrlFromActionLink(action))
														.attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 22:
											if (action.Enabled && record.CanWrite && record.StateCode == "0") {
												$("<li role='presentation'></li>")
													.append($("<a class='lose-opportunity-link' role='menuitem' tabindex='-1' href='#'></a>")
														.attr("data-url", getUrlFromActionLink(action))
														.attr("data-redirecturl", getRedirectUrlFromActionLink(action))
														.attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 23:
											if (action.Enabled && record.CanWrite && record.StateCode == "0") {
												$("<li role='presentation'></li>")
													.append($("<a class='generate-quote-from-opportunity-link' role='menuitem' tabindex='-1' href='#'></a>")
														.attr("data-url", getUrlFromActionLink(action))
														.attr("data-redirecturl", getRedirectUrlFromActionLink(action))
														.attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										case 25:
											if (action.Enabled && record.CanWrite && record.StateCode != "0") {
												$("<li role='presentation'></li>")
													.append($("<a class='reopen-opportunity-link' role='menuitem' tabindex='-1' href='#'></a>")
														.attr("title", action.Tooltip).html(action.Label))
													.appendTo($dropdownMenu);
											}
											break;
										}
									}
								}
								if ($dropdownMenu.children("li").length > 0) {
									$dropdown.find(".fa").addClass("fa-fw");
									$actions.append($dropdown);
								}
								$actions.appendTo($tr);
							} else {
								$tr.append("<td></td>");
							}
						}
					}

					if (selectMode == "Multiple" || selectMode == "Single") {
						$tr.children("td:first").append("<span class='fa fa-fw'></span>");
					}

					$tbody.append($tr);
				}

				if (selectMode === "Single") {
					$tbody.attr("role", "radiogroup");
				}

				$element.children(".view-grid").children("table").append($tbody.hide());
				$tbody.fadeIn();
				$this.initializePagination(data, callingElement);
				$this.addDetailsActionLinkClickEventHandlers();
				$this.addEditActionLinkClickEventHandlers();
				$this.addDeleteActionLinkClickEventHandlers();
				$this.addQualifyLeadActionLinkClickEventHandlers();
				$this.addCloseCaseActionLinkClickEventHandlers();
				$this.addResolveCaseActionLinkClickEventHandlers();
				$this.addReopenCaseActionLinkClickEventHandlers();
				$this.addCancelCaseActionLinkClickEventHandlers();
				$this.addConvertQuoteActionLinkClickEventHandlers();
				$this.addConvertOrderActionLinkClickEventHandlers();
				$this.addCalculateOpportunityActionLinkClickEventHandlers();
				$this.addActivateActionLinkClickEventHandlers();
				$this.addDeactivateActionLinkClickEventHandlers();
				$this.addActivateQuoteActionLinkClickEventHandlers();
				$this.addSetOpportunityOnHoldActionLinkClickEventHandlers();
				$this.addReopenOpportunityActionLinkClickEventHandlers();
				$this.addWinOpportunityActionLinkClickEventHandlers();
				$this.addLoseOpportunityActionLinkClickEventHandlers();
				$this.addGenerateQuoteFromOpportunityActionLinkClickEventHandlers();
				$this.addDisassociateActionLinkClickEventHandlers();
				$this.addWorkflowActionLinkClickEventHandlers();
				$element.trigger("loaded");
			},
			function (jqXhr, textStatus, errorThrown) {
				// fail
				$errorMessage.html($("<div class='alert alert-block alert-danger'></div>").html(errorThrown));
				$errorMessage.show().prop("aria-hidden", false);
			},
			function () {
				// always
				$loadingMessage.hide().prop("aria-hidden", true);
			});
	}

	entityGrid.prototype.getData = function (url, configuration, base64SecureConfiguration, sortExpression, filter, metaFilter, search, page, pageSize, applyRelatedRecordFilter, filterRelationshipName, filterEntityName, filterAttributeName, filterValue, done, fail, always) {
		done = $.isFunction(done) ? done : function () { };
		fail = $.isFunction(fail) ? fail : function () { };
		always = $.isFunction(always) ? always : function () { };
		if (!url || url == '') {
			always.call(this);
			fail.call(this, null, "error", "A required service url was not provided.");
			return;
		}
		if (!base64SecureConfiguration) {
			always.call(this);
			fail.call(this, null, "error", "A required secure ViewConfiguration was not provided.");
			return;
		}
		pageSize = pageSize || -1;
		var data = {};
		data.base64SecureConfiguration = base64SecureConfiguration;
		data.sortExpression = sortExpression;
		data.search = search;
		data.page = page;
		data.pageSize = pageSize;
		data.filter = filter;
		data.metaFilter = metaFilter;
		if (applyRelatedRecordFilter) {
			data.applyRelatedRecordFilter = applyRelatedRecordFilter;
			data.filterRelationshipName = filterRelationshipName;
			data.filterEntityName = filterEntityName;
			data.filterAttributeName = filterAttributeName;
			if (filterValue) {
				data.filterValue = filterValue;
			}
		}
		var jsonData = JSON.stringify(data);
		$.ajax({
			type: 'POST',
			dataType: "json",
			contentType: 'application/json; charset=utf-8',
			url: url,
			data: jsonData,
			global: false
		})
			.done(done)
			.fail(fail)
			.always(always);
	}

	entityGrid.prototype.formatAttributeValue = function (attribute, attributeMetadata, index, record) {
		if (!attribute) {
			return null;
		}
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var iframed = inIframe();
		var html;
		var buildDetailsLink = index == 0 && enableActions && layout.Configuration.DetailsActionLink.Enabled; // If first column and details link is enabled then cell needs to be made clickable
		switch (attribute.Type) {
		case "System.DateTime":
			var moment = window.moment;
			if (moment) {
				//convert to moment format
				var dateFormat = dateFormatConverter.convert($element.closest("[data-dateformat]").data("dateformat") || "M/d/yyyy", dateFormatConverter.dotNet, dateFormatConverter.momentJs);
				var timeFormat = dateFormatConverter.convert($element.closest("[data-timeformat]").data("timeformat") || "h:mm tt", dateFormatConverter.dotNet, dateFormatConverter.momentJs);
				var datetimeFormat = dateFormat + ' ' + timeFormat;

				if (attribute.FormattedValue.indexOf(":") != -1) { // not a date only field
					html = $("<time></time>").attr("datetime", attribute.DisplayValue).text(moment(attribute.Value).format(datetimeFormat));
				} else {
					html = $("<time></time>").attr("datetime", attribute.DisplayValue).addClass("date-only").text(moment(attribute.Value).format(dateFormat));
				}
			} else {
				html = $("<time></time>").attr("datetime", attribute.DisplayValue).text(attribute.DisplayValue);
			}
			break;
		case "System.String":
			if (!buildDetailsLink && typeof attributeMetadata !== typeof undefined && attributeMetadata != null && typeof attributeMetadata.Format !== typeof undefined) {
				if (attributeMetadata.Format == 0) { // Email
					var hrefEmail = attribute.DisplayValue;
					if (hrefEmail && hrefEmail != '') {
						if (!hrefEmail.toLowerCase().startsWith("mailto")) {
							hrefEmail = "mailto:" + hrefEmail;
						}
					}
					html = $("<a></a>").attr("href", hrefEmail).text(attribute.DisplayValue);
				} else if (attributeMetadata.Format == 3) { // Url
					var url = attribute.DisplayValue;
					if (url && url != '') {
						if (!url.toLowerCase().startsWith("http")) {
							url = "http://" + url;
						}
					}
					var $link = $("<a></a>");
					$link.attr("href", url).text(attribute.DisplayValue);
					if (iframed) $link.attr("target", "_top");
					html = $link;
				} else if (attributeMetadata.Format == 4) { // Ticker Symbol
					var symbol = attribute.DisplayValue;
					if (symbol && symbol != '') {
						var tickerUrl = "http://go.microsoft.com/fwlink?linkid=8506&Symbol=" + encodeURIComponent(symbol.toUpperCase());
						var $tickerLink = $("<a></a>");
						$tickerLink.attr("href", tickerUrl).text(attribute.DisplayValue);
						if (iframed) $tickerLink.attr("target", "_top");
						html = $tickerLink;
					}
				}
			} else {
				html = attribute.DisplayValue;
			}
		break;
			default:
				html = attribute.DisplayValue;
				break;
		}

		if (buildDetailsLink) {
			var hrefDetails = "#";
			if (!layout.Configuration.DetailsActionLink.EntityForm && layout.Configuration.DetailsActionLink.URL != null) {
				hrefDetails = URI(getUrlFromActionLink(layout.Configuration.DetailsActionLink)).addSearch(layout.Configuration.DetailsActionLink.QueryStringIdParameterName, record.Id.toString());
			}
			var $detailsLink = $("<a></a>").attr("href", hrefDetails).addClass("details-link").addClass("has-tooltip").attr("data-toggle", "tooltip").attr("title", layout.Configuration.DetailsActionLink.Tooltip).html(html);
			if (hrefDetails != "#" && iframed) $detailsLink.attr("target", "_top");
			if (layout.Configuration.DetailsActionLink.EntityForm) {
				$detailsLink.addClass("launch-modal").attr("data-entityformid", layout.Configuration.DetailsActionLink.EntityForm.Id);
			}
			html = $detailsLink;
		}

		return html;
	}

	entityGrid.prototype.addSortEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var $table = $element.children(".view-grid").find("table");

		$table.find("th.sort-enabled a").on("click", function (e) {
			e.preventDefault();
			var $header = $(this).closest("th");
			$(this).find(".fa-arrow-down").remove();
			$(this).find(".fa-arrow-up").remove();
			$(this).find(".sort-hint").remove();
			var name = $header.data("sort-name");
			var dir = $header.data("sort-dir");
			if (typeof name === typeof undefined || name === false || name === null || name === '') {
				return;
			}
			if (typeof dir === typeof undefined || dir === false) {
				dir = "";
			}
			var sortExpression;
			if (dir == "ASC") {
				sortExpression = name + " DESC";
				$header.data("sort-dir", "DESC").removeClass("sort-asc").addClass("sort-desc");
				$header.attr("aria-sort", "descending");
				$(this).append(" ").append($("<span></span>").addClass("fa").addClass("fa-arrow-down")).append("<span class='sr-only sort-hint'>. Activate to sort in ascending order</span>");;
			} else {
				sortExpression = name + " ASC";
				$header.data("sort-dir", "ASC").removeClass("sort-desc").addClass("sort-asc");
				$header.attr("aria-sort", "ascending");
				$(this).append(" ").append($("<span></span>").addClass("fa").addClass("fa-arrow-up")).append("<span class='sr-only sort-hint'>. Activate to sort in descending order</span>");;
			}
			$header.addClass("sort");
			$table.data("sort-expression", sortExpression);
			$table.find("th.sort a").each(function () {
				var $column = $(this).closest("th");
				var columnName = $column.data("sort-name");
				if (typeof columnName === typeof undefined || columnName === false || columnName === null || columnName === '') {
					return;
				}
				if (columnName != name) {
					$(this).find(".fa-arrow-down").remove();
					$(this).find(".fa-arrow-up").remove();
					$column.removeClass("sort-asc").removeClass("sort-desc").removeClass("sort").data("sort-dir", "").removeAttr("aria-sort");
				}
			});
			$this.load(1);
		});
	}

	entityGrid.prototype.addDetailsActionLinkClickEventHandlers = function() {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions) {
			return;
		}
		$table.find(".details-link.launch-modal").on("click", function (e) {
			e.preventDefault();
			var link = $(this);
			var $tr = $(this).closest("tr");
			var id = $tr.data("id");
			var $modal = $element.children(".modal-form-details");
			var $iframe = $modal.find("iframe");
			var page = $iframe.data("page");
			var entityformid = $(this).data("entityformid");
			if (!entityformid) return;
			var src = page + "?id=" + id + "&entityformid=" + entityformid + "&languagecode=" + layout.Configuration.LanguageCode;
			$iframe.attr("src", src);
			$iframe.on("load", function () {
				$modal.find(".form-loading").hide();
				$modal.find("iframe").contents().find("#EntityFormControl").show();
			});
			$modal.find(".form-loading").show();
			$modal.find("iframe").contents().find("#EntityFormControl").hide();
			$modal.modal();
			$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
				link.closest(".action").children("a").trigger("focus");
			});
		});
	}

	entityGrid.prototype.addEditActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions) {
			return;
		}
		$table.find(".edit-link.launch-modal").on("click", function (e) {
			e.preventDefault();
			var link = $(this);
			var $tr = $(this).closest("tr");
			var id = $tr.data("id");
			var $modal = $element.children(".modal-form-edit");
			var $iframe = $modal.find("iframe");
			var page = $iframe.data("page");
			var entityformid = $(this).data("entityformid");
			if (!entityformid) return;
			var src = page + "?id=" + id + "&entityformid=" + entityformid + "&languagecode=" + layout.Configuration.LanguageCode;
			$iframe.attr("src", src);
			$iframe.on("load", function () {
				$modal.find(".form-loading").hide();
				$modal.find("iframe").contents().find("#EntityFormControl").show();
			});
			$modal.find(".form-loading").show();
			$modal.find("iframe").contents().find("#EntityFormControl").hide();
			$modal.modal();
			$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
				link.closest(".action").children("a").trigger("focus");
			});
		});
	}

	entityGrid.prototype.addDeleteActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.DeleteActionLink.Enabled && !layout.Configuration.DeleteActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.DeleteActionLink);
		$table.find(".delete-link").on("click", function (e) {
			e.preventDefault();
			var link = $(this);
			var $tr = $(this).closest("tr");
			var id = $tr.data("id");
			var $modal = $element.children(".modal-delete");
			var $button = $modal.find("button.primary");
			$button.off("click");
			$button.on("click", function (ev) {
				ev.preventDefault();
				$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				var data = JSON.stringify(entityReference);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: data
				}).done(function () {
					var rowCount = $element.children(".view-grid").find("tbody tr").length;
					if (rowCount == 1) {
						// we are deleting the last item of the page so we need to decrement the page number
						var $pagination = $element.children(".view-pagination");
						var page = $pagination.data("current-page");
						if (page > 1) {
							page--;
							$pagination.data("current-page", page);
						}
					}
					$modal.modal("hide");
					displaySuccessAlert(layout.Configuration.DeleteActionLink.SuccessMessage, $element, true);
					onComplete($element, layout.Configuration.DeleteActionLink);
				}).fail(function (jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					$modal.modal("hide");
					displayErrorAlert(error, $element);
				}).always(function () {
					$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
				});
			});
			$modal.modal();
			$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
				link.closest(".action").children("a").trigger("focus");
			});
		});
	}

	entityGrid.prototype.addQualifyLeadActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.QualifyLeadActionLink.Enabled && !layout.Configuration.QualifyLeadActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.QualifyLeadActionLink);

		if (layout.Configuration.QualifyLeadActionLink.ShowModal == 1) {
			$table.find(".qualify-lead-link").on("click", function(e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var $modal = $element.children(".modal-qualify");
				var $button = $modal.find("button.primary");
				$button.off("click");
				$button.on("click", function(ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var data = {};
					data.createAccount = true;
					data.createContact = true;
					data.createOpportunity = true;
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					data.entityReference = entityReference;
					var json = JSON.stringify(data);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: json
					}).done(function() {
						$modal.modal("hide");
						displaySuccessAlert(layout.Configuration.QualifyLeadActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.QualifyLeadActionLink);
					}).fail(function(jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function() {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".qualify-lead-link").on("click", function (e) {
				e.preventDefault();
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var data = {};
				data.createAccount = true;
				data.createContact = true;
				data.createOpportunity = true;
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				data.entityReference = entityReference;
				var json = JSON.stringify(data);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: json
				}).done(function () {
					displaySuccessAlert(layout.Configuration.QualifyLeadActionLink.SuccessMessage, $element, true);
					onComplete($element, layout.Configuration.QualifyLeadActionLink);
				}).fail(function (jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					displayErrorAlert(error, $element);
				}).always(function () { });
			});
		}
	}

	entityGrid.prototype.addCloseCaseActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.CloseIncidentActionLink.Enabled && !layout.Configuration.CloseIncidentActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.CloseIncidentActionLink);

		if (layout.Configuration.CloseIncidentActionLink.ShowModal == 1) {
			$table.find(".close-case-link").on("click", function(e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var $modal = $element.children(".modal-closecase");
				var $button = $modal.find("button.primary");
				$button.off("click");
				$button.on("click", function(ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var data = {};
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					data.entityReference = entityReference;
					var resolution = layout.Configuration.CloseIncidentActionLink.DefaultResolution;
					var description = layout.Configuration.CloseIncidentActionLink.DefaultResolutionDescription;
					data.resolutionSubject = resolution;
					data.resolutionDescription = description;
					var json = JSON.stringify(data);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: json
					}).done(function() {
						$modal.modal("hide");
						displaySuccessAlert(layout.Configuration.CloseIncidentActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.CloseIncidentActionLink);
					}).fail(function(jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function() {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".close-case-link").on("click", function (e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var data = {};
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				data.entityReference = entityReference;
				var resolution = layout.Configuration.CloseIncidentActionLink.DefaultResolution;
				var description = layout.Configuration.CloseIncidentActionLink.DefaultResolutionDescription;
				data.resolutionSubject = resolution;
				data.resolutionDescription = description;
				var json = JSON.stringify(data);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: json
				}).done(function () {
					displaySuccessAlert(layout.Configuration.CloseIncidentActionLink.SuccessMessage, $element, true);
					onComplete($element, layout.Configuration.CloseIncidentActionLink);
				}).fail(function (jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					displayErrorAlert(error, $element);
				}).always(function () { });
			});
		}
	}

	entityGrid.prototype.addConvertQuoteActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.ConvertQuoteToOrderActionLink.Enabled && !layout.Configuration.ConvertQuoteToOrderActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.ConvertQuoteToOrderActionLink);

		if (layout.Configuration.ConvertQuoteToOrderActionLink.ShowModal == 1) {
			$table.find(".convert-quote-link").on("click", function(e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var $modal = $element.children(".modal-convert-quote");
				var $button = $modal.find("button.primary");
				$button.off("click");
				$button.on("click", function(ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					var data = JSON.stringify(entityReference);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: data
					}).done(function() {
						$modal.modal("hide");
						displaySuccessAlert(layout.Configuration.ConvertQuoteToOrderActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.ConvertQuoteToOrderActionLink);
					}).fail(function(jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function() {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".convert-quote-link").on("click", function (e) {
				e.preventDefault();
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				var data = JSON.stringify(entityReference);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: data
				}).done(function () {
					displaySuccessAlert(layout.Configuration.ConvertQuoteToOrderActionLink.SuccessMessage, $element, true);
					onComplete($element, layout.Configuration.ConvertQuoteToOrderActionLink);
				}).fail(function (jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					displayErrorAlert(error, $element);
				}).always(function () { });
			});
		}
	}

	entityGrid.prototype.addConvertOrderActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.ConvertOrderToInvoiceActionLink.Enabled && !layout.Configuration.ConvertOrderToInvoiceActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.ConvertOrderToInvoiceActionLink);

		if (layout.Configuration.ConvertOrderToInvoiceActionLink.ShowModal == 1) {
			$table.find(".convert-order-link").on("click", function(e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var $modal = $element.children(".modal-convert-order");
				var $button = $modal.find("button.primary");
				$button.off("click");
				$button.on("click", function(ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					var data = JSON.stringify(entityReference);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: data
					}).done(function() {
						$modal.modal("hide");
						displaySuccessAlert(layout.Configuration.ConvertOrderToInvoiceActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.ConvertOrderToInvoiceActionLink);
					}).fail(function(jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function() {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".convert-order-link").on("click", function (e) {
				e.preventDefault();
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					var data = JSON.stringify(entityReference);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: data
					}).done(function () {
						displaySuccessAlert(layout.Configuration.ConvertOrderToInvoiceActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.ConvertOrderToInvoiceActionLink);
					}).fail(function (jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						displayErrorAlert(error, $element);
					}).always(function () { });
			});
		}
	}

	entityGrid.prototype.addCalculateOpportunityActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.CalculateOpportunityActionLink.Enabled && !layout.Configuration.CalculateOpportunityActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.CalculateOpportunityActionLink);

		if (layout.Configuration.CalculateOpportunityActionLink.ShowModal == 1) {
			$table.find(".calculate-opportunity-link").on("click", function(e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var $modal = $element.children(".modal-calculate");
				var $button = $modal.find("button.primary");
				$button.off("click");
				$button.on("click", function(ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					var data = JSON.stringify(entityReference);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: data
					}).done(function() {
						$modal.modal("hide");
						displaySuccessAlert(layout.Configuration.CalculateOpportunityActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.CalculateOpportunityActionLink);
					}).fail(function(jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function() {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".calculate-opportunity-link").on("click", function (e) {
				e.preventDefault();
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				var data = JSON.stringify(entityReference);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: data
				}).done(function () {
					displaySuccessAlert(layout.Configuration.CalculateOpportunityActionLink.SuccessMessage, $element, true);
					onComplete($element, layout.Configuration.CalculateOpportunityActionLink);
				}).fail(function (jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					displayErrorAlert(error, $element);
				}).always(function () { });
			});
		}
	}

	entityGrid.prototype.addResolveCaseActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.ResolveCaseActionLink.Enabled && !layout.Configuration.ResolveCaseActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.ResolveCaseActionLink);
		var $modal = $element.children(".modal-resolvecase");
		$table.find(".resolve-case-link").on("click", function (e) {
			e.preventDefault();
			var link = $(this);
			var $tr = $(this).closest("tr");
			var id = $tr.data("id");
			var $button = $modal.find("button.primary");
			$button.off("click");
			$button.on("click", function (ev) {
				ev.preventDefault();
				$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
				var data = {};
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				data.entityReference = entityReference;
				var $resolution = $modal.find(".resolution-input");
				data.resolutionSubject = $resolution.val();
				var $resolutionDescription = $modal.find(".resolution-description-input");
				data.resolutionDescription = $resolutionDescription.val();
				var json = JSON.stringify(data);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: json
				}).done(function () {
					$modal.modal("hide");
					displaySuccessAlert(layout.Configuration.ResolveCaseActionLink.SuccessMessage, $element, true);
					onComplete($element, layout.Configuration.ResolveCaseActionLink);
				}).fail(function (jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					$modal.modal("hide");
					displayErrorAlert(error, $element);
				}).always(function () {
					$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
				});
			});
			$modal.modal();
			$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
				$modal.find(".resolution-input").val('');
				$modal.find(".resolution-description-input").val('');
				link.closest(".action").children("a").trigger("focus");
			});
		});
	}

	entityGrid.prototype.addReopenCaseActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.ReopenCaseActionLink.Enabled && !layout.Configuration.ReopenCaseActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.ReopenCaseActionLink);

		if (layout.Configuration.ReopenCaseActionLink.ShowModal == 1) {
			$table.find(".reopen-case-link").on("click", function(e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var $modal = $element.children(".modal-reopencase");
				var $button = $modal.find("button.primary");
				$button.off("click");
				$button.on("click", function(ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					var data = JSON.stringify(entityReference);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: data
					}).done(function() {
						$modal.modal("hide");
						displaySuccessAlert(layout.Configuration.ReopenCaseActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.ReopenCaseActionLink);
					}).fail(function(jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function() {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".reopen-case-link").on("click", function (e) {
				e.preventDefault();
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				var data = JSON.stringify(entityReference);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: data
				}).done(function () {
					displaySuccessAlert(layout.Configuration.ReopenCaseActionLink.SuccessMessage, $element, true);
					onComplete($element, layout.Configuration.ReopenCaseActionLink);
				}).fail(function (jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					displayErrorAlert(error, $element);
				}).always(function () { });
			});
		}
	}

	entityGrid.prototype.addCancelCaseActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.CancelCaseActionLink.Enabled && !layout.Configuration.CancelCaseActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.CancelCaseActionLink);

		if (layout.Configuration.CancelCaseActionLink.ShowModal == 1) {
			$table.find(".cancel-case-link").on("click", function(e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var $modal = $element.children(".modal-cancelcase");
				var $button = $modal.find("button.primary");
				$button.off("click");
				$button.on("click", function(ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					var data = JSON.stringify(entityReference);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: data
					}).done(function() {
						$modal.modal("hide");
						displaySuccessAlert(layout.Configuration.CancelCaseActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.CancelCaseActionLink);
					}).fail(function(jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function() {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".cancel-case-link").on("click", function (e) {
				e.preventDefault();
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				var data = JSON.stringify(entityReference);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: data
				}).done(function () {
					displaySuccessAlert(layout.Configuration.CancelCaseActionLink.SuccessMessage, $element, true);
					onComplete($element, layout.Configuration.CancelCaseActionLink);
				}).fail(function (jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					displayErrorAlert(error, $element);
				}).always(function () { });
			});
		}
	}

	entityGrid.prototype.addActivateActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.ActivateActionLink.Enabled && !layout.Configuration.ActivateActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.ActivateActionLink);

		if (layout.Configuration.ActivateActionLink.ShowModal == 1) {
			$table.find(".activate-link").on("click", function(e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var $modal = $element.children(".modal-activate");
				var $button = $modal.find("button.primary");
				$button.off("click");
				$button.on("click", function(ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					var data = JSON.stringify(entityReference);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: data
					}).done(function() {
						$modal.modal("hide");
						displaySuccessAlert(layout.Configuration.ActivateActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.ActivateActionLink);
					}).fail(function(jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function() {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".activate-link").on("click", function (e) {
				e.preventDefault();
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				var data = JSON.stringify(entityReference);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: data
				}).done(function () {
					displaySuccessAlert(layout.Configuration.ActivateActionLink.SuccessMessage, $element, true);
					onComplete($element, layout.Configuration.ActivateActionLink);
				}).fail(function (jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					displayErrorAlert(error, $element);
				});
			});
		}
	}

	entityGrid.prototype.addDeactivateActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.DeactivateActionLink.Enabled && !layout.Configuration.DeactivateActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.DeactivateActionLink);

		if (layout.Configuration.DeactivateActionLink.ShowModal == 1) {
			$table.find(".deactivate-link").on("click", function(e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var $modal = $element.children(".modal-deactivate");
				var $button = $modal.find("button.primary");
				$button.off("click");
				$button.on("click", function(ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					var data = JSON.stringify(entityReference);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: data
					}).done(function() {
						$modal.modal("hide");
						displaySuccessAlert(layout.Configuration.DeactivateActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.DeactivateActionLink);
					}).fail(function(jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function() {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".deactivate-link").on("click", function (e) {
				e.preventDefault();
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					var data = JSON.stringify(entityReference);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: data
					}).done(function () {
						displaySuccessAlert(layout.Configuration.DeactivateActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.DeactivateActionLink);
					}).fail(function (jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						displayErrorAlert(error, $element);
					});
			});
		}
	}

	entityGrid.prototype.addActivateQuoteActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.ActivateQuoteActionLink.Enabled && !layout.Configuration.ActivateQuoteActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.ActivateQuoteActionLink);

		if (layout.Configuration.ActivateQuoteActionLink.ShowModal == 1) {
			$table.find(".activate-quote-link").on("click", function(e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var $modal = $element.children(".modal-activate-quote");
				var $button = $modal.find("button.primary");
				$button.off("click");
				$button.on("click", function(ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					var data = JSON.stringify(entityReference);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: data
					}).done(function() {
						$modal.modal("hide");
						displaySuccessAlert(layout.Configuration.ActivateQuoteActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.ActivateQuoteActionLin);
					}).fail(function(jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function() {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".activate-quote-link").on("click", function (e) {
				e.preventDefault();
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				var data = JSON.stringify(entityReference);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: data
				}).done(function () {
					displaySuccessAlert(layout.Configuration.ActivateQuoteActionLink.SuccessMessage, $element, true);
					onComplete($element, layout.Configuration.ActivateQuoteActionLin);
				}).fail(function (jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					displayErrorAlert(error, $element);
				});
			});
		}
	}

	entityGrid.prototype.addSetOpportunityOnHoldActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.SetOpportunityOnHoldActionLink.Enabled && !layout.Configuration.SetOpportunityOnHoldActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.SetOpportunityOnHoldActionLink);

		if (layout.Configuration.SetOpportunityOnHoldActionLink.ShowModal == 1) {
			$table.find(".set-opportunity-on-hold-link").on("click", function(e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var $modal = $element.children(".modal-set-opportunity-on-hold");
				var $button = $modal.find("button.primary");
				$button.off("click");
				$button.on("click", function(ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					var data = JSON.stringify(entityReference);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: data
					}).done(function() {
						$modal.modal("hide");
						displaySuccessAlert(layout.Configuration.SetOpportunityOnHoldActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.SetOpportunityOnHoldActionLink);
					}).fail(function(jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function() {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".set-opportunity-on-hold-link").on("click", function (e) {
				e.preventDefault();
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				var data = JSON.stringify(entityReference);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: data
				}).done(function () {
					displaySuccessAlert(layout.Configuration.SetOpportunityOnHoldActionLink.SuccessMessage, $element, true);
					onComplete($element, layout.Configuration.SetOpportunityOnHoldActionLink);
				}).fail(function (jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					displayErrorAlert(error, $element);
				});
			});
		}
	}

	entityGrid.prototype.addReopenOpportunityActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.ReopenOpportunityActionLink.Enabled && !layout.Configuration.ReopenOpportunityActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.ReopenOpportunityActionLink);

		if (layout.Configuration.ReopenOpportunityActionLink.ShowModal == 1) {
			$table.find(".reopen-opportunity-link").on("click", function (e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var $modal = $element.children(".modal-reopen-opportunity");
				var $button = $modal.find("button.primary");
				$button.off("click");
				$button.on("click", function (ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					var data = JSON.stringify(entityReference);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: data
					}).done(function () {
						$modal.modal("hide");
						displaySuccessAlert(layout.Configuration.ReopenOpportunityActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.ReopenOpportunityActionLink);
					}).fail(function (jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function () {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".reopen-opportunity-link").on("click", function (e) {
				e.preventDefault();
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				var data = JSON.stringify(entityReference);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: data
				}).done(function () {
					displaySuccessAlert(layout.Configuration.ReopenOpportunityActionLink.SuccessMessage, $element, true);
					onComplete($element, layout.Configuration.ReopenOpportunityActionLink);
				}).fail(function (jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					displayErrorAlert(error, $element);
				});
			});
		}
	}

	entityGrid.prototype.addWinOpportunityActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.WinOpportunityActionLink.Enabled && !layout.Configuration.WinOpportunityActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.WinOpportunityActionLink);

		if (layout.Configuration.WinOpportunityActionLink.ShowModal == 1) {
			$table.find(".win-opportunity-link").on("click", function(e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var $modal = $element.children(".modal-win-opportunity");
				var $button = $modal.find("button.primary");
				$button.off("click");
				$button.on("click", function(ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					var data = JSON.stringify(entityReference);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: data
					}).done(function() {
						$modal.modal("hide");
						displaySuccessAlert(layout.Configuration.WinOpportunityActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.WinOpportunityActionLink);
					}).fail(function(jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function() {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".win-opportunity-link").on("click", function (e) {
				e.preventDefault();
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				var data = JSON.stringify(entityReference);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: data
				}).done(function () {
					displaySuccessAlert(layout.Configuration.WinOpportunityActionLink.SuccessMessage, $element, true);
					onComplete($element, layout.Configuration.WinOpportunityActionLink);
				}).fail(function (jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					displayErrorAlert(error, $element);
				});
			});
		}
	}

	entityGrid.prototype.addLoseOpportunityActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.LoseOpportunityActionLink.Enabled && !layout.Configuration.LoseOpportunityActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.LoseOpportunityActionLink);

		if (layout.Configuration.LoseOpportunityActionLink.ShowModal == 1) {
			$table.find(".lose-opportunity-link").on("click", function(e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var $modal = $element.children(".modal-lose-opportunity");
				var $button = $modal.find("button.primary");
				$button.off("click");
				$button.on("click", function(ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					var data = JSON.stringify(entityReference);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: data
					}).done(function() {
						$modal.modal("hide");
						displaySuccessAlert(layout.Configuration.LoseOpportunityActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.LoseOpportunityActionLink);
					}).fail(function(jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function() {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".lose-opportunity-link").on("click", function (e) {
				e.preventDefault();
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				var data = JSON.stringify(entityReference);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: data
				}).done(function () {
					displaySuccessAlert(layout.Configuration.LoseOpportunityActionLink.SuccessMessage, $element, true);
					onComplete($element, layout.Configuration.LoseOpportunityActionLink);
				}).fail(function (jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					displayErrorAlert(error, $element);
				});
			});
		}
	}

	entityGrid.prototype.addGenerateQuoteFromOpportunityActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.GenerateQuoteFromOpportunityActionLink.Enabled && !layout.Configuration.GenerateQuoteFromOpportunityActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.GenerateQuoteFromOpportunityActionLink);

		if (layout.Configuration.GenerateQuoteFromOpportunityActionLink.ShowModal == 1) {
			$table.find(".generate-quote-from-opportunity-link").on("click", function(e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var $modal = $element.children(".modal-generate-quote-from-opportunity");
				var $button = $modal.find("button.primary");
				$button.off("click");
				$button.on("click", function(ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					var data = JSON.stringify(entityReference);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: data
					}).done(function () {
						$modal.modal("hide");
						displaySuccessAlert(layout.Configuration.GenerateQuoteFromOpportunityActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.GenerateQuoteFromOpportunityActionLink);
					}).fail(function(jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function() {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function () {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".generate-quote-from-opportunity-link").on("click", function (e) {
				e.preventDefault();
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				var data = JSON.stringify(entityReference);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: data
				}).done(function () {
					displaySuccessAlert(layout.Configuration.GenerateQuoteFromOpportunityActionLink.SuccessMessage, $element, true);
					onComplete($element, layout.Configuration.GenerateQuoteFromOpportunityActionLink);
				}).fail(function (jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					displayErrorAlert(error, $element);
				});
			});
		}
	}

	entityGrid.prototype.addDisassociateActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.DisassociateActionLink.Enabled || !layout.Configuration.DisassociateActionLink.URL) {
			return;
		}
		var url = getUrlFromActionLink(layout.Configuration.DisassociateActionLink);
		if (layout.Configuration.DisassociateActionLink.ShowModal == 1) {
			$table.find(".disassociate-link").on("click", function (e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var $modal = $element.children(".modal-disassociate");
				var $button = $modal.find("button.primary");
				$button.off("click");
				$button.on("click", function(ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var refEntityName = $element.data("ref-entity");
					var refEntityId = $element.data("ref-id");
					var refRelationship = $element.data("ref-rel");
					var refRelationshipRole = $element.data("ref-rel-role");
					if (!refEntityName || !refEntityId || !refRelationship) {
						return;
					}
					var target = { LogicalName: refEntityName, Id: refEntityId };
					var relationship = { SchemaName: refRelationship };
					if (refRelationshipRole) {
						relationship.PrimaryEntityRole = refRelationshipRole;
					}
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					var relatedEntities = [];
					relatedEntities[0] = entityReference;
					var disassociateRequest = {};
					disassociateRequest.target = target;
					disassociateRequest.RelatedEntities = relatedEntities;
					disassociateRequest.Relationship = relationship;
					var data = JSON.stringify(disassociateRequest);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: data
					}).done(function () {
						$modal.modal("hide");
						displaySuccessAlert(layout.Configuration.DisassociateActionLink.SuccessMessage, $element, true);
						onComplete($element, layout.Configuration.DisassociateActionLink);
					}).fail(function (jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function() {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function() {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".disassociate-link").on("click", function(e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var refEntityName = $element.data("ref-entity");
				var refEntityId = $element.data("ref-id");
				var refRelationship = $element.data("ref-rel");
				var refRelationshipRole = $element.data("ref-rel-role");
				if (!refEntityName || !refEntityId || !refRelationship) {
					return;
				}
				var target = { LogicalName: refEntityName, Id: refEntityId };
				var relationship = { SchemaName: refRelationship };
				if (refRelationshipRole) {
					relationship.PrimaryEntityRole = refRelationshipRole;
				}
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				var relatedEntities = [];
				relatedEntities[0] = entityReference;
				var disassociateRequest = {};
				disassociateRequest.target = target;
				disassociateRequest.RelatedEntities = relatedEntities;
				disassociateRequest.Relationship = relationship;
				var data = JSON.stringify(disassociateRequest);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: data
				}).done(function() {
					displaySuccessAlert(layout.Configuration.DisassociateActionLink.SuccessMessage, $element, true);
					onComplete($element, layout.Configuration.DisassociateActionLink);
				}).fail(function(jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					displayErrorAlert(error, $element);
				});
			});
		}
	}

	entityGrid.prototype.addWorkflowActionLinkClickEventHandlers = function () {
		var $this = this;
		var $element = $this._element;
		var layouts = $this._layouts;
		var layout = layouts[$this._activeLayoutIndex];
		var enableActions = $this._enableActions;
		var $table = $element.children(".view-grid").find("table");
		if (!enableActions || !layout.Configuration.ItemActionLinks || layout.Configuration.ItemActionLinks.length == 0) {
			return;
		}
		var actionLink = null;
		for (var i = 0; i < layout.Configuration.ItemActionLinks.length; i++) {
			var item = layout.Configuration.ItemActionLinks[i];
			if (item != null && typeof item.Type !== typeof undefined && item.Type == 7 && typeof item.Workflow !== typeof undefined && item.Workflow != null) {
				actionLink = item;
				break;
			}
		}
		if (actionLink == null) return;
		if (actionLink.ShowModal == 1) {
			$table.find(".workflow-link").on("click", function (e) {
				e.preventDefault();
				var link = $(this);
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var workflowid = $(this).data("workflowid");
				var $modal = $element.children(".modal-run-workflow");
				var $button = $modal.find("button.primary");
				var url = $(this).data("url");
				if (!workflowid || !url) {
					return;
				}
				$button.off("click");
				$button.on("click", function(ev) {
					ev.preventDefault();
					$(this).attr("disabled", "disabled").prepend("<span class='fa fa-spinner fa-spin' aria-hidden='true'></span>");
					var workflowActionLink = null;
					for (var j = 0; j < layout.Configuration.ItemActionLinks.length; j++) {
						var action = layout.Configuration.ItemActionLinks[j];
						if (action != null && typeof action.Type !== typeof undefined && action.Type == 7 && typeof action.Workflow !== typeof undefined && action.Workflow != null && action.Workflow.Id == workflowid) {
							workflowActionLink = action;
							break;
						}
					}
					var data = {};
					var workflowReference = {};
					workflowReference.LogicalName = "workflow";
					workflowReference.Id = workflowid;
					data.workflow = workflowReference;
					var entityReference = {};
					entityReference.LogicalName = layout.Configuration.EntityName;
					entityReference.Id = id;
					data.entity = entityReference;
					var json = JSON.stringify(data);
					$.ajax({
						type: "POST",
						contentType: "application/json",
						url: url,
						data: json
					}).done(function() {
						$modal.modal("hide");
						displaySuccessAlert(workflowActionLink != null ? workflowActionLink.SuccessMessage : null, $element, true);
						onComplete($element, workflowActionLink);
					}).fail(function(jqXhr) {
						var contentType = jqXhr.getResponseHeader("content-type");
						var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
						$modal.modal("hide");
						displayErrorAlert(error, $element);
					}).always(function() {
						$button.removeAttr("disabled", "disabled").find(".fa-spin").remove();
					});
				});
				$modal.modal();
				$modal.off("hidden.bs.modal.entitygrid").on("hidden.bs.modal.entitygrid", function() {
					link.closest(".action").children("a").trigger("focus");
				});
			});
		} else {
			$table.find(".workflow-link").on("click", function(e) {
				e.preventDefault();
				var $tr = $(this).closest("tr");
				var id = $tr.data("id");
				var workflowid = $(this).data("workflowid");
				var url = $(this).data("url");
				if (!workflowid || !url) {
					return;
				}
				var workflowActionLink = null;
				for (var j = 0; j < layout.Configuration.ItemActionLinks.length; j++) {
					var action = layout.Configuration.ItemActionLinks[j];
					if (action != null && typeof action.Type !== typeof undefined && action.Type == 7 && typeof action.Workflow !== typeof undefined && action.Workflow != null && action.Workflow.Id == workflowid) {
						workflowActionLink = action;
						break;
					}
				}
				var data = {};
				var workflowReference = {};
				workflowReference.LogicalName = "workflow";
				workflowReference.Id = workflowid;
				data.workflow = workflowReference;
				var entityReference = {};
				entityReference.LogicalName = layout.Configuration.EntityName;
				entityReference.Id = id;
				data.entity = entityReference;
				var json = JSON.stringify(data);
				$.ajax({
					type: "POST",
					contentType: "application/json",
					url: url,
					data: json
				}).done(function() {
					displaySuccessAlert(workflowActionLink != null ? workflowActionLink.SuccessMessage : null, $element, true);
					onComplete($element, workflowActionLink);
				}).fail(function(jqXhr) {
					var contentType = jqXhr.getResponseHeader("content-type");
					var error = contentType.indexOf("json") > -1 ? $.parseJSON(jqXhr.responseText) : { Message: jqXhr.status, InnerError: { Message: jqXhr.statusText } };
					displayErrorAlert(error, $element);
				});
			});
		}
	}

	entityGrid.prototype.initializePagination = function (data, currentTarget) {
		// requires ~/js/jquery.bootstrap-pagination.js

		var $this = this;
		var $element = $this._element;
		var $pagination = $element.children(".view-pagination");

		if (typeof data === typeof undefined || data === false || data == null) {
			$pagination.hide();
			return;
		}

		if ((typeof data.PageSize === typeof undefined || data.PageSize === false || data.PageSize == null) ||
		(typeof data.PageCount === typeof undefined || data.PageCount === false || data.PageCount == null) ||
		(typeof data.PageNumber === typeof undefined || data.PageNumber === false || data.PageNumber == null) ||
		(typeof data.ItemCount === typeof undefined || data.ItemCount === false || data.ItemCount == null)) {
			$pagination.hide();
			return;
		}
		
		if (data.PageCount <= 1) {
			$pagination.hide();
			return;
		}

		$pagination
			.data("pagesize", data.PageSize)
			.data("pages", data.PageCount)
			.data("current-page", data.PageNumber)
			.data("count", data.ItemCount)
			.off("click")
			.pagination({
				total_pages: $pagination.data("pages"),
				current_page: $pagination.data("current-page"),
				callback: function (event, pg) {
					event.preventDefault();
					var $li = $(event.target).closest("li");
					if ($li.not(".disabled").length > 0 && $li.not(".active").length > 0) {
						var filter = $this.getCurrentFilter();
						$this.load(pg, filter, !$this.isRelatedRecordFilterEnabled(), event.currentTarget);
					}
				}
			})
			.show();

		if (currentTarget) {
			$pagination.find("a").filter(function() {
				return $(this).text() == $(currentTarget).text();
			}).trigger("focus");
		}
	}

	entityGrid.prototype.hidePagination = function () {
		this._element.children(".view-pagination").hide();
	}

	function displayErrorModal(error, $modal) {
		$modal.prop("aria-hidden", false);

		if (typeof error !== typeof undefined && error !== false && error != null) {
			console.log(error);

			var title = "Error";
			var $title = $modal.find(".modal-title");
			var $body = $modal.find(".modal-body");

			$title.data("title", $title.html());

			if (typeof error.Message !== typeof undefined && error.Message !== false && error.Message != null) {
				if (typeof error.Message === 'number') {
					title = error.Message + " Error";
				} else {
					title = error.Message;
				}
			}

			$title.addClass("text-danger").html("<span class='fa fa-exclamation-triangle' aria-hidden='true'></span> " + title);

			if (typeof error.InnerError !== typeof undefined && error.InnerError !== false && error.InnerError != null) {
				$body.html("<p>" + error.InnerError.Message + "</p>");
			}
		}

		$modal.modal();
	}

	function displayErrorAlert(error, $element) {
		if (typeof error !== typeof undefined && error !== false && error != null) {
			console.log(error);
			var message;
			if (typeof error.InnerError !== typeof undefined && error.InnerError !== false && error.InnerError != null) {
				message = error.InnerError.Message;
			} else {
				message = error.Message;
			}
			var $container = $(".notifications");
			if ($container.length == 0) {
				var $pageheader = $(".page-heading");
				if ($pageheader.length == 0) {
					$container = $("<div class='notifications'></div>").prependTo($("#content-container"));
				} else {
					$container = $("<div class='notifications'></div>").appendTo($pageheader);
				}
			}
			$container.find(".notification").slideUp().remove();
			var $alert = $("<div class='notification alert alert-danger error alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>&times;</span></button><span class='fa fa-exclamation-triangle' aria-hidden='true'></span> " + message + "</div>")
				.on('closed.bs.alert', function () {
					if ($container.find(".notification").length == 0) $container.hide();
				}).prependTo($container);
			$container.show();
			$('html, body').animate({
				scrollTop: ($alert.offset().top - 20)
			}, 200);
		}
	}

	function displaySuccessAlert(success, $element, autohide) {
		var $container = $(".notifications");
		if ($container.length == 0) {
			var $pageheader = $(".page-heading");
			if ($pageheader.length == 0) {
				$container = $("<div class='notifications'></div>").prependTo($("#content-container"));
			} else {
				$container = $("<div class='notifications'></div>").appendTo($pageheader);
			}
		}
		$container.find(".notification").slideUp().remove();
		if (typeof success !== typeof undefined && success !== false && success != null && success != '') {
			var $alert = $("<div class='notification alert alert-success success alert-dismissible' role='alert'><button type='button' class='close' data-dismiss='alert' aria-label='Close'><span aria-hidden='true'>&times;</span></button>" + success + "</div>")
				.on('closed.bs.alert', function () {
					if ($container.find(".notification").length == 0) $container.hide();
				}).prependTo($container);
			$container.show();
			$('html, body').animate({
				scrollTop: ($alert.offset().top - 20)
			}, 200);
			if (autohide) {
				setTimeout(function () {
					$alert.slideUp(100).remove();
					if ($container.find(".notification").length == 0) $container.hide();
				}, 5000);
			}
		}
	}

	function parseSortExpression(sortExpression) {
		var sorts = [];
		if (sortExpression == null) {
			return sorts;
		}
		var sortExpressionPattern = /(\w+)\s*(asc|ascending|desc|descending)?\s*(,)?/gi;
		var matches = sortExpression.match(sortExpressionPattern);
		if (matches == null) {
			return sorts;
		}
		for (var i = 0; i < matches.length; i++) {
			var match = matches[i];
			if (match != null && match != "") {
				var name;
				var direction = "ASC";
				var pos = match.indexOf(" ");
				if (pos != -1) {
					name = match.substr(0, pos);
					direction = match.substr(pos + 1, match.length - pos);
				} else {
					name = match;
				}
				if (name != null) {
					name = name.replace(/,\s*$/, "");
				}
				if (direction != null) {
					direction = direction.replace(/,\s*$/, "");
				}
				var sort = { name: name, direction: direction };
				sorts.push(sort);
			}
		}
		return sorts;
	}

	function indexOf(list, attr, val) {
		// Determines the index of an object in an array by matching the value with the object property value.
		// Returns: -1 if object does not exist that matches the property value, otherwise returns integer index of the object in the array.
		var result = -1;
		$.each(list, function (index, item) {
			if (item[attr].toString().toLowerCase().indexOf(val.toString().toLowerCase()) != -1) {
				result = index;
			}
		});
		return result;
	}

	if (!String.prototype.startsWith) {
		String.prototype.startsWith = function(searchString, position) {
			position = position || 0;
			return this.lastIndexOf(searchString, position) === position;
		};
	}

	function getUrlFromActionLink(action) {
		if (!action || !action.URL) return null;
		return getUrlFromUrlBuilder(action.URL);
	}

	function getRedirectUrlFromActionLink(action) {
		if (!action || !action.RedirectUrl) return null;
		return getUrlFromUrlBuilder(action.RedirectUrl);
	}

	function getUrlFromUrlBuilder(urlBuilder) {
		if (!urlBuilder) return null;
		if (urlBuilder.Host == window.location.hostname) {
			return URI(urlBuilder.PathWithQueryString);
		} else {
			return URI(urlBuilder.Uri);
		}
	}

	function onComplete($gridElement, action) {

		if (typeof action == typeof undefined || action == null) {
			$gridElement.trigger("refresh");
			return;
		}

		var onCompleteRefresh = 0;
		var onCompleteRedirectToWebPage = 1;
		var onCompleteRedirectToUrl = 2;
		var onCompleteRedirect = typeof action.OnComplete != typeof undefined && action.OnComplete != null && action.OnComplete != onCompleteRefresh;

		if (onCompleteRedirect) {
			var redirectUrl = getRedirectUrl(action);

			if (redirectUrl != null && redirectUrl != '') {
				redirect(redirectUrl);
			} else {
				$gridElement.trigger("refresh");
			}
		} else {
			$gridElement.trigger("refresh");
		}
	}

	function redirect(redirectUrl) {
		if (typeof redirectUrl == 'undefined' || redirectUrl == null || redirectUrl == '') return;
		$.blockUI({
			css: {
				border: 'none',
				backgroundColor: 'transparent',
				opacity: .5,
				color: '#fff'
			},
			message: "<span class='fa fa-2x fa-spinner fa-pulse' aria-hidden='true'></span>"
		});
		if (parent) {
			parent.location.replace(redirectUrl);
		} else {
			window.location.replace(redirectUrl);
		}
	}

	function getRedirectUrl(action) {
		if (!action || !action.RedirectUrl) return null;
		if (action.RedirectUrl.Host == window.location.hostname) {
			return URI(action.RedirectUrl.PathWithQueryString);
		} else {
			return URI(action.RedirectUrl.Uri);
		}
	}

	function inIframe() {
		try {
			return window.self !== window.top;
		} catch (e) {
			return true;
		}
	}
}(jQuery));