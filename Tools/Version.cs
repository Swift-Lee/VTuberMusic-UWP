using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VTuberMusic.Tools
{
    class Version
    {
        public static string Build = "0000";
        public static string VersionNum = "v1.0";

        public static string GetBuild(DateTime time)
        {
            return time.ToString("yyyy.MM.dd.HH.mm.ss");
        }
    }
}
