var SingleSOPlugin = {

	GetQueryStrings: function () 
	{ 
		var queries = {};
		var queryString = document.URL.split('?')[1];
		if ( queryString ){
			queryStrings = queryString.split('&');
			for ( var i = 0; i < queryStrings.length; i++ ){
				hash = queryStrings[i].split('=');
				if ( hash[1] ){
	                queries[hash[0]] = hash[1];
				} else {
	                queries[hash[0]] = true;
				}
			}
	    }
	    return queries;
	},

	InjectData__deps: ['GetQueryStrings'],
  	InjectData: function ()
	{
		var qs = _GetQueryStrings();
		var l = qs["u"]; 
		var p = qs["p"]; 
		console.log(qs);
		SendMessage ('LoginFormUI', 'ReceiveLogin', l.concat(':').concat(p));
	},

};

mergeInto(LibraryManager.library, SingleSOPlugin);