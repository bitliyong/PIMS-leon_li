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

namespace MyFinancial
{
    /// <summary>
    /// NewItem.xaml 的交互逻辑
    /// </summary>
    public partial class EditItem : Window
    {
        private List<Annual> annuals;
        private Annual targetAnnual;

        public EditItem()
        {
            InitializeComponent();
        }

        public EditItem(List<Annual> a)
        {
            // TODO: Complete member initialization
            this.annuals = a;

            InitializeComponent();
        }



        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.targetTb.Text))
            {
                double d;

                if(Double.TryParse(this.targetTb.Text, out d))
                {
                    this.targetAnnual.TargetValue = d;

                    if (Double.TryParse(this.initValTb.Text, out d))
                    {
                        this.targetAnnual.CurrentValue = d;

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("输入数值错误！");
                    }
                }
                else
                {
                    MessageBox.Show("输入数值错误！");
                }
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //加载现有的年度计划
            if(this.annuals != null &&
                this.annuals.Count > 0)
            {
                this.annualsCmb.Items.Clear();

                foreach (Annual a in annuals)
                {
                    this.annualsCmb.Items.Add(a.Year.Name);
                }

                this.annualsCmb.SelectedIndex = 0;
            }
        }

        private void annualsCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;

            if (cmb != null)
            {
                if (cmb.SelectedIndex != -1)
                {
                    targetAnnual = this.annuals[cmb.SelectedIndex];

                    //更新年度计划显示
                    this.targetTb.Text = targetAnnual.TargetValue.ToString("F2");
                    this.initValTb.Text = targetAnnual.CurrentValue.ToString("F2");
                }
            }
        }
    }
}