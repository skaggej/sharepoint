using System;
using System.IO;
using System.Security;
using Microsoft.SharePoint.Client;

namespace SPO_ContentTypes
{
    class Program
    {
        #region constants
        #endregion

        #region variables

        private static ClientContext clientContext;
        private static Site site;
        private static Web rootWeb;
        private static SecureString password;
        private static string userName;
        private static string url;
        private static string outputFilePath;

        #endregion

        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                DisplayUsage();
                return;
            }
            else
            {
                outputFilePath = args[0];
                ConnectToSite();
                ShowContentTypesInEachList();
                Console.WriteLine("Done! Press enter to exit...");
                Console.ReadLine();
            }
        }

        private static void ShowContentTypesInEachList()
        {
            StreamWriter outputStreamWrite = new StreamWriter(outputFilePath, true);
            ListCollection lists = rootWeb.Lists;
            clientContext.Load(lists);
            clientContext.ExecuteQuery();
            foreach(List currentList in lists)
            {
                Console.WriteLine("Content Types in List: " + currentList.Title);
                outputStreamWrite.WriteLine("Content Types in List: " + currentList.Title);
                ContentTypeCollection listContentTypes = currentList.ContentTypes;
                clientContext.Load(listContentTypes);
                clientContext.ExecuteQuery();
                Console.ForegroundColor = ConsoleColor.Green;
                foreach(ContentType currentContentType in listContentTypes)
                {
                    Console.WriteLine(currentContentType.Name);
                    outputStreamWrite.WriteLine(currentContentType.Name);
                }
                Console.ResetColor();
                Console.WriteLine();
                outputStreamWrite.WriteLine();
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

        static void DisplayUsage()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Please specify the name of an output path/file");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Example: SPO-ContentTypes.exe output.txt");
            Console.ResetColor();
        }

        #endregion

    }
}
