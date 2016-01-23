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
        private static string siteFullUrl;
        private static string siteRelativeUrl;
        private static StreamWriter outputStream;
        private static string webPartPrefix;

        #endregion

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                DisplayUsage();
                return;
            }
            else
            {
                outputFilePath = args[0];
                webPartPrefix = args[1];
                outputStream = new StreamWriter(outputFilePath, true);
                ConnectToSite();
                UploadWebParts();
                LogOutput("Done! Press enter to exit...");
                outputStream.Close();
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
            LogOutput("Uploading Web Parts...");
            foreach (string webPartFile in webPartFiles)
            {
                string newWebPartFile = webPartFile;
                if (!webPartFile.Contains(webPartPrefix))
                {
                    newWebPartFile = webPartFile.Replace(@"..\..\SourceWebParts\", @"..\..\OutputWebParts\" + webPartPrefix + "-");
                }
                string webPartUrl = newWebPartFile.Replace(@"..\..\OutputWebParts\", "");
                using (FileStream fs = System.IO.File.OpenRead(webPartFile))
                {
                    //Read in full webPartFile
                    LogOutput("Reading from " + webPartFile);
                    StreamReader sr = new StreamReader(fs);
                    String outFile = sr.ReadToEnd();
                    sr.Close();
                    LogOutput("Writing to " + newWebPartFile);
                    //Replace the "~sitecollection/" token in file with current relative url and create a new file with the webPartPrefix
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
            //Set the Group property of each web part to "webPartPrefix Web Parts"
            List webPartGallery = clientContext.Web.GetListByTitle("Web Part Gallery");
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = "<View><Query><Where><Contains><FieldRef Name='FileLeafRef'/><Value Type='Text'>" + webPartPrefix + "</Value></Contains></Where></Query></View>";
            ListItemCollection myWebParts = webPartGallery.GetItems(camlQuery);
            clientContext.Load(myWebParts);
            clientContext.ExecuteQuery();
            LogOutput("Setting the 'Group' of each new web part to '" + webPartPrefix + " Web Parts'");
            foreach (ListItem myWebPart in myWebParts)
            {
                myWebPart["Group"] = webPartPrefix + " Web Parts";
                myWebPart.Update();
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
            siteFullUrl = site.Url;
            siteRelativeUrl = site.ServerRelativeUrl;
            //Ensure all URLs end with "/"
            siteFullUrl += "/";
            if (!siteRelativeUrl.EndsWith("/"))
            {
                siteRelativeUrl += "/";
            }
            rootWeb = site.RootWeb;
            clientContext.Load(rootWeb);
            clientContext.ExecuteQuery();
            Console.WriteLine("");
            Console.WriteLine("Connected to the Site Collection successfully...\n");
        }

        private static SecureString GetPassword()
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

        private static void DisplayUsage()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Please specify the name of an output path/file and web part prefix");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Example: SPO-WebPartUploader.exe output.txt MyWebPartPrefix");
            Console.ResetColor();
        }

        private static void LogOutput(string message)
        {
            Console.WriteLine(message);
            outputStream.WriteLine(message);
        }

        #endregion

    }
}
