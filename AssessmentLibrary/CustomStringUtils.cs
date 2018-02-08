using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentLibrary
{
    public class CustomStringUtils
    {
        // todo: this fails if there are redirects and we follow
        // need to fix...

        private static string GetLineContaining(string curlOutput, bool followRedirect, string searchItem)
        {
            string[] curlLines = curlOutput.Split(new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            //todo: make sure this is the one from curl and not text in body of response
            var linesQuery = from line in curlLines
                             where line.Contains(searchItem)
                             select line;

            string lineToParse = string.Empty;

            if (linesQuery.Count() == 0)
            {
                return string.Empty;
            }

            else if (followRedirect)
            {
                lineToParse = linesQuery.Last();
            }
            else
            {
                lineToParse = linesQuery.First();
            }

            return lineToParse;

        }

        public static string GetLocation(string curlOutput)
        {
            return GetLocation(curlOutput, false);
        }
        public static string GetLocation(string curlOutput, bool followRedirects)
        {
            string lineToParse = GetLineContaining(curlOutput, followRedirects, "Location:");
            try
            {
                return lineToParse.Split(' ')[1];
            }
            catch
            {
                return string.Empty;
            }
        }

        public static int GetHTTPCodeFromCurlOutput(string curlOutput)
        {
            return GetHTTPCodeFromCurlOutput(curlOutput, false);
        }

        public static int GetHTTPCodeFromCurlOutput(string curlOutput, bool followRedirects)
        {
            string lineToParse = GetLineContaining(curlOutput, followRedirects, "HTTP/1.1");
            try
            {
                return Convert.ToInt32(lineToParse.Split(' ')[1]);
            }
            catch
            {
                return -1;
            }
        }
    }
}
