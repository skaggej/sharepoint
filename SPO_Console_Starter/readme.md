#SPO_Console_Starter Usage

This console app has everything you need to get started.  It was built with references to the SharePoint Online assemblies (v16) via the NuGet package at https://www.nuget.org/packages/Microsoft.SharePointOnline.CSOM.  Make sure to install that if you have issues.

If you change nothing and simply run it, you'll be prompted for the authentication info for your SPO environment and, once authenticated, the console app will simply output "Hello, SPO!"
<pre><code>
private static void ExecuteCustomCode() 
{
    Console.WriteLine("Hello, SPO!"); 
}
</code></pre>

# UpdateWebLogos
This method will set the site logo of each site to match the site logo of the top-level site in the site collection.  Before running this, ensure that the site logo of the top-level site has been set to the desired logo.
<pre><code>
private static void UpdateWebLogos()
{
    Web rootWeb = site.RootWeb;
    siteLogoUrl = rootWeb.SiteLogoUrl;
    RecursivelyUpdateWebLogo(rootWeb);
}

private static void RecursivelyUpdateWebLogo(Web currentWeb)
{
    Console.WriteLine("Changing " + currentWeb.Title + " site logo URL from " + currentWeb.SiteLogoUrl + " to " + siteLogoUrl + ".");
    Console.WriteLine();
    currentWeb.SiteLogoUrl = siteLogoUrl;
    currentWeb.Update();
    WebCollection subWebs = currentWeb.Webs;
    clientContext.Load(subWebs);
    clientContext.ExecuteQuery();
    foreach (Web subWeb in subWebs)
    {
        RecursivelyUpdateWebLogo(subWeb);
    }
}
</code></pre>
