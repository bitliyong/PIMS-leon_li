using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyFinancial
{
    public enum State { open, close };

    public class WindowStates
    {
        public WindowStates()
        {
            this.state = 0;
        }

        /// <summary>
        /// 标记窗口打开状态
        /// </summary>
        public UInt32 state;
        public int index;
    }
}
