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
    public partial class SetParams : Window
    {
        private AccountClass salaryAccount;
        private AccountClass otherAccount;
        private AccountClass totalOutAccount;
        private AccountClass totalAccount;

        public SetParams()
        {
            InitializeComponent();
        }

        public SetParams(AccountClass salary, AccountClass other, AccountClass totalOut, AccountClass total)
        {
            // TODO: Complete member initialization
            this.salaryAccount = salary;
            this.otherAccount = other;
            this.totalOutAccount = totalOut;
            this.totalAccount = total;

            InitializeComponent();
        }

        private void buttonClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null)
            {
                if (btn.Name.Equals(this.goBtn.Name))
                {
                    if (this.passwordTb.Password.Equals("20160427"))
                    {
                        this.passwordLb.Visibility = System.Windows.Visibility.Hidden;
                        this.passwordTb.Visibility = System.Windows.Visibility.Hidden;
                        this.goBtn.Visibility = System.Windows.Visibility.Hidden;

                        this.otherLb.Visibility = System.Windows.Visibility.Visible;
                        this.otherTb.Visibility = System.Windows.Visibility.Visible;
                        this.salaryLb.Visibility = System.Windows.Visibility.Visible;
                        this.salaryTb.Visibility = System.Windows.Visibility.Visible;
                        this.totalOutLb.Visibility = System.Windows.Visibility.Visible;
                        this.totalOutTb.Visibility = System.Windows.Visibility.Visible;
                        this.totalLb.Visibility = System.Windows.Visibility.Visible;
                        this.totalTb.Visibility = System.Windows.Visibility.Visible;
                        
                        this.setBtn.Visibility = System.Windows.Visibility.Visible;
                        this.cancelBtn.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                else if (btn.Name.Equals(this.setBtn.Name))
                {
                    double d;

                    if (this.salaryAccount != null)
                    {
                        if (Double.TryParse(this.salaryTb.Text, out d))
                        {
                            this.salaryAccount.SetInValue(d);
                        }
                    }
                    if (this.otherAccount != null)
                    {
                        if (Double.TryParse(this.otherTb.Text, out d))
                        {
                            this.otherAccount.SetInValue(d);
                        }
                    }
                    if (this.totalOutAccount != null)
                    {
                        if (Double.TryParse(this.totalOutTb.Text, out d))
                        {
                            this.totalOutAccount.SetInValue(d);
                        }
                    }
                    if (this.totalAccount != null)
                    {
                        if (Double.TryParse(this.totalTb.Text, out d))
                        {
                            this.totalAccount.SetInValue(d);
                        }
                    }

                    this.Close();
                }
                else if (btn.Name.Equals(this.cancelBtn.Name))
                {
                    this.Close();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.salaryAccount != null)
            {
                this.salaryTb.Text = this.salaryAccount.TotalValue.ToString("F2");
            }
            if (this.otherAccount != null)
            {
                this.otherTb.Text = this.otherAccount.TotalValue.ToString("F2");
            }
            if (this.totalOutAccount != null)
            {
                this.totalOutTb.Text = this.totalOutAccount.TotalValue.ToString("F2");
            }
            if (this.totalAccount != null)
            {
                this.totalTb.Text = this.totalAccount.TotalValue.ToString("F2");
            }
        }

        private void passwordTb_KeyDown(object sender, KeyEventArgs e)
        {
            //回车直接确定，触发goBtn按钮单击事件
            if (e.Key == Key.Enter)
            {
                buttonClick(this.goBtn, new RoutedEventArgs());
            }
        }
    }
}