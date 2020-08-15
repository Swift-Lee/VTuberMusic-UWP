using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTuberMusic.Tools
{
    class Log
    {
        public static string LogText;

        public static void WriteLine(string value, string level = "Debug")
        {
            value = "[" + DateTime.Now.TimeOfDay.ToString() + "][" + level + "]" + value;
            LogText = LogText + value + "\r\n";
            Debug.WriteLine(value);
        }
    }

    public class Level
    {
        public static string Error = "Error";
        public static string Info = "Info";
        public static string DeBug = "DeBug";
    }
}
