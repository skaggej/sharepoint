function setFooterWidth() {
	//Retrieve the width from list view table
	var tableWidth = parseInt($("#WebPartWPQ2").width());
	//Retrieve the left margin for the quicklaunch
	var leftMargin = $("#contentBox").css("margin-left").replace("px", "");
	if(parseInt(tableWidth)+parseInt(leftMargin) > parseInt($("body").width())) {
		var newSize = parseInt(tableWidth) + parseInt(leftMargin);
		$("#customFooter").width(newSize);
	}		
}
$(document).ready(function() {
	setFooterWidth();
});
