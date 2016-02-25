using System;
using System.Linq;
using System.Security;
using Microsoft.SharePoint.Client;

namespace SPO_MasterPages
{
    class Program
    {
        #region constants

        private const string sampleConst1 = "sample";

        #endregion

        #region variables

        private static ClientContext clientContext;
        private static Site site;
        private static Web rootWeb;
        private static SecureString password;
        private static string userName;
        private static string url;

        #endregion

        static void Main(string[] args)
        {
            ConnectToSite();
            GetMasterPageReferences();
            Console.WriteLine("Done! Press enter to exit...");
            Console.ReadLine();
        }

        static void GetMasterPageReferences()
        {
            Console.WriteLine("Getting the master page references for each site...\n");
            foreach(string webUrl in site.GetAllWebUrls())
            {                
                Console.WriteLine("Master pages for web at " + webUrl);
                ClientContext currentWebContext = new ClientContext(webUrl);
                currentWebContext.Credentials = new SharePointOnlineCredentials(userName, password);
                Web currentWeb = currentWebContext.Web;
                currentWebContext.Load(currentWeb, w=>w.AllProperties,w=>w.MasterUrl,w=>w.CustomMasterUrl);
                currentWebContext.ExecuteQuery();
                Console.WriteLine("Master URL: " + currentWeb.MasterUrl);
                Console.WriteLine("Custom Master URL: " + currentWeb.CustomMasterUrl);
                Console.WriteLine();
            }
        }

        #region helper methods

        private static void ConnectToSite()
        {
            Console.WriteLine("Please enter the URL to the SharePoint Site");
            url = Console.ReadLine();
            Console.WriteLine("Please enter the username");
            userName = Console.ReadLine();
            Console.WriteLine("Please enter the Password");
            SecureString securePassword = GetPassword();
            clientContext = new ClientContext(url);
            password = new SecureString();
            string charpassword = new System.Net.NetworkCredential(string.Empty, securePassword).Password;
            foreach (char c in charpassword.ToCharArray()) password.AppendChar(c);
            clientContext.Credentials = new SharePointOnlineCredentials(userName, password);
            site = clientContext.Site;
            clientContext.Load(site);
            clientContext.ExecuteQuery();
            rootWeb = site.RootWeb;
            clientContext.Load(rootWeb);
            clientContext.ExecuteQuery();
            Console.WriteLine("");
            Console.WriteLine("Connected to the Site Collection successfully...\n");
        }

        public static SecureString GetPassword()
        {
            SecureString pwd = new SecureString();
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.RemoveAt(pwd.Length - 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    pwd.AppendChar(i.KeyChar);
                    Console.Write("*");
                }
            }
            return pwd;
        }

        #endregion

    }
}
