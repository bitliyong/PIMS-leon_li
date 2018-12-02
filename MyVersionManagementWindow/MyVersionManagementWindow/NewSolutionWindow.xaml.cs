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
using System.Collections.ObjectModel;

namespace MyVersionManagementWindowNamespace
{
    /// <summary>
    /// NewSolutionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NewSolutionWindow : Window
    {
        public PassValuesHandler PassValuesEvent;

        public NewSolutionWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.projectsCollection = new ObservableCollection<MyVersionManagementLib.M_ProjectClass>();

            this.projectsCollectionTv.ItemsSource = this.projectsCollection;
        }

        private void projectBtn_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;

            if (bt != null)
            {
                if (bt.Name.Equals(this.AddProjectBtn.Name))
                {
                    //todo
                    NewProjectWindow npw = new NewProjectWindow();
                    npw.PassValuesEvent +=
                        (ss, ee) =>
                        {
                            this.infoSbi.Content = ee.Description;

                            if (ee.Flag == true)
                            {
                                MyVersionManagementLib.M_ProjectClass project =
                                    ee.Result as MyVersionManagementLib.M_ProjectClass;

                                if (project != null)
                                {
                                    this.projectsCollection.Add(project);
                                }
                            }
                        };

                    npw.ShowDialog();
                }
                else if (bt.Name.Equals(this.removeProjectBtn.Name))
                {
                    //todo
                    if(this.projectsCollection != null &&
                        this.projectsCollection.Count > 0 &&
                        this.projectsCollectionTv.SelectedIndex >= 0)
                    {
                        this.projectsCollection.RemoveAt(this.projectsCollectionTv.SelectedIndex);
                    }
                }
                else if (bt.Name.Equals(this.okBtn.Name))
                {
                    MyVersionManagementLib.M_SolutionClass solution = null;
                    string name = this.solutoinTitleTb.Text;
                    string description = this.solutionDescriptionTb.Text;
                    bool flag;
                    string resDescription = "";

                    //create a new solution
                    if(string.IsNullOrWhiteSpace(name) ||
                        string.IsNullOrWhiteSpace(description))
                    {
                        flag = false;

                        resDescription = "Solution Name/Description is null or whitespace.";
                    }
                    else
                    {
                        solution = new MyVersionManagementLib.M_SolutionClass(name, description);

                        if(this.projectsCollection != null &&
                            this.projectsCollection.Count > 0)
                        {
                            foreach (MyVersionManagementLib.M_ProjectClass p in this.projectsCollection)
                            {
                                solution.SolutionProjectsCollection.Add(p);
                            }
                        }

                        flag = true;

                        resDescription = "Solution create successfully.";
                    }

                    PassValuesEventArgs args = new PassValuesEventArgs(solution, flag, resDescription);

                    PassValuesEvent(this, args);

                    this.Close();
                }
                else if (bt.Name.Equals(this.cancleBtn.Name))
                {
                    //
                    PassValuesEventArgs args = new PassValuesEventArgs(null, false, "Create solution canceled.");

                    PassValuesEvent(this, args);

                    this.Close();
                }
                else
                {
                    //
                }
            }
        }

        private ObservableCollection<MyVersionManagementLib.M_ProjectClass> projectsCollection;

        private void projectsCollectionTv_Selected(object sender, RoutedEventArgs e)
        {

        }

        
    }
}
