using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CIAuth.Web.Helpers
{
    public static class Utils
    {
        public static void WriteLog(string logFile, string text)
        {
            StringBuilder message = new StringBuilder();
            message.AppendLine(DateTime.Now.ToString());
            message.AppendLine(text);
            message.AppendLine("=========================================");

            System.IO.File.AppendAllText(logFile, message.ToString());
        }
    }
}