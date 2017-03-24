<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
{% if placement %}
{% assign placement = polls.placements[placement.id] %}
{% if placement.polls.size > 0 %}
<div class="content-panel panel panel-default">
	<div class="panel-heading">
		{% assign sm = sitemarkers["Poll Archives"] %}
		{% if sm %}{% assign sal = snippets["polls/archiveslabel"] %}
		<a href="{{ sm.url }}" class="pull-right">{% if sal %}{{ sal }}{% elsif sm.adx_name %}{{ sm.adx_name }}{% else %}Poll Archives{% endif %}</a>
		{% endif %}
		<h4>
			<span class="fa fa-question-circle" aria-hidden="true"></span>
			{% assign st = snippets["polls/title"] %}
			{% if st %}{{ st }}{% else %}Poll{% endif %}
		</h4>
	</div>
	{% if random %}
	<div class="panel-body poll random" data-url="{{ placement.random_url }}" data-submit-url="{{ placement.submit_url }}"></div>
	{% else %}
	{% for poll in placement.polls %}
	{% unless forloop.index0 == 0 %}<hr style="margin:0"/>{% endunless %}
	<div class="panel-body poll" data-url="{{ poll.poll_url }}" data-submit-url="{{ poll.submit_url }}">
	</div>
	{% endfor %}
	{% endif %}
</div>
{% endif %}
{% endif %}