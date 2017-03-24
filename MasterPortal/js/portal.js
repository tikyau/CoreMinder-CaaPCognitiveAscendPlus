var portal = portal || {};

portal.convertAbbrDateTimesToTimeAgo = function($) {
  $("abbr.timeago").each(function() {
    var dateTime = Date.parse($(this).text());
    if (dateTime) {
      $(this).attr("title", dateTime.toString("yyyy-MM-ddTHH:mm:ss"));
      $(this).text(dateTime.toString("MMMM dd, yyyy h:mm tt"));
    }
  });

  $("abbr.timeago").timeago();
  
  $("abbr.posttime").each(function () {
    var dateTime = Date.parse($(this).text());
    if (dateTime) {
      $(this).attr("title", dateTime.toString("MMMM dd, yyyy h:mm tt"));
      $(this).text(dateTime.toString("MMMM dd, yyyy h:mm tt"));
    }
  });
};

portal.initializeHtmlEditors = function () {
  $(document).on('focusin', function (e) {
    if ($(e.target).closest(".mce-window").length) {
      e.stopImmediatePropagation();
    }
  });

  tinymce.init({
    selector: '.html-editors textarea',
    content_css: '/css/bootstrap.min.css,/css/tinymce.css',
    plugins: [
      "advlist autolink lists link image nonbreaking charmap print preview hr anchor code",
      "searchreplace visualblocks visualchars fullscreen",
      "insertdatetime media table contextmenu paste directionality"
    ],
    toolbar: "styleselect | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image media",
    toolbar_items_size: 'small',
    browser_spellcheck: true,
    convert_urls: false,
    height: 240,
    resize: 'both',
    end_container_on_empty_block: true,
    style_formats: [
      {
        title: 'Headings',
        items: [
          { title: 'Heading 1', format: 'h1' },
          { title: 'Heading 2', format: 'h2' },
          { title: 'Heading 3', format: 'h3' },
          { title: 'Heading 4', format: 'h4' },
          { title: 'Heading 5', format: 'h5' },
          { title: 'Heading 6', format: 'h6' }
        ]
      },
      {
        title: 'Inline',
        items: [
          { title: 'Bold', icon: 'bold', format: 'bold' },
          { title: 'Italic', icon: 'italic', format: 'italic' },
          { title: 'Underline', icon: 'underline', format: 'underline' },
          { title: 'Strikethrough', icon: 'strikethrough', format: 'strikethrough' },
          { title: 'Superscript', icon: 'superscript', format: 'superscript' },
          { title: 'Subscript', icon: 'subscript', format: 'subscript' },
          { title: 'Code', icon: 'code', format: 'code' }
        ]
      },
      {
        title: 'Blocks',
        items: [
          { title: 'Paragraph', format: 'p' },
          { title: 'Blockquote', format: 'blockquote' },
          { title: 'Div', format: 'div' },
          { title: 'Pre', format: 'pre' },
          { title: 'Code Block', format: 'codeblock' }
        ]
      },
      {
        title: 'Alignment',
        items: [
          { title: 'Left', icon: 'alignleft', format: 'alignleft' },
          { title: 'Center', icon: 'aligncenter', format: 'aligncenter' },
          { title: 'Right', icon: 'alignright', format: 'alignright' },
          { title: 'Justify', icon: 'alignjustify', format: 'alignjustify' }
        ]
      }
    ],
    formats: {
      codeblock: { block: 'pre', classes: 'prettyprint linenums' },
      underline: { inline: 'u', exact: true }
    },
    inline_styles: false,
    setup: function(editor) {
      editor.on('change', function() {
        editor.save();
      });
    }
  });
};

(function ($, XRM) {
  portal.initializeHtmlEditors();
  
  $(function () {
    portal.convertAbbrDateTimesToTimeAgo($);
    
    var facebookSignin = $(".facebook-signin");
    facebookSignin.on("click", function (e) {
      e.preventDefault();
      window.open(facebookSignin.attr("href"), "facebook_auth", "menubar=1,resizable=1,scrollbars=yes,width=800,height=600");
    });

    // Map dropdowns with .btn-select class to backing field.
    $('.btn-select').each(function() {
      var select = $(this),
        target   = $(select.data('target')),
        selected = $('option:selected', target),
        focus    = $(select.data('focus')),
        label    = $('.btn .selected', select);
      
      if (selected.length > 0) {
        label.text(selected.text());
        $('.dropdown-menu li > a[data-value="' + selected.val() + '"]', select).parent('li').addClass('active');
      }

      target.change(function () {
        var changedSelected = $('option:selected', target);
        select.find('.dropdown-menu li.active').removeClass('active');
        label.text(changedSelected.text());
        $('.dropdown-menu li > a[data-value="' + changedSelected.val() + '"]', select).parent('li').addClass('active');
      });
      
      $('.dropdown-menu li > a', select).click(function () {
        var option = $(this),
            value  = option.data('value');

        $('.dropdown-menu li', select).removeClass("active");
        option.parent('li').addClass("active");
        target.val(value);
        label.text(option.text());
        focus.focus();
      });
    });
    
    // Convert GMT timestamps to client time.
    $('abbr.timestamp').each(function() {
      var element = $(this);
      var text = element.text();
      var dateTime = Date.parse(text);
      if (dateTime) {
      	element.attr("title", text);
	      var dateFormat = dateFormatConverter.convert(element.closest("[data-dateformat]").data("dateformat") || "MMMM d, yyyy", dateFormatConverter.dotNet, dateFormatConverter.momentJs);
	      var timeFormat = dateFormatConverter.convert(element.closest("[data-timeformat]").data("timeformat") || "h:mm tt", dateFormatConverter.dotNet, dateFormatConverter.momentJs);
	      var datetimeFormat = dateFormatConverter.convert(element.attr('data-format'), dateFormatConverter.dotNet, dateFormatConverter.momentJs) || (dateFormat + ' ' + timeFormat);
	      element.text(moment(dateTime).format(datetimeFormat));
      }
    });

    // Format time elements.
	  var dateFormat = dateFormatConverter.convert($(".crmEntityFormView").closest("[data-dateformat]").data("dateformat") || "MM/dd/yyyy", dateFormatConverter.dotNet, dateFormatConverter.momentJs);
	  var timeFormat = dateFormatConverter.convert($(".crmEntityFormView").closest("[data-timeformat]").data("timeformat") || "h:mm tt", dateFormatConverter.dotNet, dateFormatConverter.momentJs);
	  var datetimeFormat = dateFormat + ' ' + timeFormat;

    $("time").each(function () {
      if (!moment) {
        return;
      }
      var datetime = $(this).attr("datetime");
      if ($(this).hasClass("date-only")) {
      $(this).text(moment(datetime).format(dateFormat));

      } else {
        $(this).text(moment(datetime).format(datetimeFormat));
      }
    });
    
    // Convert GMT date ranges to client time.
    $('.vevent').each(function() {
      var start = $('.dtstart', this);
      var end = $('.dtend', this);
      var startText = start.text();
      var endText = end.text();
      var startDate = Date.parse(startText);
      var endDate = Date.parse(endText);
      
      if (startDate) {
      	start.attr('title', startText);
      	var dateFormat = dateFormatConverter.convert(start.closest("[data-dateformat]").data("dateformat") || "MMMM d, yyyy", dateFormatConverter.dotNet, dateFormatConverter.momentJs);
      	var timeFormat = dateFormatConverter.convert(start.closest("[data-timeformat]").data("timeformat") || "h:mm tt", dateFormatConverter.dotNet, dateFormatConverter.momentJs);
      	var datetimeFormat = dateFormatConverter.convert(start.attr('data-format'), dateFormatConverter.dotNet, dateFormatConverter.momentJs) || (dateFormat + ' ' + timeFormat);
      	
      	start.text(moment(startDate).format(datetimeFormat));
      }
      
      if (endDate) {
        end.attr('title', endText);
        var sameDay = startDate
          && startDate.getYear() == endDate.getYear()
          && startDate.getMonth() == endDate.getMonth()
          && startDate.getDate() == endDate.getDate();

        var dateFormat = dateFormatConverter.convert(end.closest("[data-dateformat]").data("dateformat") || "MMMM d, yyyy", dateFormatConverter.dotNet, dateFormatConverter.momentJs);
        var timeFormat = dateFormatConverter.convert(end.closest("[data-timeformat]").data("timeformat") || "h:mm tt", dateFormatConverter.dotNet, dateFormatConverter.momentJs);

        var datetimeFormat = dateFormatConverter.convert(end.attr('data-format'), dateFormatConverter.dotNet, dateFormatConverter.momentJs) || (dateFormat + ' ' + timeFormat);

        end.text(moment(endDate).format(sameDay ? timeFormat : datetimeFormat));
      }
    });

    // Initialize Bootstrap Carousel for any elements with the .carousel class.
    $('.carousel').carousel();

    // Workaround for jQuery UI and Bootstrap tooltip name conflict
    if ($.ui && $.ui.tooltip) {
      $.widget.bridge('uitooltip', $.ui.tooltip);
    }

    $('.has-tooltip').tooltip();

    prettyPrint();

    // Initialize any shopping cart status displays.
    (function () {
      var shoppingCartStatuses = {};

      $('.shopping-cart-status').each(function () {
        var element = $(this),
          service = element.attr('data-href'),
          count = element.find('.count'),
          countValue = count.find('.value'),
          serviceQueue;

        if (!service) {
          return;
        }

        serviceQueue = shoppingCartStatuses[service];

        if (!$.isArray(serviceQueue)) {
          serviceQueue = shoppingCartStatuses[service] = [];
        }

        serviceQueue.push(function (data) {
          if (data != null && data.Count && data.Count > 0) {
            countValue.text(data.Count);
            count.addClass('visible');
            element.addClass('visible');
          }
        });
      });

      $.each(shoppingCartStatuses, function (service, queue) {
        $.getJSON(service, function (data) {
          $.each(queue, function (index, fn) {
            fn(data);
          });
        });
      });
    })();

    $('[data-state="sitemap"]').each(function () {
      var $nav = $(this),
        current = $nav.data('sitemap-current'),
        ancestor = $nav.data('sitemap-ancestor'),
        state = $nav.closest('[data-sitemap-state]').data('sitemap-state'),
        statePath,
        stateRootKey;

      if (!(state && (current || ancestor))) {
        return;
      }

      statePath = state.split(':');
      stateRootKey = statePath[statePath.length - 1];

      $nav.find('[data-sitemap-node]').each(function () {
        var $node = $(this),
          key = $node.data('sitemap-node');

        if (!key) {
          return;
        }

        $.each(statePath, function (stateIndex, stateKey) {
          if (stateIndex === 0) {
            if (current && stateKey == key) {
              $node.addClass(current);
            }
          } else {
            if (ancestor && stateKey == key && key != stateRootKey) {
              $node.addClass(ancestor);
            }
          }
        });
      });
    });

    (function () {
      var query = URI ? URI(document.location.href).search(true) || {} : {};

      $('[data-query]').each(function () {
        var $this = $(this),
          value = query[$this.data('query')];

        if (typeof value === 'undefined') {
          return;
        }

        $this.val(value).change();
      });
    })();
  });

  if (typeof XRM != 'undefined' && XRM) {
    XRM.zindex = 2000;

    var tinymceConfigurations = [XRM.tinymceSettings, XRM.tinymceCompactSettings];

    for (var i = 0; i < tinymceConfigurations.length; i++) {
      var configuration = tinymceConfigurations[i];

      if (!configuration) continue;

      // Load all page stylesheets into TinyMCE, for as close to WYSIWYG as possible.
      var stylesheets = $('head > link[rel="stylesheet"]').map(function (_, e) {
        var href = $(e).attr('href');
        return href.match(/,/) ? null : href;
      }).get();

      stylesheets.push('/css/tinymce.css');
      
      configuration.content_css = stylesheets.join(',');

      configuration.formats = $.extend(configuration.formats || {}, {
        page_header: { block: 'div', classes: 'page-header', wrapper: true },
        alert_info: { block: 'div', classes: 'alert alert-info', wrapper: true },
        alert_success: { block: 'div', classes: 'alert alert-success', wrapper: true },
        alert_warning: { block: 'div', classes: 'alert alert-warning', wrapper: true },
        alert_danger: { block: 'div', classes: 'alert alert-danger', wrapper: true },
        label_default: { inline: 'span', classes: 'label label-default' },
        label_info: { inline: 'span', classes: 'label label-info' },
        label_success: { inline: 'span', classes: 'label label-success' },
        label_warning: { inline: 'span', classes: 'label label-warning' },
        label_danger: { inline: 'span', classes: 'label label-danger' },
        well: { block: 'div', classes: 'well', wrapper: true },
        well_sm: { block: 'div', classes: 'well well-sm', wrapper: true },
        well_lg: { block: 'div', classes: 'well well-lg', wrapper: true }
      });

      configuration.style_formats.push(
      {
        title: 'Bootstrap',
        items: [
          {
            title: 'Page Header',
            format: 'page_header'
          },
          {
            title: 'Alerts',
            items: [
              { title: 'Alert Info', format: 'alert_info' },
              { title: 'Alert Success', format: 'alert_success' },
              { title: 'Alert Warning', format: 'alert_warning' },
              { title: 'Alert Danger', format: 'alert_danger' }
            ]
          },
          {
            title: 'Labels',
            items: [
              { title: 'Label', format: 'label_default' },
              { title: 'Label Info', format: 'label_info' },
              { title: 'Label Success', format: 'label_success' },
              { title: 'Label Warning', format: 'label_warning' },
              { title: 'Label Danger', format: 'label_danger' }
            ]
          },
          {
            title: 'Wells',
            items: [
              { title: 'Well', format: 'well' },
              { title: 'Well (Small)', format: 'well_sm' },
              { title: 'Well (Large)', format: 'well_lg' }
            ]
          }
        ]
      });
    }
  }

    var notification = $.cookie("adx-notification");
    if (typeof (notification) === typeof undefined || notification == null) return;
    displaySuccessAlert(notification, true);
    $.cookie("adx-notification", null);

    function displaySuccessAlert(success, autohide) {
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
            window.scrollTo(0, 0);
            if (autohide) {
                setTimeout(function() {
                    $alert.slideUp(100).remove();
                    if ($container.find(".notification").length == 0) $container.hide();
                }, 5000);
            }
        }
    }

})(window.jQuery, window.XRM);
