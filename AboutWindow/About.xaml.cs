using System.Windows;
using AboutInfo;
using VersionManagement;
using System.Windows.Controls;

namespace About
{
    /// <summary>
    /// AboutWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AboutWindow : Window
    {
        AboutInfomation about;

        public AboutWindow()
        {
            InitializeComponent();
        }

        public AboutWindow(AboutInfomation aboutInfo)
        {
            InitializeComponent();

            this.about = aboutInfo;

            UpdateDisplay(aboutInfo);
        }

        private void UpdateDisplay(AboutInfomation aboutInfo)
        {
            this.name.Text = aboutInfo.ProductName;
            this.version.Text = aboutInfo.ProductVersion;
            this.copyright.Text = aboutInfo.Copyright;
            this.author.Text = aboutInfo.Author;
            this.description.Text = aboutInfo.Description;
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                if (btn.Name.Equals(this.OK.Name))
                {
                    //this.about.UpdateVersion(VersionMananger.UpdateDegree.Stage_Update);

                    this.Close();
                }
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.history.ItemsSource = this.about.HistoryInfo;
        }
    }
}
