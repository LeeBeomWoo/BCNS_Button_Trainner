using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Web;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BCNS;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using FluentFTP;

namespace BCNS_Button_Trainner
{
    /// <summary>
    /// join_information.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class join_information : Window
    {
        FileUpload fileupload = new FileUpload();
        Person person = new Person();
        FTPTool ftpTool = new FTPTool();
        JObject json = new JObject();
        string[] uploadfilelist = new string[6];
        public join_information()
        {
            InitializeComponent();
        }
        String name, id, pass, homepage, nickname, email, call, faceStr, abStr, adress, license_1, licencse_2;
        int emailCb, callCb, sex, age, section;
        Connect connect = new Connect();
        private void face_image_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            bool? result = opf.ShowDialog();
            if (result == true)
            {
                FaceImage = new BitmapImage(new Uri(opf.FileName)); //you change source of the Image
                faceStr = opf.FileName;
                face_image.Background = new ImageBrush(FaceImage);
                face_lbl.Visibility = Visibility.Hidden;
            }
        }

        private void exit_btn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void License_1_txtB_GotFocus(object sender, RoutedEventArgs e)
        {
            License_1_txtB.Text = "";
        }

        private void License_2_txtB_GotFocus(object sender, RoutedEventArgs e)
        {
            License_2_txtB.Text = "";
        }

        private void licence_1_txtB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            bool? result = opf.ShowDialog();
            if (result == true)
            {
                licence_1_file_txtB.Text = opf.SafeFileName;
            }
        }

        private void license_2_txtB_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            bool? result = opf.ShowDialog();
            if (result == true)
            {
                license_2_file_txtB.Text = opf.SafeFileName;
            }
        }
        public void databaseStringPut(string name_Put, int age_Put, int sex_Put, string id_Put, string pass_Put, string homepage_Put, string nickname_Put, string email_Put, string call_Put, int call, int email, string adress, string license_1, string license_2, string faceStr, string abStr, string license_1_Str, string license_2_Str, string section)
        {
            person.Name = name_Put;
            person.Age = age_Put.ToString();
            person.Sex = sex_Put.ToString();
            person.Id = id_Put;
            person.PassWord = pass_Put;
            person.HomePage = homepage_Put;
            person.NickName = nickname_Put;
            person.Email = email_Put;
            person.Call = call_Put;
            person.Call_open = call.ToString();
            person.Address = adress;
            person.License = license_1;
            person.License_1 = license_2;
            person.License_AA = License_AA_1_txtB.Text;
            person.License_1_AA = License_AA_2_txtB.Text;
            person.FaceImage = faceStr;
            person.BodyImage = abStr;
            person.Permission = 0.ToString();
            person.Award = award_1_txtB.Text;
            person.Award_1 = award_2_txtB.Text;
            person.Award_AA = award_AA_1_txtB1.Text;
            person.Award_1_AA = award_AA_2_txtB.Text;
            person.Section = section;
            string json = JsonConvert.SerializeObject(person);
            string a = connect.JoinPut(person);
            if (a == "true")
            {
                uploadfilelist.SetValue("/person/face/" + person.Id + "_" + faceStr, 0);
                uploadfilelist.SetValue("/person/body/" + person.Id + "_" + abStr, 1);
                uploadfilelist.SetValue("/person/license/" + person.Id + "_" + license_1_Str, 2);
                ftpTool.UpLoadeFile(faceStr, uploadfilelist[0], id_Put, pass_Put);
                ftpTool.UpLoadeFile(abStr, uploadfilelist[1], id_Put, pass_Put);
                ftpTool.UpLoadeFile(license_1_Str, uploadfilelist[2], id_Put, pass_Put);
                if (license_2_Str != null)
                {
                uploadfilelist.SetValue("/person/license/" + person.Id + "_" + license_2_Str, 3);
                    ftpTool.UpLoadeFile(license_2_Str, uploadfilelist[3], id_Put, pass_Put);
                }
                if (award_AA_1_file_txtB != null)
                {
                    uploadfilelist.SetValue("/person/award/" + person.Id + "_" + award_AA_1_file_txtB.Text, 4);
                    ftpTool.UpLoadeFile(award_AA_1_file_txtB.Text, uploadfilelist[4], id_Put, pass_Put);
                }
                if (award_AA_2_file_txtB != null)
                {
                    uploadfilelist.SetValue("/person/award/" + person.Id + "_" + award_AA_2_file_txtB.Text, 5);
                    ftpTool.UpLoadeFile(award_AA_2_file_txtB.Text, uploadfilelist[5], id_Put, pass_Put);
                }
                // AsynchronousFtpUpLoader.FtpMain(uploadfilelist, person.Id, person.PassWord);
                MessageBox.Show("정상적으로 가입되었습니다. 관리자의 검토후 승인이 완료 된 후 개별연락이 이루어 지니 약 1주일 정도 이후 다시 접속하여 주시기 바랍니다.");
            } else
            {
                MessageBox.Show("가입이 되지 않았습니다. 다시 시도하여 주시기 바랍니다.");
            }
           
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            name = name_txtB.Text;
            age = int.Parse(age_txtB.Text);
            sex = sex_txtB.SelectedIndex;
            id = id_txtB.Text;
            pass = pass_txtB.Text;
            homepage = homepage_txtB.Text;
            nickname = nick_txtB.Text;
            email = email_txtB.Text;
            call = call_txtB.Text;
            license_1 = License_1_txtB.Text;
            licencse_2 = License_2_txtB.Text;
            adress = adress_txtB.Text;
            switch (email_Cb.SelectedIndex)
            {
                case 0:
                    emailCb = 0;
                    break;
                case 1:
                    emailCb = 1;
                    break;
            }
            switch (call_Cb.SelectedIndex)
            {
                case 0:
                    callCb = 0;
                    break;
                case 1:
                    callCb = 1;
                    break;
            }
            switch (section_com.SelectedIndex)
            {
                case 0:
                    section = 0;
                    break;
                case 1:
                    section = 1;
                    break;
            }
            if (name != null && id != null && pass != null && homepage != null && nickname != null && email != null && call != null && faceStr != null && abStr != null && license_1 !=null && licence_1_file_txtB.Text != null)
            {
                databaseStringPut(name, age, sex, id, pass, homepage, nickname, email, call, callCb, emailCb, adress, license_1, licencse_2, faceStr, abStr, licence_1_file_txtB.Text, license_2_file_txtB.Text, section.ToString());
                //디비에서 정상적으로 전송이 되었나 안되었나 확인하는 코드 추가 해야함.
                /// if(성공적으로 전송이 되었는지 판단){
                /// { 
                /// MessageBox.Show("성공적으로 가입되었습니다. 자격검증과정이 끝난 후 개별 문자연락 가오니 문자연락 이후 활동하여 주시면 됩니다. 앞으로 많은 자료와 본인의 홍보 부탁드려요~!^^");
                /// this.close();
                /// } else {
                ///  MessageBox.Show("빈 란이 있습니다. 다시한번 확인하여 주시고 다 채워 주신 후 다시 신청하여 주시기 바랍니다. 자격증의 경우 1번은 필수 이고 2번은 선택입니다 참고하시기 바랍니다.");
                ///  }
            }
            this.Close();

        }
        private ImageSource _face, _all;

        public ImageSource FaceImage
        {
            get { return _face; }
            set
            {
                _face = value;
                OnPropertyChanged("FaceImage");
            }
        }

        public ImageSource AllBody
        {
            get { return _all; }
            set
            {
                _all = value;
                OnPropertyChanged("AllBody");

            }
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            bool? result = opf.ShowDialog();
            if (result == true)
            {
                AllBody = new BitmapImage(new Uri(opf.FileName)); //you change source of the Image
                abStr = opf.FileName;
                image_box.Background = new ImageBrush(AllBody);
                all_lbl.Visibility = Visibility.Hidden;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
