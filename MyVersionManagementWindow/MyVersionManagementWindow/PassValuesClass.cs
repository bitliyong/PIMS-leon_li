using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyVersionManagementWindowNamespace
{
    /// <summary>
    /// 传递值的参数类
    /// </summary>
    public class PassValuesEventArgs : EventArgs
    {
        //两个参数，一个表示结果，另一个表示该结果是否有效。
        public PassValuesEventArgs(object result, bool flag, string description)
        {
            this.result = result;
            this.flag = flag;
            this.description = description;
        }
        /// <summary>
        /// 获取结果
        /// </summary>
        public object Result
        {
            //set
            //{
            //    this.result = value;
            //}
            get
            {
                return this.result;
            }
        }
        /// <summary>
        /// 获取结果的有效性
        /// </summary>
        public bool Flag
        {
            get
            {
                return this.flag;
            }
        }
        /// <summary>
        /// 获取结果的详细描述信息
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }
        }


        private object result;
        private bool flag;
        private string description;
    }

    public delegate void PassValuesHandler (object sender, PassValuesEventArgs e);
}
