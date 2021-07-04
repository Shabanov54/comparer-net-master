using System;
using System.Collections.Generic;
using System.Text;
using ComparerNET.Models;

namespace ComparerNET
{
    public interface IFileContext
    {
        int MaxDifferense { get; }
        string[] GetFilesToCompare();
        string GetStandartFile();
        string GetHostId(string files);
        string[] GetFileRow(string fileName);
        void WriteCompareLog(string log);
        void WriteInfoLog(string log);
    }
}
