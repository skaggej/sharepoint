using System;
using System.Security;
using Microsoft.SharePoint.Client;

namespace SPO_DeactivateFeature
{
    class Program
    {
        #region constants

        private const string spFeatureWebMinimalDownloadStrategy = "87294c72-f260-42f3-a41b-981a2ffce37a";  // Web-scoped 'Minimal Download Strategy' feature
        private const string spFeatureWebMobileBrowserView = "d95c97f3-e528-4da2-ae9f-32b3535fbb59";  // Web-scoped 'Mobile Browser View' feature

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
            //Console.WriteLine("Deactivating 'Minimal Download Strategy'\n");
            //DeactivateFeature(rootWeb, spFeatureWebMinimalDownloadStrategy);
            Console.WriteLine("Deactivating 'Mobile Browser View' feature...\n");
            DeactivateFeatureWeb(rootWeb, new Guid(spFeatureWebMobileBrowserView));
            Console.WriteLine("Done! Press enter to exit...");
            Console.ReadLine();
        }        

        private static void DeactivateFeatureWeb(Web currentWeb, Guid featureGuid)
        {
            Feature currentFeature = currentWeb.Features.GetById(featureGuid);
            clientContext.Load(currentFeature, f =>f.DisplayName);
            clientContext.ExecuteQuery();
            if (currentWeb.IsFeatureActive(featureGuid))
            {
                Console.WriteLine("Deactivating web-scoped '" + currentFeature.DisplayName + "' feature for web at:\n" + currentWeb.Url + "\n");
                currentWeb.DeactivateFeature(featureGuid);
            }
            WebCollection webs = currentWeb.Webs;
            clientContext.Load(webs);
            clientContext.ExecuteQuery();
            foreach(Web web in webs)
            {
                DeactivateFeatureWeb(web, featureGuid);
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
