<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SampleAddInWeb.Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
      <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"
                EnablePartialRendering="true" />
        <asp:UpdatePanel ID="PopulateData" runat="server" UpdateMode="Conditional">
          <ContentTemplate>      
            <table border="1" cellpadding="10">
             <tr><th><asp:LinkButton ID="CSOM" runat="server" Text="Populate Data" 
                                   OnClick="CSOM_Click" /></th></tr>
             <tr><td>

            <h2>SharePoint Site</h2>
            <asp:Label runat="server" ID="WebTitleLabel"/>

            <h2>Current User:</h2>
            <asp:Label runat="server" ID="CurrentUserLabel" />

            <h2>Site Users</h2>
            <asp:ListView ID="UserList" runat="server">     
                <ItemTemplate >
                  <asp:Label ID="UserItem" runat="server" 
                                    Text="<%# Container.DataItem.ToString()  %>">
                  </asp:Label><br />
               </ItemTemplate>
            </asp:ListView>

            <h2>Site Lists</h2>
                   <asp:ListView ID="ListList" runat="server">
                       <ItemTemplate >
                         <asp:Label ID="ListItem" runat="server" 
                                    Text="<%# Container.DataItem.ToString()  %>">
                        </asp:Label><br />
                      </ItemTemplate>
                  </asp:ListView>
                </td>              
              </tr>
             </table>
           </ContentTemplate>
         </asp:UpdatePanel>
      </div>
    </form>
</body>
</html>
