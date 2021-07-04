using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Globalization;
using ComparerNET.Exceptions;

namespace ComparerNET
{
    public class FileContext : IFileContext
    {
        //private string workDir = @"E:\Upload";
        //private readonly string tempDir = @"E:\Upload\Temp";
        //private readonly string standartFileName = @"E:\Upload\Props_SCO.csv";
        //private readonly string workDir = @"C:\utils\IADATA";
        //private readonly string tempDir = @"C:\utils\IADATA\Temp";
        //private readonly string standartFileName = @"C:\utils\FileComparsion\Props_SCO.csv";
        private readonly string workDir = @"\\Ma_ia_virtual\iadata";
        private readonly string tempDir = @"\\Ma_ia_virtual\iadata\Temp";
        private readonly string standartFileName = @"\\MA_IA_VIRTUAL\FileComparsion\Props_SCO.csv";
        private readonly string maxDifference = "10000";
        private readonly string actualDayNum = "2";
        private readonly string settingsFileName = "settings.json";
        private Dictionary<string, string> settings;

        public string _workDir;
        private string _tempDir;
        private string _standartFileName;
        private int _actualDayNum;

        public int MaxDifferense => int.Parse(settings["max-difference"]);

        public FileContext()
        {
            var settingsPath = Path.Combine(Directory.GetCurrentDirectory(), settingsFileName);

            if (!File.Exists(settingsPath))
            {
                PostInitialSettings();
            }

            Initializing();
        }

        public string[] GetFileRow(string fileName)
        {
            if (!IsFileActual(fileName))
            {
                throw new OldFileException();
            }

            return File
                .ReadAllLines(fileName, Encoding.Default)
                .Skip(1)
                .ToArray();
        }

        public string[] GetFilesToCompare()
        {
            var filesToCompare = Directory.GetFiles(tempDir, "Prop*.csv");
            var filesZip = Directory.GetFiles(workDir, "Prop*.zip", SearchOption.AllDirectories);

            string[] filesForUnpuck = GetFilesForUnpuck(filesToCompare,filesZip);
            Unpucker.UnzipList(filesForUnpuck, tempDir);

            return Directory.GetFiles(tempDir, "Prop*.csv");
        }

        public string GetHostId(string files)
        {
            var fileName = Path.GetFileNameWithoutExtension(files);
            int index = fileName.IndexOf('_');
            return Path.GetFileNameWithoutExtension(files).Substring(++index, fileName.Length-index);
        }

        public string GetStandartFile()
        {
            return _standartFileName;
        }

        public void WriteInfoLog(string log)
        {
            string logDir = GetLogDir();
            var logFileName = Path.Combine(logDir, $"{DateTime.Now.ToShortDateString()}_Info.log");

            using (StreamWriter streamWriter = new StreamWriter(logFileName, false))
            {
                streamWriter.Write(log);
            }
        }

        public void WriteCompareLog(string log)
        {
            var logDir = GetLogDir();
            var logFileName = Path.Combine(logDir, $"{DateTime.Now.ToShortDateString()}_Compare.csv");

            using (StreamWriter streamWriter = new StreamWriter(logFileName, false, Encoding.GetEncoding(1251)))
            {
                streamWriter.Write(log);
            }
        }

        private string GetLogDir()
        {
            var monthName = $"{DateTime.Now.Month.ToString()}.{DateTime.Now.Year.ToString()}";
            var logDir = Path.Combine(Directory.GetCurrentDirectory(), "Logs", monthName);
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            return logDir;
        }

        private void UnpuckFiles(string[] zipFiles)
        {
            Unpucker.UnzipList(zipFiles, tempDir);
        }

        private bool IsFileActual(string file)
        {
            return File.GetLastWriteTime(file).AddDays(2) > DateTime.Now;
        }

        private Dictionary<string, string> GetSettings()
        {
            using (StreamReader streamReader = new StreamReader(settingsFileName))
            {
                string settingsString = streamReader.ReadToEnd();
                using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(settingsString)))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Dictionary<string, string>));
                    settings = (Dictionary<string, string>)serializer.ReadObject(memoryStream);
                }
            }

            return settings;
        }

        private void PostSettings(Dictionary<string, string> settings)
        {
            using (StreamWriter streamWriter = new StreamWriter(settingsFileName))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Dictionary<string, string>));
                    serializer.WriteObject(memoryStream, settings);
                    memoryStream.Position = 0;
                    StreamReader streamReader = new StreamReader(memoryStream);

                    string json = streamReader.ReadToEnd();
                    streamWriter.Write(json);
                }
            }
        }

        private string[] GetFilesForUnpuck(string[] filesToCompare, string[] filesZip)
        {
            List<string> filesForUnpuck = new List<string>();
            foreach (var zip in filesZip)
            {
                var csv = Directory.GetFiles(tempDir, $"*{Path.GetFileNameWithoutExtension(zip)}*");
                if (!csv.Any())
                {
                    filesForUnpuck.Add(zip);
                }
                else
                {
                    var zipFileDate = File.GetLastWriteTime(zip);
                    var csvFileDate = File.GetLastWriteTime(csv.First());

                    if (zipFileDate > csvFileDate.AddHours(10))
                    {
                        filesForUnpuck.Add(zip);
                    }
                }
            }

            return filesForUnpuck.ToArray();
        }

        private void PostInitialSettings()
        {
            settings = new Dictionary<string, string>();
            settings["work-dir"] = workDir;
            settings["temp-dir"] = tempDir;
            settings["standart-file-name"] = standartFileName;
            settings["max-difference"] = maxDifference;
            settings["actual-day-num"] = actualDayNum;
            PostSettings(settings);
        }

        private void Initializing()
        {
            var settings = GetSettings();
            _workDir = settings["work-dir"];
            _tempDir = settings["temp-dir"];
            _standartFileName = settings["standart-file-name"];
            _actualDayNum = int.Parse(settings["actual-day-num"]);

            if (!Directory.Exists(_tempDir))
                Directory.CreateDirectory(_tempDir);

            Directory.Delete(_tempDir, true);
            Directory.CreateDirectory(_tempDir);
        }
    }
}
