using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GraduationProjectServices
{
    // Service to write logs.
    public static class LogService
    {
        private static readonly List<string> _notWrittenExceptions
            = new List<string>();

        private static object _lock = new object();

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
            lock (_lock)
            {
                try
                {
                    if (_notWrittenExceptions.Count > 0)
                    {
                        while (_notWrittenExceptions.Count > 0)
                        {
                            File.AppendAllText(path, _notWrittenExceptions.Last() + Environment.NewLine);
                            _notWrittenExceptions.Remove(_notWrittenExceptions.Last());
                        }
                    }

                    File.AppendAllText(path, res + Environment.NewLine);
                }
                catch
                {
                    _notWrittenExceptions.Add(res + Environment.NewLine);
                }
            }
        }
    }
}
