using System;
using System.Net;
using System.Security;
using Microsoft.SharePoint.Client;

namespace SPO_Console_Starter
{
    class Program
    {
        #region private static variables
        
        private static ClientContext clientContext;
        private static Site site;
        private static string siteRelativeUrl;
        private static SecureString password;
        private static string username;
        private static string url;
        
        #endregion

        static void Main(string[] args)
        {
            ConnectToSite();
            ExecuteCustomCode();
            Console.Write("Press any key to continue...");
            Console.Read();
        }

        private static void ExecuteCustomCode()
        {
            Console.WriteLine("Hello, SPO!");
            Console.WriteLine();

            //UpdateSiteLogos();
        }

        /// <summary>
        /// This method will set the site logo of each site to match the site logo of the top-level site in the site collection.
        /// Before running this, ensure that the site logo of the top-level site has been set to the desired logo.
        /// </summary>
        private static void UpdateSiteLogos()
        {
            string siteLogoUrl = "";
            Web rootWeb = site.RootWeb;
            siteLogoUrl = rootWeb.SiteLogoUrl;
            WebCollection subWebs = rootWeb.Webs;
            clientContext.Load(subWebs);
            clientContext.ExecuteQuery();
            foreach (Web subWeb in subWebs)
            {
                Console.WriteLine("Changing " + subWeb.Title + " site logo URL from " + subWeb.SiteLogoUrl + " to " + siteLogoUrl + ".");
                Console.WriteLine();
                subWeb.SiteLogoUrl = siteLogoUrl;
                subWeb.Update();
                clientContext.ExecuteQuery();
            }
        }

        #region "helper functions"

        private static void ConnectToSite()
        {
            Console.WriteLine("Please enter the URL to the SharePoint Site");
            url = Console.ReadLine();

            Console.WriteLine("Please enter the username");
            username = Console.ReadLine();

            Console.WriteLine("Please enter the password");
            SecureString securepassword = getpassword();

            clientContext = new ClientContext(url);
            password = new SecureString();
            string charpassword = new NetworkCredential(string.Empty, securepassword).Password;
            foreach (char c in charpassword.ToCharArray()) password.AppendChar(c);
            clientContext.Credentials = new SharePointOnlineCredentials(username, password);
            site = clientContext.Site;

            clientContext.Load(site);
            clientContext.ExecuteQuery();

            siteRelativeUrl = site.ServerRelativeUrl;

            clientContext.Load(site.RootWeb);
            clientContext.ExecuteQuery();

            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Successfully connected to site at " + site.Url);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to continue..");
            Console.ReadLine();
        }

        public static SecureString getpassword()
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