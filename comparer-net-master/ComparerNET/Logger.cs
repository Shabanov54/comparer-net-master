using System;
using System.Collections.Generic;
using System.Text;
using ComparerNET.Models;

namespace ComparerNET
{
    public class Logger : ILogger
    {
        private IFileContext _fileContext;

        public Logger(IFileContext fileContext)
        {
            _fileContext = fileContext;
        }

        public void LogCompare(List<CompareItem> compares)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"Эталон;Сравниваемый;Код товара;Товар;Свойство;Строка в эталоне;Строка в проверяемом;Число в эталоне;Число в проверяемом;Дата в эталоне;Дата в проверяемом");
            foreach (var item in compares)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(item.HostId);
                stringBuilder.Append(";");
                stringBuilder.Append(item.HostStandartId);
                stringBuilder.Append(";");
                stringBuilder.Append(item.MedId);
                stringBuilder.Append(";");
                stringBuilder.Append(item.MedTitle);
                stringBuilder.Append(";");
                stringBuilder.Append(item.PropTitle);
                stringBuilder.Append(";");
                stringBuilder.Append(item.PropStandartStr);
                stringBuilder.Append(";");
                stringBuilder.Append(item.PropComparetStr);
                stringBuilder.Append(";");
                stringBuilder.Append(item.PropStandartVal);
                stringBuilder.Append(";");
                stringBuilder.Append(item.PropCompareVal);
                stringBuilder.Append(";");
                stringBuilder.Append(item.DateStandart);
                stringBuilder.Append(";");
                stringBuilder.Append(item.DateCompare);
            }
            string log = stringBuilder.ToString();

            _fileContext.WriteCompareLog(log);
        }

        public void LogInfo(List<LogItem> logItems)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Проведено сравнение файлов");
            foreach (var log in logItems)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(log.DateLog);
                stringBuilder.Append(" ");
                stringBuilder.Append(log.TimeLog);
                stringBuilder.Append(" ");
                stringBuilder.Append(log.TypeInfo);
                stringBuilder.Append(" ");
                stringBuilder.Append(log.Message);
            }
            string logs = stringBuilder.ToString();

            _fileContext.WriteInfoLog(logs);
        }

        
    }
}
