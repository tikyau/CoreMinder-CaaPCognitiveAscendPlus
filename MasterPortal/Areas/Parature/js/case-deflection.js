/*
# Parature Case Deflection widget

<input type="text" class="form-control parature-deflection"
  data-url="{{ parature.articles.search_url | escape }}"
  data-target="#articles"
  data-template="#article-results" />

<div id="articles"></div>

{% raw %}
<script id="article-results" type="text/x-handlebars-template">
  <div class="list-group">
    {{# each articles}}
      <a class="list-group-item" href="{{ url }}">{{ question }}</a>
    {{/each}}
  </div>
</script>
{% endraw %}

For usage on an Entity Form, create an Entity Form Metadata record for a single line of text attribute and set the 'CSS Class'field value to 'parature-deflection'.

*/

!function (handlebars) { var a = handlebars.template, n = handlebars.templates = handlebars.templates || {}; n.kbarticles = a(function (a, n, e, r, s) { function t(a, n) { var r, s = ""; return s += '\r\n<div class="panel panel-default">\r\n  <div class="panel-heading">\r\n    <div class="panel-title">Suggested Topics</div>\r\n  </div>\r\n  <div class="list-group">\r\n    ', r = e.each.call(a, a && a.articles, { hash: {}, inverse: d.noop, fn: d.program(2, l, n), data: n }), (r || 0 === r) && (s += r), s += '\r\n  </div>\r\n</div>\r\n<a href="/" class="btn btn-block btn-info">Found my answer</a>\r\n' } function l(a, n) { var r, s, t = ""; return t += '\r\n    <a class="list-group-item" href="', (s = e.url) ? r = s.call(a, { hash: {}, data: n }) : (s = a && a.url, r = typeof s === c ? s.call(a, { hash: {}, data: n }) : s), t += o(r) + '">', (s = e.question) ? r = s.call(a, { hash: {}, data: n }) : (s = a && a.question, r = typeof s === c ? s.call(a, { hash: {}, data: n }) : s), t += o(r) + "</a>\r\n    " } this.compilerInfo = [4, ">= 1.0.0"], e = this.merge(e, a.helpers), s = s || {}; var i, c = "function", o = this.escapeExpression, d = this; return i = e["if"].call(n, n && n.articles, { hash: {}, inverse: d.noop, fn: d.program(1, t, s), data: s }), i || 0 === i ? i : "" }) }(Handlebars);

(function ($, handlebars) {
  'use strict';

	$(document).on('keyup.adx.parature.case-deflection', '.parature-deflection', _.debounce(getResults, 500));

  function getResults() {
    var $this = $(this),
      url = $this.data('url'),
      target = $this.data('target'),
      template = $this.data('template'),
      value = $this.val(),
			compiledTemplate,
			$target;

		if (!(value)) {
			$this.parent().find(".articles").empty().hide();
			return;
		}

		if (!url) {
			url = $("body").data("parature-case-deflection-url");
			if (!url) {
				window.console.log("Parature case deflection URL not specified.");
      return;
    }
		}

		if (value.length < 4) return;

		if (!target) {
			$target = $("<div class='articles pull-left' style='margin-top: 20px; width: 100%;'></div>");
		} else {
			$target = $(target);
		}

		$target.html('<div style="text-align: center;"><span class="fa fa-spinner fa-spin" aria-hidden="true"></span></div>');

		if (!template) {
			compiledTemplate = handlebars.templates['kbarticles'];
		} else {
    compiledTemplate = handlebars.compile($(template).html());
		}

		$.getJSON(url, { keywords: value }, function(data) {
			$target.html(compiledTemplate(data));
			if (!target) {
				$this.parent().find(".articles").remove();
				$this.parent().append($target).show();
			}
    });
  }

}(jQuery, Handlebars));
