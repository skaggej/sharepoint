// Register namespace
window.mAdcOW = window.mAdcOW || {};
 
// Override function to allow the use of Srch.ContentBySearch.getPictureMarkup
// as it's pretty nice :)
window.mAdcOW.overrideSet = false;
window.mAdcOW.OverrideValueInfo = function () {
    if (window.mAdcOW.overrideSet) return;
    window.mAdcOW.overrideSet = true;
    window.mAdcOW.origValueInfoConstructor = Srch.ValueInfo.prototype.constructor;
    Srch.ValueInfo.prototype.constructor = function (originalInputValue, managedProperty) {
        try {
            window.mAdcOW.origValueInfoConstructor(originalInputValue, managedProperty);
        }
        catch (e) { }
    }
}
 
// Function to render each item row
window.mAdcOW.ItemRenderer = {
    itemHtml: function (ctx) {
        var imageValue = new Srch.ValueInfo(ctx.CurrentItem.URL, "ImageUrl");
        var titleValue = new Srch.ValueInfo(ctx.CurrentItem.Title, "Title");
 
        var pictureMarkup = Srch.ContentBySearch.getPictureMarkup(imageValue, 100, 100, ctx.CurrentItem, "cbs-picture3LinesImg", titleValue, '');
 
        var linkURL = '';
        var html = '<li><div class="cbs-picture3LinesContainer" data-displaytemplate="ItemPicture4Lines">\
                        <div style="display:none;" class="cbs-picture3LinesImageContainer">\
                            <a class="cbs-pictureImgLink" href="' + linkURL + '" title="' + $htmlEncode(ctx.CurrentItem.Title) + '">' + pictureMarkup + '</a>\
                        </div>\
                        <div class="cbs-picture3LinesDataContainer" style="margin-left:0;max-width:none;">\
                            <h2 class="cbs-picture3LinesLine1 ms-accentText2 ms-noWrap"> ' + ctx.CurrentItem.Title + '</h2>\
                            <div class="cbs-picture3LinesLine2 ms-noWrap"> ' + ctx.CurrentItem._x0062_gp8 + '</div>\
                            <div class="cbs-pictureLine3 ms-textSmall">' + STSHtmlDecode(ctx.CurrentItem.Body) + '</div>\
                        </div>\
                    </div></li>';
        return html;
    }
};

 
// anonymous self-executing function to hook up JS Link templates for the announcement list
(function () {
    var overrideCtx = {};
    overrideCtx.Templates = {};
    overrideCtx.OnPreRender = window.mAdcOW.OverrideValueInfo;
    // header template which includes a CSS reference to the search css which is used
    overrideCtx.Templates.Header = "<link rel=\"stylesheet\" type=\"text/css\" href=\"/_layouts/15/1033/styles/Themable/searchv15.css\" /><ul class=\"cbs-List\">";
    overrideCtx.Templates.Item = window.mAdcOW.ItemRenderer.itemHtml;
    overrideCtx.Templates.Footer = "</ul>";
    overrideCtx.BaseViewID = 1;
    overrideCtx.ListTemplateType = 104; //Announcement
 
    SPClientTemplates.TemplateManager.RegisterTemplateOverrides(overrideCtx);
})();