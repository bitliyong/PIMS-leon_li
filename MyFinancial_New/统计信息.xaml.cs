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
using System.IO;
using System.Data;
using System.Collections.ObjectModel;
using FileOperation;

namespace MyFinancial
{
    partial class details
    {
        public string Name { get; set; }
        public string TIMESpan { get; set; }//时间跨度
        public double Target { get; set; }
        public double Current { get; set; }
        public double Degree { get; set; }
    }

    /// <summary>
    /// 统计信息.xaml 的交互逻辑
    /// </summary>
    public partial class 统计信息 : Window
    {
        private static string messagePath = "E:\\Micrsoft\\mg.msg";
        string[] source = null;
        string annualDetailsString;//年度收支情况详情
        List<string> filterParams = new List<string>();

        //创建一个数据表用于列表化显示年度计划完成情况
        DataTable dt = new DataTable();

        List<details> ld = new List<details>();

        //收支明细列表显示信息保存路径
        private string inOutDetSavePath;
        //收支明细列表显示信息数据源
        private ObservableCollection<IEBF> iebfCollection = new ObservableCollection<IEBF>();

        //信息数据缓冲字符串
        string messages = null;

        public 统计信息(string path, string detais)
        {
            messagePath = path + "\\logcat.log";
            annualDetailsString = detais;

            this.inOutDetSavePath = path + "\\inoutDet.sav";

            InitializeComponent();
        }

        /// <summary>
        /// 增加筛选项，只有加入筛选的string参数才会被添加到rtf
        /// </summary>
        /// <param name="filter"></param>
        private void AddFilter(params string[] filter)
        {
            foreach (string f in filter)
            {
                if (!filterParams.Contains(f))
                {
                    filterParams.Add(f);
                }
            }
        }
        /// <summary>
        /// 删除筛选项，这些筛选项对应的信息将不会被加入rtf
        /// </summary>
        /// <param name="filter"></param>
        private void DelFilter(params string[] filter)
        {
            foreach (string f in filter)
            {
                if (filterParams.Contains(f))
                {
                    filterParams.Remove(f);
                }
            }
        }

        /// <summary>
        /// 获取给定的rt中的所有非空白行，不包含换行符和回车符，返回结果存储在str
        /// </summary>
        /// <param name="str"></param>
        /// <param name="rt"></param>
        private void GetRTFTexts(ref string[] str, RichTextBox rt)
        {
            string tem;
            List<string> lstr = new List<string>();
            char[] sp = { '\r', '\n' };

            this.richTextBox1.Selection.Select(this.richTextBox1.Document.ContentStart,
                this.richTextBox1.Document.ContentEnd);
            tem = this.richTextBox1.Selection.Text;
            
            str = tem.Split(sp);
            foreach (string s in str)
            {
                if (!string.IsNullOrWhiteSpace(s))
                {
                    lstr.Add(s);
                }
            }

            str = lstr.ToArray();
        }

        /// <summary>
        /// 在src中找到所有包含str的字符串(不包含回车换行符)并返回一个包含这些字符串的数组
        /// </summary>
        /// <param name="src"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private string[] SerachInfos(string[] src, string[] str)
        {
            string[] res;
            List<string> lstr = new List<string>();

            foreach(string ft in str)
            {
                foreach (string s in src)
                {
                    if (s.Contains(ft))
                    {
                        lstr.Add(s);
                    }
                }
            }

            res = lstr.ToArray();

            return res;
        }

        /// <summary>
        /// 使用str刷新richtextbox的显示
        /// </summary>
        /// <param name="str"></param>
        private void RefreshRTF()
        {
            string[] str = null;

            str = SerachInfos(source, filterParams.ToArray());

            //清除当前显示
            this.richTextBox1.Selection.Select(this.richTextBox1.Document.ContentStart,
                this.richTextBox1.Document.ContentEnd);

            this.richTextBox1.Selection.Text = "";

            foreach (string s in str)
            {
                this.richTextBox1.AppendText(s);
                this.richTextBox1.AppendText("\n");//补充上回车换行符
            }
        }
        /// <summary>
        /// 载入RTF信息
        /// </summary>
        /// <param name="richTextBox"></param>
        private void LoadRTFInfos(RichTextBox richTextBox)
        {
            string[] ss = File_Encode_Operate.LoadFromHiddenTextFile(messagePath);

            if (ss != null &&
                ss.Length > 0)
            {
                messages = "";

                foreach(string s in ss)
                {
                    if (!string.IsNullOrWhiteSpace(s.Replace("\r\n", " ")))
                    {
                        messages += s;
                    }
                }

                richTextBox.AppendText(messages);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadRTFInfos(this.richTextBox1);
            GetRTFTexts(ref source, this.richTextBox1);

            //创建数据列表
            dt.Columns.Add("年度", typeof(string));
            dt.Columns.Add("目标", typeof(string));
            dt.Columns.Add("实现", typeof(string));
            dt.Columns.Add("完成度", typeof(string));

            this.annualGrid.ItemsSource = dt.DefaultView;

            if (!this.Load())
            {
                getInoutDetailsFromMessage();
            }

            this.inOutGrid.ItemsSource = this.iebfCollection;

            //滚动到最后
            this.richTextBox1.ScrollToEnd();

            //显示最新结果
            this.inOutGrid.SelectedIndex = this.iebfCollection.Count - 1;
            this.inOutGrid.ScrollIntoView(this.inOutGrid.Items[this.inOutGrid.SelectedIndex]);
        }

        /// <summary>
        /// 从保存的操作信息文件中解析出收支信息
        /// </summary>
        private void getInoutDetailsFromMessage()
        {
            if (!string.IsNullOrWhiteSpace(messages))
            {
                string[] msgs = messages.Replace("\r\n", "\r").Split("\r".ToArray(), StringSplitOptions.RemoveEmptyEntries);

                double totalIn = 0.0;
                double totalOut = 0.0;
                double salaryIn = 0.0;
                double profitIn = 0.0;
                double otherIn = 0.0;

                foreach (string _s in msgs)
                {
                    string s = _s;

                    if(string.IsNullOrWhiteSpace(s) != true)
                    {
                        while (s.Contains("  "))
                        {
                            s = s.Replace("  ", " ");
                        }

                        if (s.Contains("Error：") ||
                            s.Contains("Err"))
                            continue;
                        
                        if(s.Contains("收入") ||
                            s.Contains("支出"))
                        {
                            //string date = s.Substring(0, s.IndexOf(':'));
                            //string inOut = s.Contains("支出") ? "支出" : (s.Contains("工资收入") ? "工资收入" : (s.Contains("收益收入") ? "收益收入" : "其他收入"));
                            //int startOffset = inOut.Equals("支出") ? 2 : 4;
                            //string count = s.Substring(s.IndexOf(inOut) + startOffset, 
                            //    s.IndexOf("元") - s.IndexOf(inOut) - startOffset).Trim();
                            //string comment = s.Substring(s.IndexOf("详情：") + 3, s.Length - s.IndexOf("详情：") - 3);

                            /*
                             * 修改参数获取方式，原有方式过于简陋，无法应付用户不合法的输入信息。
                             */
                            try
                            {
                                string tmp = s;

                                string date = tmp.Substring(0, tmp.IndexOf(' ') - 1);
                                tmp = tmp.Substring(tmp.IndexOf(' ') + 1);

                                string account = tmp.Substring(0, tmp.IndexOf(' '));
                                tmp = tmp.Substring(tmp.IndexOf(' ') + 1);

                                string inOut = tmp.Substring(0, tmp.IndexOf(' '));
                                tmp = tmp.Substring(tmp.IndexOf(' ') + 1);

                                string count = tmp.Substring(0, tmp.IndexOf(' '));
                                tmp = tmp.Substring(tmp.IndexOf(' ') + 1);

                                string comment = tmp.Substring(tmp.IndexOf('：') + 1);

                                IEBF i = new IEBF();

                                i.Date = date;
                                i.Account = account;
                                i.InOut = inOut;
                                i.Count = Double.Parse(count);
                                i.Comment = comment;

                                /*
                                 * 信用卡的收支由于最终会体现为非信用卡账户的收支，所以信用卡的收支不计算在内
                                 */
                                if (i.Account.Contains("信用卡"))
                                    continue;

                                /*
                                 *分类统计求和
                                 *总收入（工资+收益+其他）
                                 *总支出
                                 */
                                if (i.InOut == "工资收入")
                                {
                                    salaryIn += i.Count;
                                }
                                else if (i.InOut == "收益收入")
                                {
                                    profitIn += i.Count;
                                }
                                else if (i.InOut == "其他收入")
                                {
                                    otherIn += i.Count;
                                }
                                else if (i.InOut == "支出")
                                {
                                    totalOut += i.Count;
                                }

                                this.iebfCollection.Add(i);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Exception Occured: " + ex.Message);
                            }
                        }
                    }
                }

                totalIn = salaryIn + profitIn + otherIn;

                this.totalInLb.Content = totalIn.ToString("F2");
                this.totalOutLb.Content = totalOut.ToString("F2");
                this.salaryInLb.Content = salaryIn.ToString("F2");
                this.profitInLb.Content = profitIn.ToString("F2");
                this.otherInLb.Content = otherIn.ToString("F2");
            }
        }
        #region
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (!this.Save())
                {
                    MessageBox.Show("统计信息：保存收支明细数据失败！");
                }
                
                this.Close();
            }
        }

#endregion

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tc = sender as TabControl;

            if (tc != null)
            {
                if (tc.SelectedItem.Equals(this.documentView))
                {
                    System.Windows.Documents.FlowDocument doc = annual.Document;
                    doc.Blocks.Clear();

                    this.annual.AppendText(annualDetailsString);
                }
                else if (tc.SelectedItem.Equals(this.listView))
                {
                    UpdateDataTable();
                }
                else if (tc.SelectedItem.Equals(this.inOutDetails))
                {
                    this.inOutGrid.ItemsSource = this.iebfCollection;
                    //MessageBox.Show("收支明细");
                }
            }
        }

        /// <summary>
        /// 刷新数据表中的数据
        /// </summary>
        private void UpdateDataTable()
        {
            //dt.Rows.Clear();
            ld.Clear();

            string[] _astring = annualDetailsString.Replace("\n\n", "*").Split('*');

            if (_astring != null)
            {
                foreach (var a in _astring)
                {
                    string[] _dstring = a.Split('\n');

                    if (_dstring.Length == 5)
                    {
                        //DataRow dr = dt.NewRow();

                        //dr["年度"] = _dstring[0].Replace("年度：", "");
                        //dr["目标"] = _dstring[1].Replace("目标：", "");
                        //dr["实现"] = _dstring[2].Replace("实现：", "");
                        //dr["完成度"] = _dstring[3].Replace("完成度：", "");

                        details d = new details();

                        d.Name = _dstring[0].Replace("年度：", "");
                        d.TIMESpan = _dstring[1].Replace("起止时间：", "");
                        d.Target = Double.Parse(_dstring[2].Replace("目标：", ""));
                        d.Current = Double.Parse(_dstring[3].Replace("实现：", ""));
                        d.Degree = Double.Parse(_dstring[4].Replace("完成度：", ""));

                        ld.Add(d);
                    }

                    //dt.Rows.Add(dr);
                }

                this.annualGrid.ItemsSource = ld;
            }
        }

        public override string ToString()
        {
            string s = null;

            if (this.iebfCollection.Count > 0)
            {
                foreach (var i in this.iebfCollection)
                {
                    s += i.ToString('*') + "#";
                }
            }

            return s;
            //return base.ToString();
        }

        private bool Save()
        {
            if (this.inOutDetSavePath != null)
            {
                return FileOperation.File_Encode_Operate.SaveToTextFileAndHideFile(this.inOutDetSavePath, this.ToString());
            }

            return false;
        }

        private bool Load()
        {
            /*
            if (this.inOutDetSavePath != null)
            {
                string[] strs = FileOperation.File_Encode_Operate.LoadFromHiddenTextFile(this.inOutDetSavePath);
                if (strs != null &&
                    strs.Length > 0)
                {
                    string[] _strs = strs[0].Split('#');

                    if (_strs.Length > 0)
                    {
                        foreach (string s in _strs)
                        {
                            IEBF obj;

                            if (IEBF.Load(s, '*', out obj))
                            {
                                this.iebfCollection.Add(obj);
                            }
                        }

                        if (this.iebfCollection.Count > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            */
            return false;
        }
    }

    /// <summary>
    /// 收支明细类
    /// </summary>
    internal class IEBF
    {
        /// <summary>
        /// 根据指定的分割字符c创建本实例的所有字段的字符串表示
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public string ToString(char c)
        {
            string s = this.Date + c;
            s += this.Account + c;
            s += this.InOut + c;
            s += this.Count + c;
            s += this.Comment;

            return s;
            //return base.ToString();
        }

        /// <summary>
        /// 根据指定的分割字符c将源字符串src分割，并用于初始化实例
        /// </summary>
        /// <param name="src"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static bool Load(string src, char c, out IEBF obj)
        {
            obj = new IEBF();

            if (src != null &&
                !string.IsNullOrWhiteSpace(src))
            {
                string[] strs = src.Split(c);

                if (strs.Length >= 5)
                {
                    obj.Date = strs[0];
                    obj.Account = strs[1];
                    obj.InOut = strs[2];
                    obj.Count = Double.Parse(strs[3]);
                    obj.Comment = strs[4];

                    return true;
                }
            }

            return false;
        }
        #region
        public string Date
        {
            get;
            set;
        }
        public string Account
        {
            get;
            set;
        }
        public string InOut
        {
            get;
            set;
        }
        public double Count
        {
            get;
            set;
        }
        public string Comment
        {
            get;
            set;
        }
        #endregion
    }
}
