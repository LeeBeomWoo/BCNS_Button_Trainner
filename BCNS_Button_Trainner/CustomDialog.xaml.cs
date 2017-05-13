using System.Windows;

namespace BCNS_Button_Trainner
{
    /// <summary>
    /// CustomDialog.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CustomDialog : Window
    {
        public CustomDialog()
        {
            InitializeComponent();
        }
        public string ResponseText
        {
            get { return yuotubead.Text; }
            set { yuotubead.Text = value; }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void CanCelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
