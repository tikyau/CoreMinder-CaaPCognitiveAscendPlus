(function() {

	function bustFrame() {
		if (top != self) {
			top.location.replace(self.location.href);
		}
	}

	window.onload = bustFrame;

})();
