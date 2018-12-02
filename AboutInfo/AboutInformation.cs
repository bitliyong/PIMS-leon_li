using System.Collections.ObjectModel;
using VersionManagement;
using System;
using FileOperation;
using System.Text;
namespace AboutInfo
{
    public class AboutInfomation
    {
        private string productName;
        private string copyright;
        private string author;
        private string description;
        private string contactInfo;

        private VersionMananger version;

        private ObservableCollection<VersionAbstract> historyInfoCollection;//历史版本信息

        public AboutInfomation()
        {
            this.historyInfoCollection = new ObservableCollection<VersionAbstract>();
        }

        public AboutInfomation(string productName, string copyright,
            string author, string description = "无", string contactInfo = "Email: bitliyong@163.com, Tel: 15618905473")
        {
            this.productName = productName;
            this.copyright = copyright;
            this.author = author;
            this.description = description;
            this.contactInfo = contactInfo;
            this.version = new VersionMananger(
                (DateTime.Now.Year.ToString()
                +DateTime.Now.Month.ToString()
                +DateTime.Now.Day.ToString()), 1, 0, 0, VersionMananger.GreekAlphabet.Beta);

            this.historyInfoCollection = new ObservableCollection<VersionAbstract>();
        }

        /// <summary>
        /// 更新版本信息
        /// </summary>
        /// <param name="degree"></param>
        /// <param name="newProductName"></param>
        /// <param name="newCopyRight"></param>
        /// <param name="newAuthor"></param>
        /// <param name="newDescription"></param>
        /// <param name="newContactInfo"></param>
        public void UpdateVersion(VersionMananger.UpdateDegree  degree, string newProductName = "",
            string newCopyRight = "", string newAuthor = "",
            string newDescription = "", string newContactInfo = "")
        {
            //需要更新版本
            if (this.description != null &&
                !this.description.Equals(newDescription))
            {
                //将当前的关于信息加入到历史信息集合中
                VersionAbstract a = new VersionAbstract();

                a.Version = this.version.GetVersionString();
                a.Description = this.description;

                this.historyInfoCollection.Add(a);

                //更新版本号
                this.version.UpdateVersion(degree);

                //更新程序描述信息
                if (!newProductName.Equals(""))
                {
                    this.productName = newProductName;
                }
                if (!newCopyRight.Equals(""))
                {
                    this.copyright = newCopyRight;
                }
                if (!newAuthor.Equals(""))
                {
                    this.author = newAuthor;
                }
                if (!newDescription.Equals(""))
                {
                    this.description = newDescription;
                }
                if (!newContactInfo.Equals(""))
                {
                    this.contactInfo = newContactInfo;
                }
            }
        }

        public override string ToString()
        {
            string _s = this.author + "#";
            _s += this.contactInfo + "#";
            _s += this.copyright + "#";
            _s += this.description + "#";
            _s += this.productName + "#";
            _s += this.version.ToString('*');

            //保存历史版本信息
            if(this.historyInfoCollection != null &&
                this.historyInfoCollection.Count > 0)
            {
                foreach (var h in this.historyInfoCollection)
                {
                    _s += "$" + h.ToString('*');
                }
            }

            return _s;
        }

        /// <summary>
        /// 将实例保存在指定的路径，并返回保存结果
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Save(string path)
        {
            return File_Operate.SaveToTextFileAndHideFile(path, this.ToString(), Encoding.Default);
        }

        /// <summary>
        /// 从指定路径中读取内容，设置本实例的各个字段
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool Load(string path)
        {
            string[] strs = File_Operate.LoadFromHiddenTextFile(path, Encoding.Default);

            bool res = false;

            if (strs != null)
            {
                string[] ss = strs[0].Split('$');

                string[] s = ss[0].Split('#');

                if (s.Length >= 6)
                {
                    this.author = s[0];
                    this.contactInfo = s[1];
                    this.copyright = s[2];
                    this.description = s[3];
                    this.productName = s[4];
                    res = VersionMananger.TryParse(s[5], '*', out this.version);
                }

                for (int i = 1; i < ss.Length; i++)
                {
                    s = ss[i].Split('*');

                    if (s.Length >= 2)
                    {
                        VersionAbstract a = new VersionAbstract();

                        a.Version = s[0];
                        a.Description = s[1];

                        this.historyInfoCollection.Add(a);
                    }
                }
            }

            return res;
        }

        #region
        public string ContactInfo
        {
            get
            {
                return contactInfo;
            }
            set
            {
                this.contactInfo = value;
            }
        }

        public string ProductName
        {
            get
            {
                return productName;
            }
            set
            {
                this.productName = value;
            }
        }

        public string ProductVersion
        {
            get
            {
                if (this.version != null)
                {
                    return this.version.GetVersionString();
                }
                else
                {
                    return "0.0.0";
                }
            }
        }

        public string Copyright
        {
            get
            {
                return copyright;
            }
            set
            {
                this.copyright = value;
            }
        }

        public string Author
        {
            get
            {
                return author;
            }
            set
            {
                this.author = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                this.description = value;
            }
        }

        public ObservableCollection<VersionAbstract> HistoryInfo
        {
            get
            {
                return this.historyInfoCollection;
            }
        }
        #endregion

        /// <summary>
        /// 获取当前对象的深拷贝
        /// </summary>
        /// <returns></returns>
        private AboutInfomation getDeepCopy()
        {
            AboutInfomation a = new AboutInfomation();

            a.author = this.author;
            a.contactInfo = this.contactInfo;
            a.copyright = this.copyright;
            a.description = this.description;
            a.productName = this.productName;
            a.version = this.version.GetDeepCopy();

            return a;
        }
    }
}
