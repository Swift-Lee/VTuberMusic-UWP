using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;

namespace VTuberMusic.Tools
{
    public static class Lang
    {
        public static string ReadLangText(string key)
        {
            var resourceLoader = ResourceLoader.GetForCurrentView();
            return resourceLoader.GetString(key);
        }
    }
}
