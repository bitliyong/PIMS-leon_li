using System.Windows;
using User;
using AboutInfo;
using VersionManagement;
using System.Collections.ObjectModel;
using System.IO;
using FileOperation;
using System.Windows.Controls;
using System.Windows.Data;
using System;
using About;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Text;

namespace MyFinancial
{
    /// <summary>
    /// Financial_New.xaml 的交互逻辑
    /// </summary>
    public partial class Financial_New : Window
    {
        public UserInformation loginedUser = new UserInformation();
        public WindowStates winSta = new WindowStates();

        //关于信息
        AboutInfomation aboutInfo = new AboutInfomation();

        private string aboutSavepath;

        //版本信息更新
        //使能update将会更新程序的版本号
        private bool update = true;
        private VersionMananger.UpdateDegree updateDegree = VersionMananger.UpdateDegree.Stage_Update;
        private string newAppName = "我的财务管家(new)";
        private string newAuthor = "李勇";
        private string newCopyright = "版权所有，翻版必究。Copyright@Bitliyong";
        /*
         * 新版本的描述，不能出现换行符！！！
         */
        //private string newDescription = "添加供反馈用的开发者邮箱，取消年度目标完成比例的上下限限制。";
        private string newDescription = "1. 修复当log信息中连续出现两个空格导致解析收支信息异常的bug; 2. 修复收支重复计算的bug";
        private string newContackInfo = "Email: bitliyong@163.com";

        //账户列表
        //显示在datagrid中的账户(收和支)
        private ObservableCollection<AccountClass> dynamicAccountCollection = new ObservableCollection<AccountClass>();
        //显示在label中的账户(单收或单支)
        private ObservableCollection<AccountClass> staticAccountCollection = new ObservableCollection<AccountClass>();

        //源账户列表
        //private ObservableCollection<AccountClass> srcAccountCollection = new ObservableCollection<AccountClass>();
        //目标账户列表
        private ObservableCollection<string> desAccountCollection = new ObservableCollection<string>();

        //转账源账户和转账目标账户
        private AccountClass srcAccount;
        private AccountClass desAccount;

        //当前所选择的账户
        private AccountClass currentAccount;

        //数据保存
        private string dataSavePath;
        //信息保存
        private string infoSavePath;

        //年度计划对象列表
        private List<Annual> annualTargetList;
        //当前的年度计划
        private Annual currentAnnualTarget;

        //年度计划保存路径
        private string annualSavePath;
        /// <summary>
        /// 更新并保存版本信息
        /// </summary>
        private void updateVersion()
        {
            //避免重复更新
            if (update == true &&
                aboutInfo != null &&
                !string.IsNullOrWhiteSpace(aboutInfo.Description) &&
                !aboutInfo.Description.Equals(this.newDescription))
            {
                aboutInfo.UpdateVersion(updateDegree, newAppName, newCopyright,
                    newAuthor, newDescription, newContackInfo);

                if (!aboutInfo.Save(this.aboutSavepath))
                {
                    MessageBox.Show("版本信息保存失败！");
                }
            }
        }

        public Financial_New(int index, UserInformation user)
        {
            this.winSta.index = index;

            loginedUser.GetValue(user);

            this.loginedUser.UserFolder += "\\financial";

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataSavePath = this.loginedUser.UserFolder + "\\accounts.acc";
            infoSavePath = this.loginedUser.UserFolder + "\\logcat.log";
            annualSavePath = this.loginedUser.UserFolder + "\\annuals.ann";

            //先创建好不存在的文件
            if (!File.Exists(dataSavePath))
            {
                File.Create(dataSavePath);
            }

            if (!File.Exists(infoSavePath))
            {
                File.Create(infoSavePath);
            }

            if (!File.Exists(annualSavePath))
            {
                File.Create(annualSavePath);
            }

            //为账户创建存储目录
            if (!Directory.Exists(loginedUser.UserFolder))
            {
                Directory.CreateDirectory(loginedUser.UserFolder);
            }
            //首先加载保存的信息
            LoadSaveDatas();

            //如果静态账户为空，创建4个静态账户
            if (this.staticAccountCollection.Count == 0)
            {
                //创建静态总额账户
                AccountClass totalAccount = new AccountClass("total", 0.0, "记录当前资金总额", AccountClass.AccoutType.总额);
                this.staticAccountCollection.Add(totalAccount);

                //创建静态工资账户
                AccountClass salaryAccount = new AccountClass("salary", 0.0, "记录工资收入情况", AccountClass.AccoutType.收);
                this.staticAccountCollection.Add(salaryAccount);

                //创建静态其他收入账户
                AccountClass anotherInAccount = new AccountClass("anotherIncome", 0.0, "记录其他收入情况", AccountClass.AccoutType.收);
                this.staticAccountCollection.Add(anotherInAccount);

                //创建静态支出账户
                AccountClass outlayAccount = new AccountClass("outlay", 0.0, "记录总支出情况", AccountClass.AccoutType.支);
                this.staticAccountCollection.Add(outlayAccount);
            }
            
            //设置绑定
            MySetContentBinding(this.staticAccountCollection[0], "TotalValue", this.totalValueLb);
            MySetContentBinding(this.staticAccountCollection[1], "TotalValue", this.salaryValueLb);
            MySetContentBinding(this.staticAccountCollection[2], "TotalValue", this.otherValueLb);
            MySetContentBinding(this.staticAccountCollection[3], "TotalValue", this.spendValueLb);

            //设置数据源
            this.accountsGrid.ItemsSource = this.dynamicAccountCollection;

            //设置当前选择第一个账户
            this.accountsGrid.SelectedIndex = 0;

            //设置当前选择的账户对象
            //currentAccount = this.dynamicAccountCollection[0];

            //当前账户不为空，绑定
            MySetContentBinding(this.currentAccount, "TotalValue", this.selectAccountTotalValueLb);
            
            //为源combobox控件添加items
            UpdateTransSrcItems();

            //this.srcAccountCmb.ItemsSource = this.dynamicAccountCollection;
            this.desAccountCmb.ItemsSource = this.desAccountCollection;

            //加载年度计划
            LoadAnnuals();

            //初始化关于信息加载与保存的参数
            this.aboutSavepath = this.loginedUser.UserFolder + "\\about.sav";

            if (!aboutInfo.Load(this.aboutSavepath))
            {
                aboutInfo = new AboutInfomation(this.newAppName, this.newCopyright, this.newAuthor,
                this.newDescription, this.newContackInfo);
            }
        }

        private void UpdateTransSrcItems()
        {
            this.srcAccountCmb.Items.Clear();

            if (this.dynamicAccountCollection.Count > 0)
            {
                foreach (AccountClass a in this.dynamicAccountCollection)
                {
                    //只有收支型账户能够进行转账操作
                    if (a.AccoutType1 == AccountClass.AccoutType.收支)
                    {
                        this.srcAccountCmb.Items.Add(a.Title);
                    }
                }
            }
        }

        /// <summary>
        /// 加载存储的参数值
        /// </summary>
        private void LoadSaveDatas()
        {
            string[] datas = File_Encode_Operate.LoadFromHiddenTextFile(dataSavePath);
            //读取文件成功，加载信息
            if (datas != null)
            {
                string[] fields = datas[0].Split("#".ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);

                if (fields != null &&
                    fields.Length > 0)
                {
                    //依次加载所有的对象
                    foreach (string s in fields)
                    {
                        if (!string.IsNullOrWhiteSpace(s))
                        {
                            AccountClass a;

                            //初始化对象
                            if (AccountClass.Load(s, out a))
                            {
                                //收支型和信用卡类型的账户放入datagrid中显示
                                if (a.AccoutType1 == AccountClass.AccoutType.收支 ||
                                    a.AccoutType1 == AccountClass.AccoutType.信用卡 ||
                                    a.AccoutType1 == AccountClass.AccoutType.现金)
                                {
                                    this.dynamicAccountCollection.Add(a);
                                }
                                //其他账户静态显示，不能由用户直接更改
                                else
                                {
                                    this.staticAccountCollection.Add(a);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void LoadAnnuals()
        {
            //加载年度计划列表失败,重新创建一个计划列表，并重新创建一个当前年度对应的计划对象
            if (!Annual.Load(this.annualSavePath, out this.annualTargetList))
            {
                //创建年度计划列表
                this.annualTargetList = new List<Annual>();
                //创建当前年度计划
                this.currentAnnualTarget = new Annual();

                this.currentAnnualTarget.TargetValue = 70000;
                this.currentAnnualTarget.CurrentValue = this.staticAccountCollection[0].TotalValue;
                this.currentAnnualTarget.SetYear(DateTime.Now.Year);

                this.annualTargetList.Add(this.currentAnnualTarget);
            }

            //找到当前的年度计划
            /*******************************************************************
             * 如果日期在5.1日（不含）之前，为上一年度，之后为本年度
             ******************************************************************/
            int year = (DateTime.Now.Month < 5) ? (DateTime.Now.Year - 1) : (DateTime.Now.Year);
            this.currentAnnualTarget = FindAnnualTargetByYear(year);

            //判断是否未找到年度计划，或者已进入新年度，如果进入新的年度则新建一个AnnualTarget实例作为当前年度。
            if (this.currentAnnualTarget == null ||
                this.currentAnnualTarget.WithinCurrentYear(DateTime.Today) == 1)
            {
                //创建当前年度计划
                this.currentAnnualTarget = new Annual();

                this.currentAnnualTarget.TargetValue = 70000;
                this.currentAnnualTarget.CurrentValue = 0;
                this.currentAnnualTarget.SetYear(DateTime.Now.Year);

                this.annualTargetList.Add(this.currentAnnualTarget);
            }

            //刷新界面显示
            //刷新年度计划信息
            this.currentAnnualTarget.UpdateForeground(this.CompletePercentBar);

            //设置与年度计划相关的绑定值
            SetAnnualBinding();
        }

        private void SetAnnualBinding()
        {
            MySetContentBinding(this.currentAnnualTarget, "TargetValue", this.manualValueLb);
            MySetContentBinding(this.currentAnnualTarget, "CurrentValue", this.currentValueLb);
            MySetContentBinding(this.currentAnnualTarget, "TimeSpan", this.timeSpanLb);
            MySetContentBinding(this.currentAnnualTarget, "CompleteDegree", this.comPercentLb);

            MySetValueBinding(this.currentAnnualTarget, "DegreeVal", this.CompletePercentBar);
        }

        private Annual FindAnnualTargetByYear(int year)
        {
            if (this.annualTargetList != null &&
                this.annualTargetList.Count >= 0)
            {
                foreach (Annual a in this.annualTargetList)
                {
                    //最后一天的年份与当前年份相同
                    if (a.Year.BeginDate.Year == year)
                    {
                        return a;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 设置Content绑定
        /// </summary>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        private void MySetContentBinding(object source, string property, Control label)
        {
            try
            {
                BindingOperations.ClearBinding(label, Label.ContentProperty);

                Binding binding = new Binding();
                binding.Source = source;
                binding.Path = new PropertyPath(property);
                //将数据源与目标连接在一起---其中把txtAge设置为绑定目标，并指定目标的属性为 
                //静态只读DependencyProperty类型的依赖属性TextBox.TextProperty.（运行是可以赋值的，注意与const的区别）
                //这类属性的值可以通过binding依赖在其他对象的属性值上，被其他对象的属性值所驱动。
                BindingOperations.SetBinding(label, Label.ContentProperty, binding);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// 设置Value绑定
        /// </summary>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="progressBar"></param>
        private void MySetValueBinding(object source, string property, ProgressBar progressBar)
        {
            try
            {
                BindingOperations.ClearBinding(progressBar, ProgressBar.ValueProperty);

                Binding binding = new Binding();
                binding.Source = source;
                binding.Path = new PropertyPath(property);
                //将数据源与目标连接在一起---其中把txtAge设置为绑定目标，并指定目标的属性为 
                //静态只读DependencyProperty类型的依赖属性TextBox.TextProperty.（运行是可以赋值的，注意与const的区别）
                //这类属性的值可以通过binding依赖在其他对象的属性值上，被其他对象的属性值所驱动。
                BindingOperations.SetBinding(progressBar, ProgressBar.ValueProperty, binding);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null)
            {
                double value;

                //输入了正确的数据值
                if(Double.TryParse(this.valueTb.Text, out value))
                {
                    //金额不能为负值和0
                    if (value <= 0)
                    {
                        return;
                    }

                    //普通收入
                    if (btn.Name.Equals(this.mInBtn.Name))
                    {
                        //收益收入
                        if (this.detailsTb.Text.Contains("收益"))
                        {
                            //显示操作结果
                            this.logcatRtb.AppendText(InExTransClass.Income(this.currentAccount, value,
                            this.datePicker1.SelectedDate.Value, InExTransClass.IncomeType.PROFIT, this.detailsTb.Text));
                        }
                        //其他收入
                        else
                        {
                            //显示操作结果
                            this.logcatRtb.AppendText(InExTransClass.Income(this.currentAccount, value,
                            this.datePicker1.SelectedDate.Value, InExTransClass.IncomeType.OTHER, this.detailsTb.Text));
                        }

                        //非信用卡账户的收支要记入总额账户,并更新对应的年度的收入情况
                        if (this.currentAccount.AccoutType1 != AccountClass.AccoutType.信用卡)
                        {
                            //现金账户其他收入不计入总额和年度计划，但是计入工资收入
                            if (this.currentAccount.AccoutType1 != AccountClass.AccoutType.现金)
                            {
                                //总额账户添加收入
                                this.staticAccountCollection[0].InValue = value;

                                //更新该年度的收入情况
                                this.currentAnnualTarget.CurrentValue += value;
                            }

                            this.staticAccountCollection[2].InValue = value;
                        }
                    }
                    //工资收入
                    else if (btn.Name.Equals(this.mSalaryBtn.Name))
                    {
                        //显示操作结果
                        this.logcatRtb.AppendText(InExTransClass.Income(this.currentAccount, value,
                        this.datePicker1.SelectedDate.Value, InExTransClass.IncomeType.SALARY, this.detailsTb.Text));

                        //非信用卡账户的收支要记入总额账户,工资收入要记入工资账户,更新该年度的收入情况
                        if (this.currentAccount.AccoutType1 != AccountClass.AccoutType.信用卡)
                        {
                            //现金账户工资收入不计入总额和年度计划，但是计入工资收入
                            if (this.currentAccount.AccoutType1 != AccountClass.AccoutType.现金)
                            {
                                //总额账户添加收入
                                this.staticAccountCollection[0].InValue = value;

                                //更新该年度的收入情况
                                this.currentAnnualTarget.CurrentValue += value;
                            }

                            this.staticAccountCollection[1].InValue = value;
                        }
                    }
                    //支出
                    else if (btn.Name.Equals(this.mOutBtn.Name))
                    {
                        bool res;

                        //显示操作结果
                        this.logcatRtb.AppendText(InExTransClass.Outcome(this.currentAccount, value,
                        this.datePicker1.SelectedDate.Value, this.detailsTb.Text, out res));

                        //非信用卡账户的收支要记入总额账户，支出要记入总支出账户,更新该账户的收支情况
                        if (this.currentAccount.AccoutType1 != AccountClass.AccoutType.信用卡)
                        {
                            //非信用卡账户的支出金额不能超过目标账户的总额
                            if (res == false)
                            {
                                return;
                            }

                            //总额账户减去支出
                            this.staticAccountCollection[0].OutValue = value;
                            this.staticAccountCollection[3].InValue = value;

                            //更新该年度的收入情况
                            this.currentAnnualTarget.CurrentValue -= value;
                        }
                    }
                    //转账
                    else if (btn.Name.Equals(this.mTransBtn.Name))
                    {
                        //显示操作结果
                        this.logcatRtb.AppendText(InExTransClass.Trans(this.srcAccount, this.desAccount, value,
                        this.datePicker1.SelectedDate.Value, this.detailsTb.Text));
                    }
                }

                
            }
        }

        private void comboBox_DropDownClosed(object sender, EventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            if (cb != null)
            {
                //转账源账户
                if (cb.Name.Equals(this.srcAccountCmb.Name))
                {
                    if (cb.SelectedIndex >= 0)
                    {
                        //当前的源账户
                        //srcAccount = this.dynamicAccountCollection[cb.SelectedIndex];
                        srcAccount = FindItemByTitle(this.dynamicAccountCollection ,this.srcAccountCmb.Text);

                        if (srcAccount != null)
                        {
                            //目标账户不能与源账户相同,将该账户从目标账户中移出
                            //先清空现有账户列表
                            this.desAccountCollection.Clear();
                            //禁用转账按钮
                            this.mTransBtn.IsEnabled = false;

                            //将不同与源账户的收支型账户对象加入到目标账户集合中
                            foreach (string title in this.srcAccountCmb.Items)
                            {
                                if (!title.Equals(this.srcAccountCmb.Text))
                                {
                                    this.desAccountCollection.Add(title);
                                }
                            }
                            //foreach (AccountClass a in this.dynamicAccountCollection)
                            //{
                            //    if (!a.Equals(srcAccount) &&
                            //        a.AccoutType1 == AccountClass.AccoutType.收支)
                            //    {
                            //        this.desAccountCollection.Add(a.Title);
                            //    }
                            //}
                        }
                    }
                }
                //目标账户选择改变
                else if (cb.Name.Equals(this.desAccountCmb.Name))
                {
                    //选择了合适的项
                    if (cb.SelectedIndex != -1)
                    {
                        this.desAccount = FindItemByTitle(this.dynamicAccountCollection, cb.Text);

                        if (this.desAccount != null)
                        {
                            this.mTransBtn.IsEnabled = true;
                        }
                    }
                    else
                    {
                        this.mTransBtn.IsEnabled = false;
                    }
                }
            }
        }

        /// <summary>
        /// 找到指定标题的账户对象
        /// </summary>
        /// <param name="observableCollection"></param>
        /// <param name="p"></param>
        /// <returns></returns>
        private AccountClass FindItemByTitle(ObservableCollection<AccountClass> observableCollection, string p)
        {
            if(observableCollection != null &&
                observableCollection.Count > 0)
            {
                foreach (AccountClass a in observableCollection)
                {
                    if (a.Title.Equals(p))
                    {
                        return a;
                    }
                } 
            }

            return null;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                //Save();
                this.Hide();
            }
        }

        private void FileItemClicked(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            if (mi != null)
            {
                //退出，隐藏窗口
                if (mi.Header.Equals("_Exit"))
                {
                    //Save();
                    //隐藏窗口
                    this.Hide();
                }
                //新建账户
                else if (mi.Header.Equals("_New Account"))
                {
                    AccountClass a = new AccountClass();

                    NewItem ni = new NewItem(a);

                    ni.ShowDialog();

                    //如果成功创建了账户
                    if (!string.IsNullOrWhiteSpace(a.Title))
                    {
                        //更新动态账户集合
                        this.dynamicAccountCollection.Add(a);

                        //如果新建账户不是信用卡和现金账户，且初始值不为0，则需要更新总额,并更新转账源列表
                        if (a.AccoutType1 != AccountClass.AccoutType.信用卡 &&
                            a.AccoutType1 != AccountClass.AccoutType.现金 &&
                            a.TotalValue > 0)
                        {
                            this.staticAccountCollection[0].InValue = a.TotalValue;

                            //更新当前对应的年度计划的数据
                            this.currentAnnualTarget.CurrentValue += a.TotalValue;

                            //更新转账源账户列表
                            UpdateTransSrcItems();
                        }

                        //输出创建账户的信息
                        string s = this.datePicker1.Text + ": 新建账户" + a.Title;
                        s += "，账户类别：" + a.AccoutType1.ToString();
                        s += "，账户余额：" + a.TotalValue.ToString("F2");
                        s += " 元\r";

                        this.logcatRtb.AppendText(s);
                    }
                }
                //编辑年度计划
                else if (mi.Header.Equals("_Edit Annual"))
                {
                    EditItem editor = new EditItem(this.annualTargetList);

                    editor.ShowDialog();

                    //更新显示
                    //刷新年度计划信息
                    this.currentAnnualTarget.UpdateForeground(this.CompletePercentBar);
                }
            }
        }

        private void OptionClicked(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            if (mi != null)
            {
                if (mi.Header.Equals("_Details"))
                {
                    try
                    {
                        string details = Annual.GetAnnualDetails(this.annualTargetList);

                        统计信息 info = new 统计信息(loginedUser.UserFolder, details);

                        info.ShowDialog();
                    }
                    catch (InvalidOperationException ioe)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                }
                else if (mi.Header.Equals("_Developer Options"))
                {
                    SetParams set = new SetParams(this.staticAccountCollection[1], 
                        this.staticAccountCollection[2], 
                        this.staticAccountCollection[3],
                        this.staticAccountCollection[0]);

                    set.ShowDialog();
                }
                else if (mi.Header.Equals("_ScrollBack"))
                {
                    /*SetParams set = new SetParams(this.staticAccountCollection[1], 
                        this.staticAccountCollection[2], 
                        this.staticAccountCollection[3]);

                    set.ShowDialog();*/
                    MessageBox.Show("未找到备份文件");
                }
                else if (mi.Header.Equals("_Import History Record"))
                {
                    ImportOpMsgs();
                }
            }
        }

        private void ImportOpMsgs()
        {
            Import.Import import = new Import.Import();
            Import.Filter f = new Import.Filter();

            import.filters.Clear();

            //f.msg = "账户余额信息（*.mon）";
            //f.ftr = "*.mon";
            //import.AddFilter(f);

            f.msg = "操作日志文件（*.msg）";
            f.ftr = "*.msg";
            import.AddFilter(f);

            import.extension = ".msg";

            import.fileNames.Clear();
            //import.fileNames.Add("m.mon");
            //import.fileNames.Add("mg.msg");

            //import.fileNames.Add("logcat.log");

            //import.destination = loginedUser.UserFolder;

            import.SelectItem();

            string[] dess = import.source.ToArray();
            //string[] srcs = import.source.ToArray();
            string des;
            string[] src;

            if (dess != null && dess.Length > 0)
            {
                des = dess[0];
                src = File_Operate.LoadFromHiddenTextFile(des, Encoding.Default);

                HistoryWin h = new HistoryWin(src);

                h.ShowDialog();
            }
            else
            {
                MessageBox.Show("未能找到匹配项，无法导入！");
            }

            //for (int index = 0; index < dess.Length; index++)
            //{
            //    des = import.destination + "\\" + dess[index];
            //    src = import.GetCorrespondingItem("*.msg");

            //    if (src != null)
            //    {
            //        import.ImportItem(des, src, Import.Import.ImportOptions.APPENDSTART);
            //    }
            //    else
            //    {
            //        MessageBox.Show("未能找到匹配项，无法导入！");
            //        break;//选择文件格式不正确
            //    }
            //}
        }

        private void HelpClicked(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            if (mi != null)
            {
                //关于窗口
                if (mi.Header.Equals("_About"))
                {
                    AboutWindow a = new AboutWindow(aboutInfo);

                    a.ShowDialog();
                }
                else if (mi.Header.Equals("_FeedBack"))
                {
                    MessageBox.Show(aboutInfo.ContactInfo, "Information",
                         MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK,
                         MessageBoxOptions.DefaultDesktopOnly);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //如果此窗口已经加载，保存可能修改的数据，否则直接关闭窗口，不保存数据。
            if (this.IsLoaded)
            {
                Save();
            }
        }

        private void Save()
        {
            //获取要保存的参数
            string content = "";

            //保存静态账户数据
            if (this.staticAccountCollection.Count > 0)
            {
                foreach (AccountClass a in this.staticAccountCollection)
                {
                    content += a.ToString() + "#";
                }
            }
            //保存动态账户数据
            if (this.dynamicAccountCollection.Count > 0)
            {
                foreach (AccountClass a in this.dynamicAccountCollection)
                {
                    content += a.ToString() + "#";
                }
            }

            //保存参数
            if (!File_Encode_Operate.SaveToTextFileAndHideFile(this.dataSavePath, content))
            {
                //参数保存失败
                MessageBox.Show("参数保存失败！");
            }

            //保存年度计划
            if (this.annualTargetList.Count > 0)
            {
                if (!Annual.Save(this.annualSavePath, this.annualTargetList))
                {
                    //参数保存失败
                    MessageBox.Show("参数保存失败！");
                }
            }

            //保存logcat信息
            SaveLogcatMessage(this.infoSavePath, this.logcatRtb);

            //根据设定更新版本信息
            updateVersion();
        }

        /// <summary>
        /// 保存操作信息
        /// </summary>
        /// <param name="richTextBox"></param>
        /// <returns></returns>
        public bool SaveLogcatMessage(string messagePath, RichTextBox richTextBox)
        {
            try
            {
                if (!File.Exists(messagePath))
                {
                    File.Create(messagePath);
                }

                TextRange documentTextRange = new TextRange(richTextBox.Document.ContentStart,
                    richTextBox.Document.ContentEnd);

                if (!string.IsNullOrWhiteSpace(documentTextRange.Text))
                {
                    if (!File_Encode_Operate.AppendToTextFileAndHideFile(messagePath, documentTextRange.Text))
                    {
                        MessageBox.Show("保存数据失败！");
                    }
                }

                return true;
            }
            catch (DirectoryNotFoundException de)
            {
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void accountsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //设置当前选择账户对象，并刷新显示
            AccountClass a = this.accountsGrid.SelectedItem as AccountClass;

            if (a != null)
            {
                this.currentAccount = a;

                //信用卡账户不能有工资收入
                if (this.currentAccount.AccoutType1 == AccountClass.AccoutType.收支)
                {
                    this.mSalaryBtn.IsEnabled = true;
                    this.mOutBtn.IsEnabled = true;
                    this.mInBtn.IsEnabled = true;
                }
                //现金账户不能支出(只作为统计使用)
                else if (this.currentAccount.AccoutType1 == AccountClass.AccoutType.现金)
                {
                    this.mSalaryBtn.IsEnabled = true;
                    this.mOutBtn.IsEnabled = false;
                    this.mInBtn.IsEnabled = true;
                }
                else
                {
                    this.mSalaryBtn.IsEnabled = false;
                    this.mOutBtn.IsEnabled = true;
                    this.mInBtn.IsEnabled = true;
                }

                //刷新显示信息
                this.selectAccountLb.Content = a.Title;
                this.selectAccountTypeLb.Content = a.AccoutType1;

                //当前账户已经改变，重新绑定
                MySetContentBinding(this.currentAccount, "TotalValue", this.selectAccountTotalValueLb);
            }
        }


        /// <summary>
        /// 文本内容改变，滚动到底部显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logcatRtb_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((RichTextBox)sender).ScrollToEnd();
        }

        /// <summary>
        /// 选择日期，
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void datePicker1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker dg = sender as DatePicker;

            if(dg != null &&
                dg.SelectedDate != null)
            {
                //加载对应的年度计划
                DateTime select = dg.SelectedDate.Value;
                int year = select.Year;

                //看所选日期在哪一年度
                //4.30日之前按前一年算
                if(select.CompareTo(new DateTime(year, 4, 30)) <= 0)
                //if (select.Month < 5 &&
                //        select.Day < 30)
                {
                    year = select.Year - 1;
                }
                
                this.currentAnnualTarget = FindAnnualTargetByYear(year);

                //如果未找到该年度计划，新建一个
                if (this.currentAnnualTarget == null)
                {
                    this.currentAnnualTarget = new Annual();

                    this.currentAnnualTarget.TargetValue = 85000.00;
                    this.currentAnnualTarget.CurrentValue = 0.00;
                    this.currentAnnualTarget.SetYear(year);

                    //添加到年度计划列表中
                    if (this.annualTargetList != null)
                    {
                        this.annualTargetList.Add(this.currentAnnualTarget);
                    }
                }

                //刷新界面显示
                //刷新年度计划信息
                this.currentAnnualTarget.UpdateForeground(this.CompletePercentBar);

                //重新设置与年度计划相关的绑定值
                SetAnnualBinding();
            }
            
        }

        /// <summary>
        /// 上下文菜单单击事件句柄
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;

            if (mi != null)
            {
                if (mi.Header.Equals(this.deleteItemMi.Header))
                {
                    AccountClass a = this.accountsGrid.SelectedItem as AccountClass;

                    if (a != null)
                    {
                        //删除选择项
                        this.dynamicAccountCollection.Remove(a);

                        this.logcatRtb.AppendText(CreateLogCatInfo("删除账户" + a.Title));
                    }
                }
            }
        }

        private string CreateLogCatInfo(string originalInfo)
        {
            string msg = DateTime.Now.ToString("yyyy-MM-dd") + ": ";

            return msg + originalInfo + "\r";
        }

        
    }
}
