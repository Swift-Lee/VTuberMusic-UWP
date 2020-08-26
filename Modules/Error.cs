using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTuberMusic.Tools;

namespace VTuberMusic.Modules
{
    class Error
    {
        public string ErrorCode { get; set; } = Lang.ReadLangText("ErrorCodeNotFound");
        public Type ReTryPage { get; set; }
        public object ReTryArgs { get; set; } = null;
        public bool CanBackHome { get; set; } = true;
    }
}
