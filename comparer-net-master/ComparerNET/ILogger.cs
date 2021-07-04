using System;
using System.Collections.Generic;
using System.Text;
using ComparerNET.Models;

namespace ComparerNET
{
    public interface ILogger
    {
        void LogInfo(List<LogItem> logItems);
        void LogCompare(List<CompareItem> compares);
    }
}
