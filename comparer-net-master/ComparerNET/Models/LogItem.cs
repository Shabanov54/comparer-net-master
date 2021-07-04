using System;
using System.Collections.Generic;
using System.Text;

namespace ComparerNET.Models
{
    public class LogItem
    {
        public LogItem(string typeInfo)
        {
            DateLog = DateTime.Now.ToShortDateString();
            TimeLog = DateTime.Now.ToShortTimeString();
            TypeInfo = typeInfo;
        }
        public string DateLog { get; }
        public string TimeLog { get; }
        public string TypeInfo { get; }
        public string Message { get; set; }
    }
}
