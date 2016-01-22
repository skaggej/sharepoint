using System;
using System.IO;
using System.Security;
using Microsoft.SharePoint.Client;

namespace SPO_WebPartUploader
{
    class Program
    {
        #region constants

        private const string siteCollectionToken = "~sitecollection/";
        private const string spListNameWebPartGallery = "Web Part Gallery";

        #endregion

        #region variables

        private static ClientContext clientContext;
        private static Site site;
        private static Web rootWeb;
        private static SecureString password;
        private static string userName;
        private static string url;
        private static string outputFilePath;
        private static string siteRelativeUrl;

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
                UploadWebParts();
                Console.WriteLine("Done! Press enter to exit...");
                Console.ReadLine();
            }
        }

        private static void UploadWebParts()
        {
            //Obtain a reference to the Web Part Gallery
            var folder = clientContext.Web.GetListByTitle(spListNameWebPartGallery).RootFolder;
            clientContext.Load(folder);
            clientContext.ExecuteQuery();
            //Upload the web parts to the Web Part Gallery
            string[] webPartFiles = Directory.GetFiles(@"..\..\SourceWebParts");
            Console.WriteLine("Uploading Fuse Web Parts...");
            foreach (string webPartFile in webPartFiles)
            {
                string newWebPartFile = webPartFile;
                if (!webPartFile.Contains("Fuse"))
                {
                    newWebPartFile = webPartFile.Replace(@"..\..\SourceWebParts\", @"..\..\OutputWebParts\Fuse-");
                }
                string webPartUrl = newWebPartFile.Replace(@"..\..\OutputWebParts\", "");
                using (FileStream fs = System.IO.File.OpenRead(webPartFile))
                {
                    //Read in full webPartFile
                    Console.WriteLine("Reading from " + webPartFile);
                    StreamReader sr = new StreamReader(fs);
                    String outFile = sr.ReadToEnd();
                    sr.Close();
                    Console.WriteLine("Writing to " + newWebPartFile);
                    //Replace the "~sitecollection/" token in file with current relative url and create a new file with a prefix of "Fuse-"
                    outFile = outFile.Replace(siteCollectionToken, siteRelativeUrl);
                    FileStream fsWrite = System.IO.File.OpenWrite(newWebPartFile);
                    StreamWriter sw = new StreamWriter(fsWrite);
                    sw.Write(outFile);
                    sw.Close();
                    fsWrite.Close();
                }
                //Upload the new webpart file to the Web Part Gallery
                using (var stream = System.IO.File.OpenRead(newWebPartFile))
                {
                    FileCreationInformation fileInfo = new FileCreationInformation();
                    fileInfo.ContentStream = stream;
                    fileInfo.Overwrite = true;
                    fileInfo.Url = webPartUrl;
                    folder.Files.Add(fileInfo);
                    clientContext.ExecuteQuery();
                }
                //Delete the file
                System.IO.File.Delete(newWebPartFile);
            }
            //Set the Group property of each web part to "Fuse Web Parts"
            List webPartGallery = clientContext.Web.GetListByTitle("Web Part Gallery");
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = "<View><Query><Where><Contains><FieldRef Name='FileLeafRef'/><Value Type='Text'>Fuse</Value></Contains></Where></Query></View>";
            ListItemCollection fuseWebParts = webPartGallery.GetItems(camlQuery);
            clientContext.Load(fuseWebParts);
            clientContext.ExecuteQuery();
            Console.WriteLine("Setting the 'Group' of each new web part to 'Fuse Web Parts'");
            foreach (ListItem fuseWebPart in fuseWebParts)
            {
                fuseWebPart["Group"] = "Fuse Web Parts";
                fuseWebPart.Update();
                clientContext.ExecuteQuery();
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
            Console.WriteLine("Example: SPO-WebPartUploader.exe output.txt");
            Console.ResetColor();
        }

        #endregion

    }
}
