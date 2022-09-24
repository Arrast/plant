using System.IO;

namespace versoft.data_model
{
    public static class Const
    {
        public static string DataFileFolder = "DataFiles";
        public static string DataFileExtension = ".bytes";
        public static string ClassFileExtension = ".cs";
        public static string SpreadsheetPlayerPrefsKey = "spreadsheet_key";
        public static char DelimiterForArrays = ';';

        public static string[] SplitLineFromCSV(string line, char divider = '\t')
        {
            return line.Split(divider);
        }

        public static string GetDataFilePath()
        {
            return Path.Combine(UnityEngine.Application.streamingAssetsPath, DataFileFolder);
        }
    }
}