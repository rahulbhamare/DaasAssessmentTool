using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentLibrary
{
    public class Utils
    {
        public static string GetInputFilePath(string fileName)
        {
            return string.Concat(Path.GetFullPath(@"..\..\..\"), fileName);
        }
    }
}
