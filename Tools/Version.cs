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
        public static string GetBuild(DateTime time)
        {
            return time.ToString("yyyy.MM.dd.HH.mm.ss");
        }
    }
}
