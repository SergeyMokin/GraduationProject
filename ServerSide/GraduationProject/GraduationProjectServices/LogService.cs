using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GraduationProjectServices
{
    // Service to write logs.
    public static class LogService
    {
        private static readonly List<string> NotWrittenExceptions
            = new List<string>();

        private static readonly object Lock = new object();

        public static void UpdateLogFile(Exception ex)
        {
            var date = DateTime.Now.ToString("dd/MM/yy HH:mm:ss");
            var methodPath = ex.TargetSite.DeclaringType.FullName;
            var res = date + " --- " + methodPath + " --- " + ex.Message;

            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                res += ex.Message;
            }

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                          "wwwroot/logs", DateTime.Now.ToString("dd_MM_yy") + "_log.txt");
            lock (Lock)
            {
                try
                {
                    if (NotWrittenExceptions.Any())
                    {
                        while (NotWrittenExceptions.Any())
                        {
                            File.AppendAllText(path, NotWrittenExceptions.Last() + Environment.NewLine);
                            NotWrittenExceptions.Remove(NotWrittenExceptions.Last());
                        }
                    }

                    File.AppendAllText(path, res + Environment.NewLine);
                }
                catch
                {
                    NotWrittenExceptions.Add(res + Environment.NewLine);
                }
            }
        }
    }
}
