using ComparerNET.Exceptions;
using ComparerNET.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ComparerNET
{
    public class Compare
    {
        private readonly ILogger _logger;
        private readonly IFileContext _fileContext;
        private List<LogItem> _logs;
        private LogItem _logItem = null;
        private int _parsingError=0;

        public Compare()
        {
            _fileContext = new FileContext();
            _logger = new Logger(_fileContext);
            _logs = new List<LogItem>();
        }

        public void Run()
        {
            string[] filesToCompare = _fileContext.GetFilesToCompare();
            if (filesToCompare == null)
            {
                Console.WriteLine("Отсутствуют файлы для сравнения");
                Console.ReadKey();
                return;
            }
            string standartFile = _fileContext.GetStandartFile();

            try
            {
                CompareFiles(filesToCompare, standartFile);
            }
            catch (Exception)
            {
                Console.WriteLine("Хрясь");
                Console.ReadKey();
            }
        }

        private void CompareFiles(string[] filesToCompare, string standartFile)
        {
            List<CompareItem> compares = new List<CompareItem>();

            string[] standartFileRow=null;
            try
            {
                standartFileRow = _fileContext.GetFileRow(standartFile);
            }
            catch (OldFileException)
            {
                var message = "Файл сравнения устарел! Сравнение отменено!";
                _logItem = new LogItem("критическая ошибка")
                {
                    Message = message
                };
                _logs.Add(_logItem);
                _logger.LogInfo(_logs);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(message);
                Console.ReadKey();
                throw new Exception();
            }

            string standartHostId = _fileContext.GetHostId(standartFile);

            foreach (var file in filesToCompare)
            {
                if (file == standartFile)
                {
                    continue;
                }

                string hostId = _fileContext.GetHostId(file);

                List<CompareItem> compareItems= new List<CompareItem>();

                try
                {
                    string[] comparerFileRow = _fileContext.GetFileRow(file);
                    compareItems = GetCompares(standartFileRow, comparerFileRow, hostId, standartHostId);
                    _logItem = new LogItem(" инфо ")
                    {
                        Message = $"При сравнении хоста {hostId} произошло {_parsingError} ошибок при парсинге файла, получено {compareItems.Count} расхождений"
                    };
                }
                catch (OldFileException)
                {
                    _logItem = new LogItem("ошибка")
                    {
                        Message = $"При сравнении хоста {hostId} обнаружено что файл с данными устарел"
                    };
                }
                catch (OverflowMaxDiffExeption ex)
                {
                    _logItem = new LogItem("ошибка")
                    {
                        Message = ex.Message
                    };
                }
                catch (Exception)
                {
                    _logItem = new LogItem("ошибка")
                    {
                        Message = $"При сравнении хоста {hostId} произошла неизвестная ошибка, детализация расхождений отменена"
                    };
                }

                compares.AddRange(compareItems);
                _logs.Add(_logItem);
                _logItem = null;
                _parsingError = 0;
                Console.WriteLine($"{DateTime.Now.ToShortTimeString()} Файл хоста {hostId} обработан");
            }

            _logger.LogCompare(compares);

            _logger.LogInfo(_logs);
        }

        private List<CompareItem> GetCompares(string[] standartFileRow, string[] comparerFileRow, string hostId, string standartHostId)
        {
            string[] standartFileExcept = standartFileRow.Except(comparerFileRow).ToArray();
            string[] compareFileExcept = comparerFileRow.Except(standartFileRow).ToArray();

            if ((standartFileExcept.Length+compareFileExcept.Length)>_fileContext.MaxDifferense)
            {
                throw new OverflowMaxDiffExeption($"При сравнении хоста {hostId} обнаружено свыше {_fileContext.MaxDifferense} отклонений, детализация расхождений отменена");
            }

            List<PropItem> standartProps = ConvertRowsToProps(standartFileExcept);
            List<PropItem> comparerProps = ConvertRowsToProps(compareFileExcept);

            return GetCompareItems(standartProps, comparerProps, hostId, standartHostId);
        }

        private List<CompareItem> GetCompareItems(List<PropItem> standartProps, List<PropItem> comparerProps, string hostId, string standartHostId)
        {
            List<PropItem> standartItems = standartProps.Except(comparerProps).ToList();
            List<PropItem> comparerItems = comparerProps.Except(standartProps).ToList();

            List<CompareItem> comparerProp = CompareProps(standartItems, comparerItems, hostId, standartHostId);

            return comparerProp;
        }

        private List<CompareItem> CompareProps(List<PropItem> standartItems, List<PropItem> comparerItems, string hostId, string standarthostId)
        {
            DateTime dateCompare = DateTime.Now;
            List<CompareItem> resultItems = new List<CompareItem>();

            if (standartItems!=null)
            {
                foreach (var item in standartItems)
                {
                    var compareItem = new CompareItem
                    {
                        HostId = standarthostId,
                        HostStandartId = hostId,
                        MedId = item.MedId,
                        MedTitle = item.MedTitle,
                        PropTitle = item.PropTitle,
                        PropStandartStr = item.PropStr,
                        PropStandartVal = item.PropVal,
                        DateStandart = item.DateVal
                    };

                    var qwery = comparerItems
                        .Where(p => p.PropMedId == item.PropMedId)
                        .FirstOrDefault();

                    if (qwery!=null)
                    {
                        compareItem.PropComparetStr = qwery.PropStr;
                        compareItem.PropCompareVal = qwery.PropVal;
                        compareItem.DateCompare = qwery.DateVal;
                    }

                    resultItems.Add(compareItem);
                }
            }

            if (comparerItems != null)
            {
                foreach (var item in comparerItems)
                {
                   var compareItem = new CompareItem
                    {
                        HostId = hostId,
                        HostStandartId = standarthostId,
                        MedId = item.MedId,
                        MedTitle = item.MedTitle,
                        PropTitle = item.PropTitle,
                        PropComparetStr = item.PropStr,
                        PropCompareVal = item.PropVal,
                        DateCompare = item.DateVal
                    };

                    var qwery = standartItems
                        .Where(p => p.PropMedId == item.PropMedId)
                        .FirstOrDefault();

                    if (qwery != null)
                    {
                        compareItem.PropStandartStr = qwery.PropStr;
                        compareItem.PropStandartVal = qwery.PropVal;
                        compareItem.DateStandart = qwery.DateVal;
                    }

                    resultItems.Add(compareItem);
                }
            }

            var result = resultItems.Distinct().ToList();
            return result;
        }

        private List<PropItem> ConvertRowsToProps(string[] fileExcepts)
        {
            List<PropItem> propItems = new List<PropItem>();

            foreach (var except in fileExcepts)
            {
                string[] fields = except.Split(';');
                if (fields.Count() != 7)
                {
                    _parsingError++;
                }
                else
                {
                    DateTime dateVal = DateTime.MinValue;
                    if (!string.IsNullOrEmpty(fields[6]))
                    {
                        var isDateReadable = DateTime.TryParseExact(fields[6].Substring(0, 8), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateVal);
                        if (!isDateReadable)
                        {
                            _parsingError++;
                        }
                    }

                    var propItem = new PropItem
                    {
                        MedId = fields[0],
                        MedTitle = fields[1],
                        PropTitle = fields[2],
                        PropId = fields[3],
                        PropStr = fields[4],
                        PropVal = fields[5],
                    };

                    if (dateVal == DateTime.MinValue)
                    {
                        propItem.DateVal = "";
                    }
                    else
                    {
                        propItem.DateVal = dateVal.ToShortDateString();
                    }

                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(propItem.MedId);
                    stringBuilder.Append(propItem.PropId);
                    stringBuilder.Append(propItem.PropStr);
                    stringBuilder.Append(propItem.PropVal);
                    stringBuilder.Append(propItem.DateVal.ToString());
                    propItem.PropIndex = stringBuilder.ToString();
                    stringBuilder.Clear();
                    stringBuilder.Append(propItem.MedId);
                    stringBuilder.Append(propItem.PropId);
                    propItem.PropMedId = stringBuilder.ToString();
                    propItems.Add(propItem);
                }
            }

            return propItems;
        }
    }
}
