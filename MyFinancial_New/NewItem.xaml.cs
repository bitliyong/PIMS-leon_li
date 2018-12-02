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
    public partial class NewItem : Window
    {
        private AccountClass accout;

        public NewItem()
        {
            InitializeComponent();
        }

        public NewItem(AccountClass a)
        {
            // TODO: Complete member initialization
            this.accout = a;

            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(this.titleTb.Text))
            {
                this.accout.Title = this.titleTb.Text;

                double d;

                if(Double.TryParse(this.initValTb.Text, out d))
                {
                    this.accout.Description = this.TextBox_details.Text;

                    switch (this.typeCmb.Text)
                    {
                        case "收支":
                            this.accout.AccoutType1 = AccountClass.AccoutType.收支;
                            this.accout.InValue = d;
                            break;
                        case "信用卡":
                            this.accout.AccoutType1 = AccountClass.AccoutType.信用卡;
                            this.accout.OutValue = d;
                            break;
                        case "现金":
                            this.accout.AccoutType1 = AccountClass.AccoutType.现金;
                            this.accout.InValue = d;
                            break;
                        default:
                            this.accout.AccoutType1 = AccountClass.AccoutType.收支;
                            this.accout.InValue = d;
                            break;
                    }

                    this.Close();
                }
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}