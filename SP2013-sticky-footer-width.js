function setFooterWidth() {
	//Retrieve the width from list view table
	var tableWidth = parseInt($("#WebPartWPQ2").width());
	if(tableWidth > parseInt($("body").width())) {
		//Retrieve the left margin for the quicklaunch
		var leftMargin = $("#contentBox").css("margin-left").replace("px", "");
		var newSize = tableWidth + parseInt(leftMargin);
		$("customFooter").width(newSize);
	}		
}
$(document).ready(function() {
	setFooterWidth();
});
