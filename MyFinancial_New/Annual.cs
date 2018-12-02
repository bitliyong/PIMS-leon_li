using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows;
using FileOperation;
using System.IO;

namespace MyFinancial
{
    /// <summary>
    /// 年度类
    /// </summary>
    public class YEAR
    {
        private DateTime beginDate;
        private DateTime endDate;
        private string name;

        public YEAR()
        {
            beginDate = new DateTime(2015, 5, 1);
            endDate = new DateTime(2016, 4, 30);
            name = "2015";
        }

        public DateTime BeginDate
        {
            get { return beginDate; }
            set
            {
                beginDate = value;
            }
        }
        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                endDate = value;
            }
        }
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
            }
        }

        public string ToString()
        {
            return this.name + "#" 
                + this.beginDate.ToShortDateString() 
                + "#" + this.endDate.ToShortDateString();
        }

        internal YEAR GetDeepCopy()
        {
            YEAR y = new YEAR();

            y.beginDate = this.beginDate;
            y.endDate = this.endDate;
            y.name = this.name;

            return y;
        }

        public string TimeSpan 
        {
            get
            {
                return (this.BeginDate.ToString("yyyy.MM.dd") + "-"
                    + this.EndDate.ToString("yyyy.MM.dd"));
            }
        }
    }
    /// <summary>
    /// 年度计划类
    /// </summary>
    public class Annual : INotifyPropertyChanged
    {
        private double targetValue = 70000.0;
        private YEAR year;
        private double currentValue = 60000.0;
        //private List<Annual> historyRecordList;
        //private double accumulatedValue = 0.0;

        private List<KeyValuePair<Color, int>> colors = new List<KeyValuePair<Color, int>>
        {   new KeyValuePair<Color, int>(Colors.DarkRed, 0),
            new KeyValuePair<Color,int>(Colors.Red,1),
            new KeyValuePair<Color,int>(Colors.LightGreen,2),
            new KeyValuePair<Color,int>(Colors.Green,3)};

        #region
        /// <summary>
        /// 以往各个年度的累计值
        /// </summary>
        //public double AccumulatedValue
        //{
        //    get { return accumulatedValue; }
        //}
        public event PropertyChangedEventHandler PropertyChanged
             = (ss, ee) =>
                 {
                     //todo
                 };
        /// <summary>
        /// 年度目标值
        /// </summary>
        public double TargetValue
        {
            get { return this.targetValue; }
            set
            {
                targetValue = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("TargetValue"));
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CompleteDegree"));
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("DegreeVal"));
                }
            }
        }
        /// <summary>
        /// 年度
        /// </summary>
        public YEAR Year
        {
            get { return year; }
            set
            {
                year = value;
                if (this.PropertyChanged != null)
                {
                    year = value;

                    if (this.PropertyChanged != null)
                    {
                        this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Year"));
                    }
                }
            }
        }
        /// <summary>
        /// 当前值
        /// </summary>
        public double CurrentValue
        {
            get { return this.currentValue; }
            set
            {
                currentValue = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CurrentValue"));
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("CompleteDegree"));
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("DegreeVal"));
                }
            }
        }

        /// <summary>
        /// 获取当前完成度
        /// </summary>
        public string CompleteDegree
        {
            get
            {
                return (this.GetCompleteDegree()*100 + "%");
            }
        }

        /// <summary>
        /// 获取当前完成度
        /// </summary>
        public double DegreeVal
        {
            set
            {
                
            }
            get
            {
                return (this.GetCompleteDegree());
            }
        }

        public String TimeSpan
        {
            get { return this.Year.TimeSpan; }
        }
        #endregion
        public Annual()
        {
            //historyRecordList = new List<Annual>();
            year = new YEAR();
        }
        public Annual(YEAR year, double target, double current = 0.0)
        {
            this.year = year;
            this.targetValue = target;
            this.currentValue = current;
            //historyRecordList = new List<Annual>();
        }
        /// <summary>
        /// 设置年度（开始和结束日期，以及年度命名（默认为起始日期的年份）
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void SetYear(DateTime start, DateTime end, string name = null)
        {
            year.BeginDate = start;
            year.EndDate = end;
            year.Name = name == null ? start.Year.ToString() : name;
        }
        /// <summary>
        /// 设定年度目标值
        /// </summary>
        /// <param name="value"></param>
        public void SetTargetValue(double value)
        {
            targetValue = Math.Round(value, 2, MidpointRounding.ToEven);
        }
        /// <summary>
        /// 计算目标完成度
        /// </summary>
        /// <returns></returns>
        public double GetCompleteDegree()
        {
            return Math.Round(currentValue / targetValue, 4, MidpointRounding.ToEven);
        }
        /// <summary>
        /// 刷新控件前景色，以指示目标完成度
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        public bool UpdateForeground(Object control)
        {
            try
            {
                Color foreground = GetForeground(GetCompleteDegree());

                ProgressBar progressBar = control as ProgressBar;
                if (progressBar == null)
                {
                    Label label = control as Label;

                    if (label == null)
                        return false;

                    label.Foreground = new SolidColorBrush(foreground);
                }
                else
                {
                    progressBar.Foreground = new SolidColorBrush(foreground);
                }

                return true;
            }
            catch (InvalidCastException ie)
            {
                MessageBox.Show(ie.Message);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }
        /// <summary>
        /// 根据目标完成度改变前景色
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        private Color GetForeground(double percent)
        {
            if (percent < 0.5)
            {
                return colors.ElementAt(0).Key;
            }
            else if (percent < 0.75)
            {
                return colors.ElementAt(1).Key;
            }
            else if (percent < 1)
            {
                return colors.ElementAt(2).Key;
            }
            else
            {
                return colors.ElementAt(3).Key;
            }
        }
        /// <summary>
        /// 将过去的年度内容存入历史
        /// </summary>
        /// <param name="pastTarget"></param>
        /// <returns></returns>
        //public bool AddToHistory(Annual pastTarget)
        //{
        //    if (pastTarget != null)
        //    {
        //        this.historyRecordList.Add(pastTarget);
        //        this.accumulatedValue += pastTarget.currentValue;//将当前值加入累计值
        //        this.currentValue = 0.0;
        //        return true;
        //    }

        //    MessageBox.Show("存储值为空！");
        //    return false;
        //}
        /// <summary>
        /// 将存储的记录转换成字符串进行保存
        /// </summary>
        /// <returns></returns>
        //private string RecordToString()
        //{
        //    string rec = null;

        //    if (historyRecordList.Count > 0)
        //    {
        //        foreach (Annual his in historyRecordList)
        //        {
        //            rec += "##" + his.ToString();
        //        }
        //    }

        //    return rec;
        //}
        /// <summary>
        /// 日期是否在当前年度中（1-当前年度之后，-1-当前年度之前，0-在本年度中
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public int WithinCurrentYear(DateTime day)
        {
            //晚于当前年度最后时间
            if (day.CompareTo(year.EndDate) >= 0)
            {
                return 1;
            }
            //早于当前年度的最早时间
            else if (day.CompareTo(year.BeginDate) < 0)
            {
                return -1;
            }
            else
                return 0;
        }
        /// <summary>
        /// 转成格式化字符串
        /// </summary>
        /// <returns></returns>
        private string ToString()
        {
            return this.year.ToString() + "#" + this.targetValue.ToString("F2") + "#"
                + this.currentValue.ToString("F2");// + "#" +this.accumulatedValue.ToString("F2");
        }
        /// <summary>
        /// 保存信息
        /// </summary>
        /// <returns></returns>
        public static bool Save(string path, List<Annual> an)
        {
            try
            {
                string str = null;

                if(an != null &&
                    an.Count > 0)
                {
                    foreach (Annual a in an)
                    {
                        str += a.ToString() + "*";
                    }

                    File_Operate.SaveToTextFile(path, str);

                    return true;
                }

                return false;                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }
        /// <summary>
        /// 载入数据，设置对象失败返回false,对象将为null
        /// </summary>
        /// <returns></returns>
        public static bool Load(string path, out List<Annual> an)
        {
            try
            {
                an = null;

                //拆分为多个annual_target.toString()数据
                if (!File.Exists(path))
                {
                    return false;
                }

                string[] temp = File_Operate.LoadFromTextFile(path);

                if (temp == null)
                {
                    return false;
                }

                string[] infos = temp[0].Split("*".ToArray(), StringSplitOptions.RemoveEmptyEntries);

                if (infos.Length > 0)
                {
                    //创建对象
                    an = new List<Annual>();

                    for (int i = 0; i < infos.Length; i++)
                    {
                        Annual a = new Annual();

                        GetAnnualDetailsFromStrings(infos[i].Split('#'), a);

                        an.Add(a);
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                an = null;

                return false;
            }

            return false;
        }

        private static void GetAnnualDetailsFromStrings(string[] temp, Annual a)
        {
            DateTime date = new DateTime();

            a.year.Name = temp[0];

            DateTime.TryParse(temp[1], out date);
            a.year.BeginDate = date;

            DateTime.TryParse(temp[2], out date);
            a.year.EndDate = date;

            a.targetValue = double.Parse(temp[3]);

            a.currentValue = double.Parse(temp[4]);

            //a.accumulatedValue = double.Parse(temp[5]);
        }
        /// <summary>
        /// 更新一组控件的前景色
        /// </summary>
        /// <param name="controls"></param>
        internal bool UpdateForegrounds(object[] controls)
        {
            if (controls.Length > 0)
            {
                foreach (object control in controls)
                {
                    if (!UpdateForeground(control))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        internal Annual GetDeepCopy()
        {
            Annual a = new Annual();

            //a.accumulatedValue = this.accumulatedValue;
            a.currentValue = this.currentValue;
            a.targetValue = this.targetValue;
            a.year = this.year.GetDeepCopy();

            return a;
        }

        internal static string GetAnnualDetails(List<Annual> ans)
        {
            string s = "";

            if(ans != null &&
                ans.Count > 0)
            {
                foreach (Annual a in ans)
                {
                    s += annualDescription(a);
                }
            }
            return s;
        }

        private static string annualDescription(Annual a)
        {
            string s = null;

            if (a != null)
            {
                s += ("年度：" + a.Year.Name);
                s += ("\n起止时间：" + a.Year.TimeSpan);
                s += ("\n目标：" + a.TargetValue.ToString());
                s += ("\n实现：" + a.CurrentValue.ToString());
                s += ("\n完成度：" + a.GetCompleteDegree().ToString("F2"));
                s += "\n\n";
            }

            return s;
        }

        /// <summary>
        /// 设置年度
        /// </summary>
        /// <param name="year"></param>
        internal void SetYear(int year)
        {
            YEAR y = new YEAR();

            DateTime begin = new DateTime(year, 5, 1);
            DateTime end = new DateTime(year+1, 4, 30);

            y.BeginDate = begin;
            y.EndDate = end;

            y.Name = year.ToString();

            this.year = y;
        }
    }
}
