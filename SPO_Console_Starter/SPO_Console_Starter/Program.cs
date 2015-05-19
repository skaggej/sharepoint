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
        private static ClientContext subclientContext;
        private static Site mysite;
        private static string mySiteRelativeUrl;
        private static SecureString passWord;
        private static string userName;
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
        }

        #region "helper functions"

        private static void ConnectToSite()
        {
            Console.WriteLine("Please enter the URL to the SharePoint Site");
            url = Console.ReadLine();

            Console.WriteLine("Please enter the username");
            userName = Console.ReadLine();

            Console.WriteLine("Please enter the Password");
            SecureString securePassword = getPassword();

            clientContext = new ClientContext(url);
            passWord = new SecureString();
            string charpassword = new NetworkCredential(string.Empty, securePassword).Password;
            foreach (char c in charpassword.ToCharArray()) passWord.AppendChar(c);
            clientContext.Credentials = new SharePointOnlineCredentials(userName, passWord);
            mysite = clientContext.Site;

            clientContext.Load(mysite);
            clientContext.ExecuteQuery();

            mySiteRelativeUrl = mysite.ServerRelativeUrl;

            clientContext.Load(mysite.RootWeb);
            clientContext.ExecuteQuery();

            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Successfully connected to site at " + mysite.Url);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to continue..");
            Console.ReadLine();
        }

        public static SecureString getPassword()
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