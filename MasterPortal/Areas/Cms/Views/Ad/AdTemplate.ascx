﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
{% if ad %}
<div>
	{% if ad.redirect_url %}<a class="ad-link" href="{{ ad.redirect_url }}">{% endif %}
	{% if ad.title %}
	<h5 class="ad-title">{{ ad.title }}</h5>
	{% endif %}
	{% if ad.image.url %}
	<img class="ad-link-image center-block" src="{{ ad.image.url }}" alt="{{ ad.image.alternate_text }}"{% if ad.image.height %} height="{{ ad.image.height }}"{% endif %}{% if ad.image.width %} width="{{ ad.image.width }}"{% endif %} style="{% if ad.image.height %}height:{{ ad.image.height }}px;{% endif %}{% if ad.image.width %}width:{{ ad.image.width }}px;{% endif %}">
	{% endif %}
	{% if ad.redirect_url %}</a>{% endif %}
	{% if show_copy %}{% if ad.copy %}
	<div class="ad-copy">{{ ad.copy }}</div>
	{% endif %}{% endif %}
</div>
{% endif %}