<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

{% if poll %}
<div>
	{% unless poll.user_selected_option %}
	<div class="poll-questionpanel" data-id="{{ poll.id }}" data-name="{{ poll.name }}">
		<h5 class="poll-question">{{ poll.question }}</h5>
		<ul class="poll-options list-unstyled">
			{% for option in poll.options %}
			<li class="radio">
				<label for="poll_option_{{ forloop.index }}">
					<input type="radio" name="{{ poll.name }}" value="{{ option.id }}" id="poll_option_{{ forloop.index }}" />
					<span class="sr-only">{{ poll.question }}</span>
					{{ option.answer }}
				</label>
			</li>
			{% endfor %}
		</ul>
		<button type="button" class="btn btn-primary poll-submit">{% if poll.submit_button_label %}{{ poll.submit_button_label }}{% else %}Submit{% endif %}</button>
		<button type="button" class="btn btn-default poll-viewresults">{% assign vr = snippets["polls/resultslabel"] %}{% if vr %}{{ vr }}{% else %}View Results{% endif %}</button>
	</div>
	{% endunless %}
	<div class="poll-resultspanel">
		<h5 class="poll-question">{{ poll.question }}</h5>
		<div class="poll-results">
			{% for option in poll.options %}
			<div class="poll-result">
				<span class="poll-option">{{ option.answer }}</span>
				<div class="progress">
					<div class="bar progress-bar" style="min-width:2em;width:{{ option.percentage }}%">{{ option.percentage | max_decimals: 0 }}%</div>
				</div>
			</div>
			{% endfor %}
		</div>
		<p>{% assign tl = snippets["polls/totalslabel"] %}{% if tl %}{{ tl }}{% else %}Total Votes:{% endif %} {{ poll.votes }}</p>
		{% unless poll.user_selected_option %}
		<button type="button" class="btn btn-default poll-return">{% assign rl = snippets["polls/returnlabel"] %}{% if rl %}{{ rl }}{% else %}Return to Poll{% endif %}</button>
		{% endunless %}
	</div>
</div>
{% endif %}