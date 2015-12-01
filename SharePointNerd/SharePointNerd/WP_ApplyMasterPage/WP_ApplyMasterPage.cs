using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace SharePointNerd.WebParts
{
    [ToolboxItemAttribute(false)]
    public class WP_ApplyMasterPage : WebPart
    {
        Label lResult;
        protected override void CreateChildControls()
        {
            Button bApplyMasterPage = new Button { Text = "Apply Master Page" };
            bApplyMasterPage.Click += new System.EventHandler(bApplyMasterPage_Click);
            lResult = new Label { Text = "The master page has not yet been applied." };
            this.Controls.Add(bApplyMasterPage);
            this.Controls.Add(new LiteralControl("&nbsp;&nbsp;"));
            this.Controls.Add(lResult);
        }

        void bApplyMasterPage_Click(object sender, System.EventArgs e)
        {
            SPWebCollection allWebs = SPContext.Current.Site.AllWebs;
            SPWeb rootWeb = SPContext.Current.Site.RootWeb;
            string masterUrl = rootWeb.MasterUrl;
            foreach (SPWeb web in allWebs)
            {
                try
                {
                    web.MasterUrl = masterUrl;
                    web.Update();
                    lResult.Text = "Applied!";
                }
                catch (Exception exception)
                {
                    SPList errorsList = rootWeb.Lists["Errors"];
                    SPListItem newError = errorsList.Items.Add();
                    newError["Message"] = exception.Message;
                    newError["Source"] = exception.Source;
                    newError["StackTrace"] = exception.StackTrace;
                    newError["InnerException"] = exception.InnerException;
                    newError.Update();
                    lResult.Text = "An error has occurred!  See the Errors list for more information.";
                    return;  //ensures the error only occurs once
                }
                finally
                {                    
                    web.Dispose();
                }
            }
        }
    }
}