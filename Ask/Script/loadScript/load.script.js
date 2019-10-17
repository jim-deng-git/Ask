if (typeof jQuery === 'undefined') {
  throw new Error('loadScript\'s JavaScript requires jQuery')
}
+function ($) {
  'use strict';
  var version = $.fn.jquery.split(' ')[0].split('.')
  if ((version[0] < 2 && version[1] < 9) || (version[0] == 1 && version[1] == 9 && version[2] < 1) || (version[0] > 3)) {
    throw new Error('loadScript\'s JavaScript requires jQuery version 1.9.1 or higher')
  }
}(jQuery);


+function ($) {
	'use strict';
	
	sessionStorage.clear();
	$.extend({
		
		loadScript: function(files) {

			$.map(files, function(file) {

			    if (!sessionStorage.getItem(file)) {
			    	
					var script = document.createElement('script'); script.type = 'text/javascript'; script.async = true;
				    script.src = file;
					sessionStorage.setItem(file, true);
			    	var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(script, s);
				}
			});
		}, 
		loadStrong: function() {
			console.log(sessionStorage);
		},
		clearStrong: function() {
			sessionStorage.clear();
		}
	});

}(jQuery);
