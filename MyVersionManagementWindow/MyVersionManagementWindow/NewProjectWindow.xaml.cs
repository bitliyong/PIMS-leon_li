using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyVersionManagementWindowNamespace
{
    /// <summary>
    /// NewProjectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewProjectWindow : Window
    {
        public PassValuesHandler PassValuesEvent;

        public NewProjectWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 初始化可选版本值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<int> a = new List<int>();

            for(int i = 0; i <= 10; i++)
            {
                a.Add(i);
            }

            this.mainCmb.Items.Clear();
            this.subCmb.Items.Clear();
            this.stageCmb.Items.Clear();
            this.greekCmb.Items.Clear();

            this.mainCmb.ItemsSource = a;
            this.subCmb.ItemsSource = a;
            this.stageCmb.ItemsSource = a;
            this.greekCmb.ItemsSource = Enum.GetNames(typeof(MyVersionManagementLib.M_VersionClass.GreekAlphabet));

            this.mainCmb.SelectedIndex = 1;
            this.subCmb.SelectedIndex = 0;
            this.stageCmb.SelectedIndex = 0;
            this.greekCmb.SelectedIndex = 0;
        }

        private void projectBtn_Click(object sender, RoutedEventArgs e)
        {
            var bt = sender as Button;

            if (bt != null)
            {
                if (bt.Name.Equals(this.selectLocationBtn.Name))
                {
                    System.Windows.Forms.FolderBrowserDialog dialog = 
                        new System.Windows.Forms.FolderBrowserDialog();

                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        this.projectLocationTb.Text = dialog.SelectedPath;
                    }
                }
                else if (bt.Name.Equals(this.selectWorkspaceBtn.Name))
                {
                    System.Windows.Forms.FolderBrowserDialog dialog = 
                        new System.Windows.Forms.FolderBrowserDialog();

                    dialog.Description = "Your project files will be stored here.";
                    dialog.ShowNewFolderButton = true;

                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        this.projectWorkspaceTb.Text = dialog.SelectedPath;
                    }
                }
                else if (bt.Name.Equals(this.okBtn.Name))
                {
                    MyVersionManagementLib.M_ProjectClass project = null;
                    
                    string name = this.projectNameTb.Text;
                    string description = this.projectDescriptionTb.Text;
                    string location = this.projectLocationTb.Text;
                    string workspace = this.projectWorkspaceTb.Text;

                    int main = this.mainCmb.SelectedIndex+1;
                    int sub = this.subCmb.SelectedIndex;
                    int stage = this.stageCmb.SelectedIndex;
                    MyVersionManagementLib.M_VersionClass.GreekAlphabet greek = 
                        (MyVersionManagementLib.M_VersionClass.GreekAlphabet)this.greekCmb.SelectedIndex;

                    MyVersionManagementLib.M_VersionClass version = new MyVersionManagementLib.M_VersionClass(DateTime.Now.ToString("yyyyMMdd"),
                        main, sub, stage, greek);

                    bool flag;
                    string resDescription = "";

                    //create a new solution
                    if (string.IsNullOrWhiteSpace(name) ||
                        string.IsNullOrWhiteSpace(location) ||
                        string.IsNullOrWhiteSpace(workspace) ||
                        string.IsNullOrWhiteSpace(description))
                    {
                        flag = false;

                        resDescription = "Project Name/Location/Workspace/Description is null or whitespace.";
                    }
                    else
                    {
                        project = new MyVersionManagementLib.M_ProjectClass(name, location, workspace, 
                            description, version);

                        flag = true;

                        resDescription = "Project create successfully.";
                    }

                    PassValuesEventArgs args = new PassValuesEventArgs(project, flag, resDescription);

                    PassValuesEvent(this, args);

                    this.Close();
                }
                else if (bt.Name.Equals(this.cancleBtn.Name))
                {
                    PassValuesEventArgs args = new PassValuesEventArgs(null, false, "Create project canceled.");

                    PassValuesEvent(this, args);

                    this.Close();
                }
                else
                {
                    //
                }
            }
        }
    }
}
