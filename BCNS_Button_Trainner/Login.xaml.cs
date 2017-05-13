using System.Windows;
using BCNS;
using System.Collections.Generic;
using ScottGuHelper.JSON;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

namespace BCNS_Button_Trainner
{
    /// <summary>
    /// Login.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Login : Window
    {
        Connect connect = new Connect();
        Person person = new Person();
        Person reCeive;
        private string Id, PassWord;

        public Login()
        {
            InitializeComponent();
            id_txtB.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            join_information join = new join_information();
            join.Owner = this;
            join.Show();
        }
      
        private void loginMethod()
        {
            if (id_txtB.Text != null && pass_txtB.Password != null)
            {
                person.Id = id_txtB.Text;
                person.PassWord = pass_txtB.Password;
                reCeive = connect.LoginPost(person);
                Console.WriteLine("ID =" + person.Id + "PASS = " + person.PassWord);
                if (reCeive.Id == person.Id)
                {
                    Id = reCeive.Id;
                    PassWord = reCeive.PassWord;
                    if (Int32.Parse(reCeive.Permission) == 1)
                    {
                        var mainWindow = new MainWindow(Id, PassWord);
                        //Re-enable normal shutdown mode.
                        mainWindow.Show();
                    }
                    else if (Int32.Parse(reCeive.Permission) == 0)
                    {
                        MessageBox.Show("현재 가입 심사중에 있습니다. 심사가 완료되면 개별연락 후 본격적인 활동이 가능하오니 기다려 주시면 감사하겠습니다.");
                        Environment.Exit(0);
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                        this.Close();
                    }
                    else
                    {
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                        this.Close();
                    }
                    this.Close();
                }
                else
                {
                    Console.WriteLine("ID =" + reCeive.Id + "PASS = " + reCeive.PassWord);
                    MessageBox.Show("아이디와 비밀번호 중 잘 못 입력된 부분이 있습니다. 확인 후 다시 입력하여 주시기 바랍니다.");
                    id_txtB.Focus();
                }
            }
            else
            {
                MessageBox.Show("빈곳이 있습니다. 확인 후 모두 입력하여 주세요.");
            }

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            loginMethod();

        }

        private void Window_Closed(object sender, EventArgs e)
        {
           

        }
        public string id
        {
            get { return Id; }
        }

        private void id_txtB_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                pass_txtB.Focus();
            }
        }

        private void pass_txtB_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                loginMethod(); ;
            }

        }

        public string Pw
        {
            get { return PassWord; }
        }
    }
}
