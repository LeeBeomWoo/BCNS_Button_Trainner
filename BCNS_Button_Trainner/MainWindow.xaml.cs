using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using ImageMagick;
using System.Drawing;
using BCNS;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Reflection;
using System.Linq;
using FluentFTP;
using Microsoft.Office.Interop.Word;
using System.Windows.Xps.Packaging;
using System.Windows.Documents;

namespace BCNS_Button_Trainner
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        Connect connect = new Connect();
        FTPTool ftpTool = new FTPTool();
        public string Id, PassWord;
        string upload, upimage, listimagetext, mainimagetext, category, upmovie, playText;
        public MainWindow(string loginId, string loginPw)
        {
            InitializeComponent();
            Id = loginId;
            PassWord = loginPw;
            directorycreate();
            Cars = new ObservableCollection<Car>();
            Cars.Add(new Car() { Name = "몸에 대하여", Series = new ObservableCollection<string>() { "상체 - 골격", "상체-근육", "하체-골격", "하체 - 근육" } });
            Cars.Add(new Car() { Name = "운동에 대하여", Series = new ObservableCollection<string>() { "운동법", "스트레칭", "셀프마사지" } });
            Cars.Add(new Car() { Name = "동영상", Series = new ObservableCollection<string>() { "상체", "하체", "코어", "유산소" } });
            Cars.Add(new Car() { Name = "음식", Series = new ObservableCollection<string>() { "체지방 감소 음식", "근력강화음식", "근육증가음식", "맛있는 음식" } });
            DataContext = this;
            ftpReset();
        }

        private void ftpReset()
        {
            treeView.Items.Clear();
            treeView.Items.Add(ftpTool.CreateDirectoryItem(connect.ftpUrl, Id, Id, PassWord));
            // treeView.Items.Add(ConvertToWpf(ftpTool.CreateDirectoryNode(connect.ftpUrl, Id, Id, PassWord)));
        }
        public ObservableCollection<Car> Cars { get; set; }

        public class Car
        {
            public string Name { get; set; }
            public ObservableCollection<string> Series { get; set; }
        }
        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
        string sDirPath = @"C:\BCNS_Trainer\Temp";
        string selectedfile;
        private void directorycreate()
        {
            //Dir value to save  
            DirectoryInfo di = new DirectoryInfo(sDirPath);  //Create Directoryinfo value by sDirPath  

            if (di.Exists == false)   //If New Folder not exits  
            {
                di.Create();             //create Folder  
            }
        }
        private TreeViewItem ConvertToWpf(TreeNode node)
        {
            var wpfItem = new TreeViewItem();
            wpfItem.Header = node.Text;
            foreach (TreeNode child in node.ChildNodes)
            {
                wpfItem.Items.Add(ConvertToWpf(child));
            }
            return wpfItem;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            bool? result = openfile.ShowDialog();
            if (result == true)
            {
                if (openfile.FileName.Length > 0)
                {
                    selectedfile = openfile.SafeFileName;
                    filepath_txtB.Text = selectedfile;                   
                }
                documentViewer_start(openfile.FileName, selectedfile);
            }
        }
        private void documentViewer_start(string path, string filname)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                MessageBox.Show("The file is invalid. Please select an existing file again.");
            }
            else
            {
                string convertedXpsDoc = string.Concat(Path.GetTempPath(), "\\", Guid.NewGuid().ToString(), ".xps");
                XpsDocument xpsDocument = ConvertWordToXps(path, convertedXpsDoc);
                if (xpsDocument == null)
                {
                    return;
                }                
                document.Document = xpsDocument.GetFixedDocumentSequence();
            }
        }
        public void SaveDocumentPagesToImages(IDocumentPaginatorSource document, string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath)) return;

            if (dirPath[dirPath.Length - 1] != '\\')
                dirPath += "\\";

            if (!Directory.Exists(dirPath)) return;

            MemoryStream[] streams = null;
            try
            {
                int pageCount = document.DocumentPaginator.PageCount;
                DocumentPage[] pages = new DocumentPage[pageCount];
                for (int i = 0; i < pageCount; i++)
                    pages[i] = document.DocumentPaginator.GetPage(i);

                streams = new MemoryStream[pages.Count()];

                for (int i = 0; i < pages.Count(); i++)
                {
                    DocumentPage source = pages[i];
                    streams[i] = new MemoryStream();

                    RenderTargetBitmap renderTarget =
                       new RenderTargetBitmap((int)source.Size.Width,
                                               (int)source.Size.Height,
                                               96, // WPF (Avalon) units are 96dpi based
                                               96,
                                              PixelFormats.Default);

                    renderTarget.Render(source.Visual);

                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();  // Choose type here ie: JpegBitmapEncoder, etc
                    encoder.QualityLevel = 100;
                    encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                    encoder.Save(streams[i]);

                    FileStream file = new FileStream(dirPath + Path.GetFileNameWithoutExtension(selectedfile) + "_" + (i + 1) + ".jpg", FileMode.CreateNew);
                    file.Write(streams[i].GetBuffer(), 0, (int)streams[i].Length);
                    file.Close();
                    mainimagetext = Path.Combine(sDirPath, Path.GetFileNameWithoutExtension(selectedfile) + "_" + (i + 1) + ".jpg");
                    streams[i].Position = 0;
                }
            }
            catch (Exception e1)
            {
                throw e1;
            }
            finally
            {
                if (streams != null)
                {
                    foreach (MemoryStream stream in streams)
                    {
                        stream.Close();
                        stream.Dispose();
                    }
                }
            }
        }
        private XpsDocument ConvertWordToXps(string wordFilename, string xpsFilename)
        {
            // Create a WordApplication and host word document 
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            try
            {
                wordApp.Documents.Open(wordFilename);

                // To Invisible the word document 
                wordApp.Visible = false;

                // Minimize the opened word document 
                wordApp.WindowState = WdWindowState.wdWindowStateMinimize;


                Document doc = wordApp.ActiveDocument;

                doc.SaveAs(xpsFilename, WdSaveFormat.wdFormatXPS);


                XpsDocument xpsDocument = new XpsDocument(xpsFilename, FileAccess.ReadWrite);
                return xpsDocument;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurs, The error message is  " + ex.ToString());
                return null;
            }
            finally
            {
                wordApp.Documents.Close();
                wordApp.Quit(WdSaveOptions.wdDoNotSaveChanges);
            }
        }
        /// <summary> 
        ///  Convert the word document to xps document 
        /// </summary> 
        /// <param name="wordFilename">Word document Path</param> 
        /// <param name="xpsFilename">Xps document Path</param> 
        /// <returns></returns> 
        private void category_seperate (int a)
        {
            string[] body = new string[] { "상체_골격", "상체_근육", "하체_골격", "하체_근육" };
            string[] exercise = new string[] { "운동법", "스트레칭", "셀프마사지"};
            string[] movie = new string[] { "상체", "하체", "코어", "유산소" };
            string[] food = new string[] { "체지방 감소식", "근력강화식", "근육증가식", "맛있는 음식" };
           
        }
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "AddFile", BodyStyle = WebMessageBodyStyle.Wrapped)]
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveDocumentPagesToImages(document.Document, sDirPath);
            ftpTool.UpLoadeFile(mainimagetext, "/vvaeru", Id, PassWord);
            /**
            UpLoadD uploaddata = new UpLoadD();
            uploaddata.Id = Id;
            switch (category_1.SelectedIndex)
            {
                case 0:
                    uploaddata.Table = "body_list";
                    category = "body";
                    switch (category_2.SelectedIndex)
                    {
                        case 0:
                            uploaddata.Category = "upper_skeletone";
                            break;
                        case 1:
                            uploaddata.Category = "upper_muscle";
                            break;
                        case 2:
                            uploaddata.Category = "lower_skeletone";
                            break;
                        case 3:
                            uploaddata.Category = "lower_muscle";
                            break;
                    }
                    break;    
                case 1:
                    category = "exercise";
                    uploaddata.Table = "exercise_list";
                    switch (category_2.SelectedIndex)
                    {
                        case 0:
                            uploaddata.Category = "method";
                            break;
                        case 1:
                            uploaddata.Category = "streching";
                            break;
                        case 2:
                            uploaddata.Category = "selfmassage";
                            break;
                    }
                    break;
                case 2:
                    category = "follow";
                    uploaddata.Table = "follow_list";
                    switch (category_2.SelectedIndex)
                    {
                        case 0:
                            uploaddata.Category = "upper";
                            break;
                        case 1:
                            uploaddata.Category = "lower";
                            break;
                        case 2:
                            uploaddata.Category = "core";
                            break;
                        case 3:
                            uploaddata.Category = "breath";
                            break;
                    }
                    break;
                case 3:
                    category = "food";
                    uploaddata.Table = "food_list";
                    switch (category_2.SelectedIndex)
                    {
                        case 0:
                            uploaddata.Category = "fat";
                            break;
                        case 1:
                            uploaddata.Category = "power";
                            break;
                        case 2:
                            uploaddata.Category = "muscle";
                            break;
                        case 3:
                            uploaddata.Category = "good";
                            break;
                    }
                    break;
            }
            uploaddata.Id = Id;
            uploaddata.Title = title_txtB.Text;
            uploaddata.Content = content_txtB.Text;
            uploaddata.ListImage = "listimage/" + listimagetext;
            uploaddata.ImageUrl = mainimagetext;
            uploaddata.OutCategory = category_1.SelectedItem.ToString() + "-" + category_2.SelectedItem.ToString();
            //사용자 에게 나오는 카테고리
            string a = connect.MainPut(uploaddata);
            string[] b = new string[2];
            b.SetValue(category + "/" + uploaddata.Category + "/" + mainimagetext, 0);
            b.SetValue(category + "/" + uploaddata.Category + "/" + uploaddata.ListImage, 1);
            ftpTool.UpLoadeFile(b[0], b[0], Id, PassWord);
            switch (category_1.SelectedIndex)
            {
                case 0:
                    break;
            }
            ftpTool.UpLoadeFile(b[1], b[1], Id, PassWord);
            if (a == "OK")
            {
                MessageBox.Show(a);
            }else
            {
                MessageBox.Show("정상적으로 업로드 되지 않았습니다. 다시한번 업로드 하여 주시기 바랍니다. 만약 같은 메세지가 반복 될 경우 네트워크문제가 없다면 관리자에게 문의 바랍니다.");
            }
            **/
        }
        private void category_1_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
           
        }
        private static string GetEmbedUrlFromLink(string link)
        {
            try
            {
                string embedUrl = link.Substring(0).Replace("https://youtu.be", "https://www.youtube.com/embed");
                embedUrl = "< iframe width = \"1280\" height = \"720\" src = \"" + embedUrl + "\" frameborder = \"0\" allowfullscreen ></ iframe > ";
                Console.WriteLine(embedUrl);
                return embedUrl;
            }
            catch
            {
                return link;
            }
        }
        private void Label_MouseLeftButtonUp(object sender, RoutedEventArgs e)
        {
            tab.SelectedIndex = 1;
            var dialog = new CustomDialog();
            if (dialog.ShowDialog() == true)
            {
                playText =  dialog.ResponseText;
                upmovie = GetEmbedUrlFromLink(playText);
                Console.WriteLine(upmovie);
                filepath_txtB.Text = upmovie;
                Uri uri = new Uri(playText, UriKind.RelativeOrAbsolute);

                // Only absolute URIs can be navigated to
                if (!uri.IsAbsoluteUri)
                {
                    MessageBox.Show("The Address URI must be absolute eg 'http://www.microsoft.com'");
                    return;
                }

                // Navigate to the desired URL by calling the .Navigate method
                youtube_view.Navigate(uri);
            }
        }

        private void listImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            bool? result = opf.ShowDialog();
            if (result == true)
            {
                ListImage = new BitmapImage(new Uri(opf.FileName)); //you change source of the Image
                upimage = opf.FileName;
                listImage.Background = new ImageBrush(ListImage);
                listimagetext = opf.SafeFileName;
                listImage.Content = null;
            }
        }
        private ImageSource _face;
        public ImageSource ListImage
        {
            get { return _face; }
            set
            {
                _face = value;
                OnPropertyChanged("ListImage");
            }
        }        

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }        
    }
}
    
   

