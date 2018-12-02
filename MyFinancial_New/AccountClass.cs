using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MyFinancial
{
    /// <summary>
    /// 账户信息类，用于记录各个账户的收支等信息
    /// </summary>
    public class AccountClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (ss, ee) =>
        {

        };

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public AccountClass()
        {
            this.title = "";
            this.description = "";
            this.totalValue = 0;
            this.inValue = 0;
            this.outValue = 0;
            this.accoutType = AccoutType.收支;
        }

        /// <summary>
        /// 使用指定值创建账户
        /// </summary>
        /// <param name="title"></param>
        /// <param name="initValue"></param>
        /// <param name="des"></param>
        public AccountClass(string title, double initValue, string des="", AccoutType type = AccoutType.收支)
        {
            this.title = title;
            this.description = des;
            this.totalValue = initValue;
            this.inValue = 0;
            this.outValue = 0;
            this.accoutType = type;
        }

        /// <summary>
        /// 输出对象的字符串形式表示
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string s = this.title;

            s += "*" + this.totalValue.ToString("F2");
            s += "*" + this.inValue.ToString("F2");
            s += "*" + this.outValue.ToString("F2");
            s += "*" + this.accoutType.ToString();
            s += "*" + this.description;
            
            return s;
        }
        
        /// <summary>
        /// 加载对象值
        /// </summary>
        /// <param name="info"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static bool Load(string info, out AccountClass a)
        {
            a = new AccountClass();

            if (!string.IsNullOrWhiteSpace(info))
            {
                string[] infos = info.Split("*".ToArray(),  StringSplitOptions.RemoveEmptyEntries);

                if (infos.Length >= 6)
                {
                    a.title = infos[0];
                    if (Double.TryParse(infos[1], out a.totalValue))
                    {
                        if (Double.TryParse(infos[2], out a.inValue))
                        {
                            if (Double.TryParse(infos[3], out a.outValue))
                            {
                                if (Enum.TryParse(infos[4], out a.accoutType))
                                {
                                    a.description = infos[5];

                                    return true;
                                }
                                
                            }
                        }
                    }
                }
            }

            return false;
        }
        
        //公共属性
        #region
        /// <summary>
        /// 账户标题
        /// </summary>
        public string Title
        {
            set
            {
                this.title = value;
            }
            get
            {
                return this.title;
            }
        }
        /// <summary>
        /// 账户描述
        /// </summary>
        public string Description
        {
            set
            {
                this.description = value;
            }
            get
            {
                return this.description;
            }
        }
        /// <summary>
        /// 账户总额(只读)
        /// </summary>
        public double TotalValue
        {
            private set
            {
                this.totalValue = value;

                //产生事件
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("TotalValue"));
                }
            }
            get
            {
                return this.totalValue;
            }
        }
        /// <summary>
        /// 账户收入
        /// </summary>
        public double InValue
        {
            set
            {
                this.inValue += value;

                //信用卡账户的收入与支出，和总额的关系与其他类型相反
                if (this.AccoutType1 != AccoutType.信用卡)
                {
                    //同步更新总额
                    this.TotalValue += value;
                }
                else
                {
                    //同步更新总额
                    this.TotalValue -= value;
                }

                //产生事件
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("InValue"));
                }
            }
            get
            {
                return this.inValue;
            }
        }
        /// <summary>
        /// 账户支出
        /// </summary>
        public double OutValue
        {
            set
            {
                this.outValue += value;

                //信用卡账户的收入与支出，和总额的关系与其他类型相反
                if (this.AccoutType1 != AccoutType.信用卡)
                {
                    //同步更新总额
                    this.TotalValue -= value;
                }
                else
                {
                    //同步更新总额
                    this.TotalValue += value;
                }

                //产生事件
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("OutValue"));
                }
            }
            get
            {
                return this.outValue;
            }
        }

        /// <summary>
        /// 账户类型
        /// </summary>
        public AccoutType AccoutType1
        {
            get { return accoutType; }
            set 
            { 
                accoutType = value;

                //产生事件
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("AccoutType1"));
                }
            }
        }
        #endregion

        public void SetInValue(double d)
        {
            this.totalValue = d;

            //产生事件
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("TotalValue"));
            }
        }

        public enum AccoutType { 收, 支, 收支, 总额, 信用卡, 现金};

        private string title;//账户标题
        private double totalValue;//账户总额
        private double inValue;//账户收入
        private double outValue;//账户支出
        private string description;//账户描述
        private AccoutType accoutType;//账户类型

        
    }

    /// <summary>
    /// 收入/支出/转账类
    /// </summary>
    class InExTransClass
    {
        public enum IncomeType{SALARY, PROFIT, OTHER};
        
        /// <summary>
        /// 收入方法，更新指定账户并返回操作日志信息
        /// </summary>
        /// <param name="targetAccount"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <param name="type"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static string Income(AccountClass targetAccount, double value, DateTime time, IncomeType type, string details)
        {
            string msg = time.ToString("yyyy-MM-dd") + ": ";

            //纯输出账户不能有收入
            if (targetAccount.AccoutType1 != AccountClass.AccoutType.支)
            {
                targetAccount.InValue = value;

                msg += targetAccount.Title;

                //工资收入
                if (type == IncomeType.SALARY)
                {
                    msg += " 工资收入";
                }
                //收益收入
                else if (type == IncomeType.PROFIT)
                {
                    msg += " 收益收入";
                }
                //其他收入
                else
                {
                    msg += " 其他收入";
                }

                //金额
                msg += " " + value + " 元，";

                //备注信息
                msg += "详情：" + details;
            }
            else
            {
                msg += "Err: 本账户不能有收入！";
            }
            
            return (msg+"\r");
        }

        /// <summary>
        /// 支出方法，更新指定账户并返回操作日志信息
        /// </summary>
        /// <param name="targetAccount"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static string Outcome(AccountClass targetAccount, double value, DateTime time, 
            string details, out bool res)
        {
            string msg = time.ToString("yyyy-MM-dd") + ": ";

            //纯收入账户不能有支出
            if (targetAccount.AccoutType1 != AccountClass.AccoutType.收)
            {
                //非信用卡账户的支出金额不能超过目标账户的总额
                if (targetAccount.AccoutType1 != AccountClass.AccoutType.信用卡 &&
                    targetAccount.TotalValue < value)
                {
                    msg += "Err：支出金额不能超过目标账户总金额！";

                    res = false;
                }
                else
                {
                    targetAccount.OutValue = value;

                    msg += targetAccount.Title;

                    //支出
                    msg += " 支出";

                    //金额
                    msg += " " + value + " 元。";

                    //备注信息
                    msg += "详情：" + details;

                    res = true;
                }
            }
            else
            {
                msg += "Err: 本账户不能有支出！";

                res = false;
            }

            return (msg + "\r");
        }

        /// <summary>
        /// 从srcAccount转账至desAccount并返回操作日志信息
        /// </summary>
        /// <param name="srcAccount"></param>
        /// <param name="desAccount"></param>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public static string Trans(AccountClass srcAccount, AccountClass desAccount, double value, DateTime time, string details = "")
        {
            string msg = time.ToString("yyyy-MM-dd") + ": ";

            //纯收入账户不能有支出
            if (srcAccount.AccoutType1 == AccountClass.AccoutType.收)
            {
                msg += "Err: " + srcAccount.Title + " 不能有支出！";
            }
            //纯支出账户不能有收入
            else if(desAccount.AccoutType1 == AccountClass.AccoutType.支)
            {
                msg += "Err: " + desAccount.Title + " 不能有收入！";
            }
            else
            {
                //支出金额不能超过目标账户的总额
                if (srcAccount.TotalValue < value)
                {
                    msg += "Err：支出金额不能超过目标账户总金额！";
                }
                else
                {
                    srcAccount.OutValue = value;
                    desAccount.InValue = value;

                    msg += srcAccount.Title;

                    //转账
                    msg += " 转账至 ";

                    msg += desAccount.Title;

                    //金额
                    msg += " " + value + " 元，详情：";

                    //备注详情
                    msg += details;
                }
            }

            return (msg+"\r");
        }
    }
}
