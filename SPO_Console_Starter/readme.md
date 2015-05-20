#SPO_Console_Starter Usage

This console app has everything you need to get started.  It was built with references to the SharePoint Online assemblies (v16) via the NuGet package at https://www.nuget.org/packages/Microsoft.SharePointOnline.CSOM.  Make sure to install that if you have issues.

If you change nothing and simply run it, you'll be prompted for the authentication info for your SPO environment and, once authenticated, the console app will simply output "Hello, SPO!"

<pre><code>
private static void ExecuteCustomCode() 
{
    Console.WriteLine("Hello, SPO!"); 
}
</code></pre>
