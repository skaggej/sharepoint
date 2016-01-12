(function () {
    var overrideCtx = {};
    overrideCtx.Templates = {};
    overrideCtx.OnPostRender = [
        HighlightRowOverride,
    ];
    var fieldJsLinkOverride = {};
    fieldJsLinkOverride.Templates = {}; 
    fieldJsLinkOverride.Templates.Fields =
    {
        // Make sure the field views get hooked up to their methods defined below
        'DueDate': { 'View': GetDueDateBackgroundColor },
        'Start_x0020_Draft_x0020_Date': { 'View': GetStartDraftDateBackgroundColor }
    }; 
    // Register the rendering templates
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCtx);
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(fieldJsLinkOverride);
})();

function HighlightRowOverride(inCTX) {
    for (var i = 0; i < inCTX.ListData.Row.length; i++) {
        var listItem = inCTX.ListData.Row[i];
        var iid = GenerateIIDForListItem(inCTX, listItem);
        var row = document.getElementById(iid);
        var itemStatus = listItem.Status;
        if (itemStatus == "Amendment in Process") {
                row.style.backgroundColor = "#FFFF00"; //yellow
        }
        else if (itemStatus == "Replacement in Process") {
                row.style.backgroundColor = "#00FF00"; //green
        }
    }
    inCTX.skipNextAnimation = true;
}

function GetDueDateBackgroundColor (ctx) {
     Date.prototype.addDays = function(days) {
        var calculatedDate = new Date(this.valueOf());
        calculatedDate.setDate(calculatedDate.getDate() + days);
        return calculatedDate;
    }
    var today = new Date();
    var dueDate = new Date(ctx.CurrentItem.DueDate);    
    if (dueDate.addDays(-14) <= today) {
            return "<div style='background-color: #FF0000;'>" + ctx.CurrentItem.DueDate + "</div>";
    } 
    return ctx.CurrentItem.DueDate;
}

function GetStartDraftDateBackgroundColor (ctx) {
    var today = new Date();
    var reminderDate = new Date(ctx.CurrentItem.Reminder_x0020_Date);    
    if (reminderDate < today) {
            return "<div style='background-color: #FFFFFF; font-weight: bold; color: #FF6600'>" + ctx.CurrentItem.Start_x0020_Draft_x0020_Date + "</div>";
    } 
    return ctx.CurrentItem.Start_x0020_Draft_x0020_Date;
}