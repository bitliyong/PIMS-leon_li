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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

using My_FileAndFolderOperationLib_Namespace;
using MyVersionManagementLib;
using System.IO;

namespace MyVersionManagementWindowNamespace
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VersionManagementMainWindow : Window
    {
        #region Fields
        private TreeViewItem currentProjectItem = null;
        #endregion

        #region Windows
        public VersionManagementMainWindow()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            if (mi != null)
            {
                if (mi.Header.Equals("_New Solution"))
                {
                    //M_SolutionClass m = new M_SolutionClass("Solution_" + DateTime.Now.ToString("yyyyMMdd hh:mm:ss"), "hello");

                    //this.currentSolutionCollection.SolutionsCollection.Add(m);

                    NewSolutionWindow nsw = new NewSolutionWindow();

                    nsw.PassValuesEvent += GetValuesHandler;

                    nsw.ShowDialog();
                }
                else if (mi.Header.Equals("_Add Project to Current Solution"))
                {
                    M_ProjectClass p = new M_ProjectClass("Project_" + DateTime.Now.ToString("yyyyMMdd hh:mm:ss"), "D:\\test", "D:\\test", "hello"
                        , new M_VersionClass());

                    this.currentSolutionCollection.CurrentSolution.SolutionProjectsCollection.Add(p);
                }
                else if (mi.Header.Equals("_Archive Project"))
                {
                    ArchiveProjectWindow apw = new ArchiveProjectWindow(this.currentProject);

                    apw.PassValuesEvent += this.GetValuesHandler;

                    apw.ShowDialog();
                }
            }
        }

        private void TreeView_Selected(object sender, RoutedEventArgs e)
        {
            TreeView tree = sender as TreeView;

            if (tree != null)
            {
                TreeViewItem ti = e.OriginalSource as TreeViewItem;

                if (ti != null)
                {
                    ti.IsExpanded = !ti.IsExpanded;
                }

                M_SolutionClass s = tree.SelectedItem as M_SolutionClass;

                if (s != null)
                {
                    GetSolutionParents(s);

                    SetTextBoxBindings();
                }
                else
                {
                    M_ProjectClass p = tree.SelectedItem as M_ProjectClass;

                    if (p != null)
                    {
                        GetProjectParents(p);

                        SetTextBoxBindings();

                        //this.currentProjectItem = ti;
                    }
                    else
                    {
                        M_ProjectVersionClass v = tree.SelectedItem as M_ProjectVersionClass;

                        if (v != null)
                        {
                            GetProjectVersionParents(v);

                            SetTextBoxBindings();
                        }
                    }
                }
            }
        }

        private void projectBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            if (btn != null)
            {
                if (btn.Name.Equals(this.viewLocationBtn.Name))
                {
                    ViewInWindowsExplorer(this.projectLocationTb.Text);
                }
                else if (btn.Name.Equals(this.viewWorkspaceBtn.Name))
                {
                    ViewInWindowsExplorer(this.projectWorkspaceTb.Text);
                }
                else
                {
                    //
                }
            }
        }

        private void projectVersionBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            if (btn != null)
            {
                if (btn.Name.Equals(this.viewVersionLocationBtn.Name))
                {
                    ViewInWindowsExplorer(this.projectVersionLocationTb.Text);
                }
                else if (btn.Name.Equals(this.openBtn.Name))
                {
                    OpenByProperApplication(this.projectWorkspaceTb.Text);
                }
                else
                {
                    //
                }
            }
        }

        private void descriptionTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = sender as TextBox;

            if (t != null)
            {
                if (t.Name.Equals(this.solutionDescriptionTb.Name))
                {
                    if (this.solutionDescriptionTb != null &&
                        this.currentSolution != null)
                    {
                        this.currentSolution.SolutionDescription = this.solutionDescriptionTb.Text;
                    }
                }
                else if (t.Name.Equals(this.projectDescriptionTb.Name))
                {
                    if (this.projectDescriptionTb != null &&
                        this.currentProject != null)
                    {
                        this.currentProject.ProjectDescription = this.projectDescriptionTb.Text;
                    }
                }
                else if (t.Name.Equals(this.projectVersionDescriptionTb.Name))
                {
                    if (this.projectVersionDescriptionTb != null &&
                        this.currentVersion != null)
                    {
                        this.currentVersion.VersionDescription = this.projectVersionDescriptionTb.Text;
                    }
                }
            }
        }
        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.currentSolutionCollection = new M_SolutionCollectionClass("MySolutionCollection", "My Solutions Collection");

            string directoryPath = Environment.CurrentDirectory + "\\datas";
            
            DirectoryInfo directory = new DirectoryInfo(directoryPath);

            if(!directory.Exists)
            {
                directory.Create();
            }

            this.savePath = directory.FullName  + "\\save.sav";

            string resultDescription;

            //载入数据成功
            if (this.currentSolutionCollection.Load(this.savePath, Encoding.Default, out resultDescription) > 0)
            {
                this.currentSolution = this.currentSolutionCollection.CurrentSolution;

                if (this.currentSolution != null)
                {
                    this.currentProject = this.currentSolution.CurrentProject;

                    if (this.currentProject != null)
                    {
                        this.currentVersion = this.currentProject.CurrentProjectVersion;
                    }
                    else
                    {
                        this.currentVersion = new M_ProjectVersionClass();
                    }
                }
                else
                {
                    this.currentProject = new M_ProjectClass();
                }
            }
            else
            {
                MessageBox.Show(resultDescription);

                this.currentSolution = new M_SolutionClass();
                this.currentProject = new M_ProjectClass();
                this.currentVersion = new M_ProjectVersionClass();
            }

            //设置数据绑定
            SetBindings();

            this.solutionsViewTv.DataContext = this.currentSolutionCollection.SolutionsCollection;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string resultDescription;

            //保存数据
            if (this.currentSolutionCollection.Save(this.savePath, Encoding.Default, out resultDescription) <= 0)
            {
                MessageBox.Show(resultDescription);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
        #endregion

        #region User
        /// <summary>
        /// 在资源管理器中查看文件或文件夹
        /// </summary>
        /// <param name="path"></param>
        private void ViewInWindowsExplorer(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("无效的路径！");
            }
            else
            {
                Process.Start(path);
            }
        }
        /// <summary>
        /// 使用合适的应用打开
        /// </summary>
        /// <param name="path"></param>
        private void OpenByProperApplication(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                MessageBox.Show("无效的路径！");
            }
            else
            {
                Process.Start(path);
            }
        }

        private void SetBindings()
        {
            SetTextBoxBindings();
            SetLabelBindings();
            SetProgressBarBindings();
            //SetRunBindings();
        }

        private void SetRunBindings()
        {
            //SetRunBinding(this.currentSolution, "SolutionDescription", this.solutionDescriptionRun);
            //SetRunBinding(this.currentProject, "ProjectDescription", this.projectDescriptionRun);
            //SetRunBinding(this.currentVersion, "Description", this.projectVersionDescriptionRun);
        }

        private void SetProgressBarBindings()
        {
            SetProgressBarBinding(this.currentProject.Copier, "ProcessedFilePercent", this.processBar);
        }

        private void SetLabelBindings()
        {
            SetLabelBinding(this.currentProject.Copier, "ProcessedPercent", this.percentLb);
            //SetLabelBinding(this.currentProject.Copier, "ProcessedPercent", this.percentLb);
        }

        private void SetTextBoxBindings()
        {
            SetTextBoxBinding(this.currentSolution, "SolutionName", this.solutoinTitleTb);
            SetTextBoxBinding(this.currentSolution, "SolutionDescription", this.solutionDescriptionTb);
            SetTextBoxBinding(this.currentProject, "ProjectName", this.projectNameTb);
            SetTextBoxBinding(this.currentProject, "ProjectLocation", this.projectLocationTb);
            SetTextBoxBinding(this.currentProject, "ProjectWorkspace", this.projectWorkspaceTb);
            SetTextBoxBinding(this.currentProject, "ProjectDescription", this.projectDescriptionTb);
            SetTextBoxBinding(this.currentVersion, "VersionNo", this.projectVersionTb);
            SetTextBoxBinding(this.currentVersion, "Path", this.projectVersionLocationTb);
            SetTextBoxBinding(this.currentVersion, "VersionDescription", this.projectVersionDescriptionTb);
        }
        /// <summary>
        /// 设置run的文本绑定
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="property"></param>
        /// <param name="targetRun"></param>
        private void SetRunBinding(object sourceObject, string property, Run targetRun)
        {
            try
            {
                Binding binding = new Binding();

                binding.Source = sourceObject;
                binding.Path = new PropertyPath(property);
                binding.Mode = BindingMode.OneWay;

                targetRun.SetBinding(Run.TextProperty, binding);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        /// <summary>
        /// 设置进度条value绑定
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="property"></param>
        /// <param name="targetProgressBar"></param>
        private void SetProgressBarBinding(object sourceObject, string property, ProgressBar targetProgressBar)
        {
            try
            {
                Binding binding = new Binding();

                binding.Source = sourceObject;
                binding.Path = new PropertyPath(property);

                targetProgressBar.SetBinding(ProgressBar.ValueProperty, binding);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        /// <summary>
        /// 设置label的content绑定
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="property"></param>
        /// <param name="targetLabel"></param>
        private void SetLabelBinding(object sourceObject, string property, Label targetLabel)
        {
            try
            {
                Binding binding = new Binding();

                binding.Source = sourceObject;
                binding.Path = new PropertyPath(property);

                targetLabel.SetBinding(Label.ContentProperty, binding);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        /// <summary>
        /// 设置textbox的文本绑定
        /// </summary>
        /// <param name="sourceObject"></param>
        /// <param name="property"></param>
        /// <param name="targetBox"></param>
        private void SetTextBoxBinding(object sourceObject, string property, TextBox targetBox)
        {
            try
            {
                if (sourceObject == null ||
                string.IsNullOrEmpty(property) ||
                targetBox == null)
                {
                    return;
                }

                Binding binding = new Binding();

                binding.Source = sourceObject;
                binding.Path = new PropertyPath(property);

                targetBox.SetBinding(TextBox.TextProperty, binding);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void GetValuesHandler(object sender, PassValuesEventArgs args)
        {
            //获取新建的解决方案
            NewSolutionWindow nsw = sender as NewSolutionWindow;

            if (nsw != null &&
                args != null)
            {
                bool flag = args.Flag;

                if (flag == true)
                {
                    M_SolutionClass m = args.Result as M_SolutionClass;

                    if (m != null)
                    {
                        this.currentSolutionCollection.SolutionsCollection.Add(m);
                    }
                }

                this.otherLb.Content = args.Description;
            }
            else
            {
                NewProjectWindow npw = sender as NewProjectWindow;

                if(nsw != null &&
                    args != null)
                {
                    bool flag = args.Flag;

                    if (flag == true)
                    {
                        M_ProjectClass m = args.Result as M_ProjectClass;

                        if (m != null)
                        {
                            this.currentSolution.SolutionProjectsCollection.Add(m);
                        }
                    }

                    this.otherLb.Content = args.Description;
                }
                else
                {
                    ArchiveProjectWindow apw = sender as ArchiveProjectWindow;

                    if (apw != null &&
                        args != null)
                    {
                        //bool flag = args.Flag;

                        //if (flag == true)
                        //{
                        //    M_SolutionClass m = args.Result as M_SolutionClass;

                        //    if (m != null)
                        //    {
                        //        this.currentSolutionCollection.SolutionsCollection.Add(m);
                        //    }
                        //}

                        this.otherLb.Content = args.Description;
                        
                        //更新解决方案显示
                        //this.currentProjectItem.Header = this.currentProject.CurrentProjectVersion.Name;
                    }
                    else
                    {
                        this.otherLb.Content = "Incorrect Sender or Invalid args";
                    }
                }
            }
        }
        /// <summary>
        /// 获取当前工程版本的递归父级对象
        /// </summary>
        /// <param name="project"></param>
        private void GetProjectVersionParents(M_ProjectVersionClass version)
        {
            if(this.currentSolutionCollection != null &&
                this.currentSolutionCollection.SolutionsCollection != null &&
                this.currentSolutionCollection.SolutionsCollection.Count > 0)
            {
                foreach (M_SolutionClass s in this.currentSolutionCollection.SolutionsCollection)
                {
                    if(s != null &&
                        s.SolutionProjectsCollection != null &&
                        s.SolutionProjectsCollection.Count > 0)
                    {
                        foreach (M_ProjectClass p in s.SolutionProjectsCollection)
                        {
                            if(p != null &&
                                p.ProjectVersionCollection != null &&
                                p.ProjectVersionCollection.Count > 0)
                            {
                                foreach (M_ProjectVersionClass v in p.ProjectVersionCollection)
                                {
                                    if (v.Equals(version))
                                    {
                                        this.currentSolution = s;
                                        this.currentProject = p;
                                        this.currentVersion = v;

                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取当前工程的父级对象
        /// </summary>
        /// <param name="project"></param>
        private void GetProjectParents(M_ProjectClass project)
        {
            if (this.currentSolutionCollection != null &&
                this.currentSolutionCollection.SolutionsCollection != null &&
                this.currentSolutionCollection.SolutionsCollection.Count > 0)
            {
                foreach (M_SolutionClass s in this.currentSolutionCollection.SolutionsCollection)
                {
                    if (s != null &&
                        s.SolutionProjectsCollection != null &&
                        s.SolutionProjectsCollection.Count > 0)
                    {
                        foreach (M_ProjectClass p in s.SolutionProjectsCollection)
                        {
                            if (p.Equals(project))
                            {
                                this.currentSolution = s;
                                this.currentProject = p;
                                this.currentVersion = (this.currentProject==null)? null : this.currentProject.CurrentProjectVersion;

                                return;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取解决方案的递归父级对象
        /// </summary>
        /// <param name="solution"></param>
        private void GetSolutionParents(M_SolutionClass solution)
        {
            if (this.currentSolutionCollection != null &&
                this.currentSolutionCollection.SolutionsCollection != null &&
                this.currentSolutionCollection.SolutionsCollection.Count > 0)
            {
                foreach (M_SolutionClass s in this.currentSolutionCollection.SolutionsCollection)
                {
                    if (s.Equals(solution))
                    {
                        this.currentSolution = s;
                        this.currentProject = (this.currentSolution == null)? null : s.CurrentProject;
                        this.currentVersion = (this.currentProject == null) ? null : this.currentProject.CurrentProjectVersion;

                        return;
                    }
                }
            }
        }
        #endregion

        #region Fields
        private M_SolutionCollectionClass currentSolutionCollection;
        private M_SolutionClass currentSolution;
        private M_ProjectClass currentProject;
        private M_ProjectVersionClass currentVersion;
        private string savePath;
        #endregion        
    }
}
