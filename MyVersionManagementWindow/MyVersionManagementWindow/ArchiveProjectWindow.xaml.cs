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
using System.Diagnostics;

namespace MyVersionManagementWindowNamespace
{
    /// <summary>
    /// ArchiveProjectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ArchiveProjectWindow : Window
    {
        public PassValuesHandler PassValuesEvent;

        public ArchiveProjectWindow(MyVersionManagementLib.M_ProjectClass project)
        {
            InitializeComponent();

            this.project = project;
        }

        /// <summary>
        /// 初始化可选版本值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MyVersionManagementLib.M_VersionClass version = this.project.CurrentProjectVersion.Version;

            List<int> a = new List<int>();

            for (int i = 0; i <= 1000; i++)
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

            this.mainCmb.SelectedIndex = version.MainVerNO;
            this.subCmb.SelectedIndex = version.SubVerNO;
            this.stageCmb.SelectedIndex = version.StageVerNO;
            this.greekCmb.SelectedIndex = (int)(version.GreekVerNO);

            this.projectNameTb.Text = this.project.ProjectName;
            this.projectLocationTb.Text = this.project.ProjectLocation;
            this.projectWorkspaceTb.Text = this.project.ProjectWorkspace;
            this.projectDescriptionTb.Text = this.project.CurrentProjectVersion.VersionDescription;
        }

        private void projectBtn_Click(object sender, RoutedEventArgs e)
        {
            var bt = sender as Button;

            if (bt != null)
            {
                if (bt.Name.Equals(this.viewLocationBtn.Name))
                {
                    Process.Start(this.project.ProjectLocation);

                    //System.Windows.Forms.FolderBrowserDialog dialog =
                    //    new System.Windows.Forms.FolderBrowserDialog();

                    //if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    //{
                    //    this.projectLocationTb.Text = dialog.SelectedPath;
                    //}
                }
                else if (bt.Name.Equals(this.selectOrViewWorkspaceBtn.Name))
                {
                    System.Windows.Forms.FolderBrowserDialog dialog =
                        new System.Windows.Forms.FolderBrowserDialog();

                    dialog.Description = "Your project files will be stored here.";
                    dialog.ShowNewFolderButton = true;
                    dialog.SelectedPath = this.project.ProjectWorkspace;

                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        this.projectWorkspaceTb.Text = dialog.SelectedPath;
                    }
                }
                else if (bt.Name.Equals(this.okBtn.Name))
                {
                    //MyVersionManagementLib.M_ProjectClass project = null;

                    //string name = this.projectNameTb.Text;
                    //string description = this.projectDescriptionTb.Text;
                    //string location = this.projectLocationTb.Text;
                    //string workspace = this.projectWorkspaceTb.Text;

                    int main = this.mainCmb.SelectedIndex;
                    int sub = this.subCmb.SelectedIndex;
                    int stage = this.stageCmb.SelectedIndex;
                    MyVersionManagementLib.M_VersionClass.GreekAlphabet greek =
                        (MyVersionManagementLib.M_VersionClass.GreekAlphabet)this.greekCmb.SelectedIndex;

                    MyVersionManagementLib.M_VersionClass version = new MyVersionManagementLib.M_VersionClass(DateTime.Now.ToString("yyyyMMdd"),
                        main, sub, stage, greek);

                    bool flag;
                    string archiveResult;
                    string resDescription = "";

                    this.project.CurrentProjectVersion.VersionDescription = this.projectDescriptionTb.Text;

                    //更新版本号，默认只更新日期版本号
                    if (main < 0 ||
                        sub < 0 ||
                        stage < 0 ||
                        greek < 0)
                    {
                        flag = false;

                        resDescription = "Project new version NO. is not select correctly.";
                    }
                    else
                    {
                        //判断是否需要进一步更新版本号（防止出现版本号相同的情况）
                        if(this.project.CurrentProjectVersion.Version.CompareTo(version, out resDescription) >= 0)
                        {
                            version.Update(out resDescription);
                        }

                        if (this.project.Archive(out archiveResult, version) > 0)
                        {
                            flag = true;

                            resDescription = "Project Archive successfully.";
                        }
                        else
                        {
                            flag = false;
                            resDescription = "Project Archive Failed, Details: " + archiveResult;
                        }
                    }

                    PassValuesEventArgs args = new PassValuesEventArgs(project, flag, resDescription);

                    PassValuesEventMethod(args);

                    this.Close();
                }
                else if (bt.Name.Equals(this.cancleBtn.Name))
                {
                    PassValuesEventArgs args = new PassValuesEventArgs(null, false, "Archive project canceled.");

                    PassValuesEventMethod(args);

                    this.Close();
                }
                else
                {
                    //
                }
            }
        }

        private void PassValuesEventMethod(PassValuesEventArgs args)
        {
            if (this.PassValuesEvent != null)
            {
                PassValuesEvent(this, args);
            }
        }

        private MyVersionManagementLib.M_ProjectClass project;
    }
}
