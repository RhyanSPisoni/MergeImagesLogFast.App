using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeImagesLogFast
{
    public static class GlobalVariables
    {
        public static string? pathIni { get; set; } = null;
        public static long CombineImgMaxHeight { get; set; } = 0;
        public static long CombineImgMaxWidth { get; set; } = 0;
        public static IEnumerable<string>? FilesImagens { get; set; }
    }
}
