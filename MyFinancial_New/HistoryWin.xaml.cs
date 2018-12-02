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
    /// HistoryWin.xaml 的交互逻辑
    /// </summary>
    public partial class HistoryWin : Window
    {
        private string[] history;

        public HistoryWin()
        {
            InitializeComponent();
        }

        public HistoryWin(string[] historyStr)
        {
            InitializeComponent();

            this.history = historyStr;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(this.history != null && this.history.Length > 0)
            {
                foreach (string s in this.history)
                {
                    this.historyRtf.AppendText(s+"\r");
                }
            }
            
            this.historyRtf.ScrollToEnd();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
    }
}
