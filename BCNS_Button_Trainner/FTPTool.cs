using BCNS;
using FluentFTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BCNS_Button_Trainner
{
    public class FTPTool
    {
        Connect conn = new Connect();
        private TreeViewItem GetTreeView(string text, string imagePath)
        {
            TreeViewItem item = new TreeViewItem();

            item.IsExpanded = false;

            // create stack panel
            StackPanel stack = new StackPanel();
            stack.Orientation = System.Windows.Controls.Orientation.Horizontal;

            // create Image
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = new BitmapImage
                (new Uri("pack://application:,,/image/" + imagePath));
            image.Width = 16;
            image.Height = 16;
            // Label
            System.Windows.Controls.Label lbl = new System.Windows.Controls.Label();
            lbl.Content = text;


            // Add into stack
            stack.Children.Add(image);
            stack.Children.Add(lbl);

            // assign stack to header
            item.Header = stack;
            return item;
        }
        public TreeNode CreateDirectoryNode(string path, string name, string Id, string Pw)
        {
            var directoryNode = new TreeNode(name);
            var directoryListing = GetDirectoryListing(path, Id, Pw);

            var directories = directoryListing.Where(d => d.IsDirectory);
            var files = directoryListing.Where(d => !d.IsDirectory);

            foreach (var dir in directories)
            {
                directoryNode.ChildNodes.Add(CreateDirectoryNode(dir.FullPath, dir.Name, Id , Pw));
                Console.WriteLine("dir = " + dir.Name);
            }
            foreach (var file in files)
            {
                directoryNode.ImageUrl = "pack://application:,,/image/document-icon.png";
                directoryNode.ChildNodes.Add(new TreeNode(file.Name));
                Console.WriteLine("file = " + file.Name);
            }
            return directoryNode;
        }
        public TreeViewItem CreateDirectoryItem(string path, string name, string Id, string Pw)
        {
            var rootNode = new TreeViewItem();
            var directoryNode = new TreeViewItem();
            var directoryListing = GetDirectoryListing(path, Id, Pw);
            rootNode = GetTreeView(name, "folder-icon.png"); ;
            var directories = directoryListing.Where(d => d.IsDirectory);
            var files = directoryListing.Where(d => !d.IsDirectory);

            foreach (var dir in directories)
            {
                rootNode.Items.Add(CreateDirectoryItem(dir.FullPath, dir.Name, Id, Pw));
                Console.WriteLine("dir = " + dir.Name);
            }
            foreach (var file in files)
            {
                directoryNode = GetTreeView(file.Name, "document-icon.png");
                rootNode.Items.Add(directoryNode);
                Console.WriteLine("file = " + file.Name);
            }
            return rootNode;
        }
        public IEnumerable<FTPListDetail> GetDirectoryListing(string rootUri, string Id, string Pw)
        {
            var CurrentRemoteDirectory = rootUri;
            var result = new StringBuilder();
            var request = GetWebRequest(WebRequestMethods.Ftp.ListDirectoryDetails, CurrentRemoteDirectory, Id, Pw);
            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        result.Append(line);
                        result.Append("\n");
                        line = reader.ReadLine();
                    }
                    if (string.IsNullOrEmpty(result.ToString()))
                    {
                        return new List<FTPListDetail>();
                    }
                    result.Remove(result.ToString().LastIndexOf("\n"), 1);
                    var results = result.ToString().Split('\n');
                    string regex =
                        @"^" +               //# Start of line
                        @"(?<dir>[\-ld])" +          //# File size          
                        @"(?<permission>[\-rwx]{9})" +            //# Whitespace          \n
                        @"\s+" +            //# Whitespace          \n
                        @"(?<filecode>\d+)" +
                        @"\s+" +            //# Whitespace          \n
                        @"(?<owner>\w+)" +
                        @"\s+" +            //# Whitespace          \n
                        @"(?<group>\w+)" +
                        @"\s+" +            //# Whitespace          \n
                        @"(?<size>\d+)" +
                        @"\s+" +            //# Whitespace          \n
                        @"(?<month>\w{3})" +          //# Month (3 letters)   \n
                        @"\s+" +            //# Whitespace          \n
                        @"(?<day>\d{1,2})" +        //# Day (1 or 2 digits) \n
                        @"\s+" +            //# Whitespace          \n
                        @"(?<timeyear>[\d:]{4,5})" +     //# Time or year        \n
                        @"\s+" +            //# Whitespace          \n
                        @"(?<filename>(.*))" +            //# Filename            \n
                        @"$";                //# End of line

                    var myresult = new List<FTPListDetail>();
                    foreach (var parsed in results)
                    {
                        var split = new Regex(regex)
                            .Match(parsed);
                        var dir = split.Groups["dir"].ToString();
                        var permission = split.Groups["permission"].ToString();
                        var filecode = split.Groups["filecode"].ToString();
                        var owner = split.Groups["owner"].ToString();
                        var group = split.Groups["group"].ToString();
                        var filename = split.Groups["filename"].ToString();
                        myresult.Add(new FTPListDetail()
                        {
                            Dir = dir,
                            Filecode = filecode,
                            Group = group,
                            FullPath = CurrentRemoteDirectory + "/" + filename,
                            Name = filename,
                            Owner = owner,
                            Permission = permission,
                        });
                    };
                    return myresult;
                }
            }
        }

        private FtpWebRequest GetWebRequest(string method, string uri, string Id, string Pw)
        {
            Uri serverUri = new Uri(uri);
            if (serverUri.Scheme != Uri.UriSchemeFtp)
            {
                return null;
            }
            var reqFTP = (FtpWebRequest)FtpWebRequest.Create(serverUri);
            reqFTP.Method = method;
            reqFTP.Credentials = new NetworkCredential(Id, Pw);
            return reqFTP;
        }
        public void UpLoadeFile(string srfile, string destPath, string Id, string Pw)
        {
            try
            {
                string serverAddress = conn.ftpUrl;
                string userId = Id;
                string userPass = Pw;
                string dir = destPath;
                string filePath = srfile;
                string fileName = Path.GetFileName(filePath);

                string uploadPath = serverAddress + dir + fileName;

                FtpWebRequest request = WebRequest.Create(uploadPath) as FtpWebRequest;

                Console.WriteLine("Passive: " + request.UsePassive);

                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(userId, userPass);

                byte[] fileContents = File.ReadAllBytes(filePath);
                Stream requestStream = request.GetRequestStream();
                request.ContentLength = fileContents.Length;
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse response = request.GetResponse() as FtpWebResponse;

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                Console.WriteLine(reader.ReadToEnd());

                Console.WriteLine("Complete, status {0}, length {1}", response.StatusDescription, response.ContentLength);

                response.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void DownLoadFile(string winfile, string serfile, string Id, string Pw)
        {
            FtpClient client = new FtpClient();
            client.Host = conn.ftpUrl;
            // if you don't specify login credentials, we use the "anonymous" user account
            client.Credentials = new NetworkCredential(Id, Pw);

            // begin connecting to the server
            client.Connect();
            // upload a file
            client.DownloadFile(winfile, serfile);
            // disconnect! good bye!
            client.Disconnect();
        }

    }
}
