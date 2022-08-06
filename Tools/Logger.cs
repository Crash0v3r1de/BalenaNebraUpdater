using BalenaNebraUpdater.Objs.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalenaNebraUpdater.Tools
{
    public static class Logger
    {
        private static readonly string ErrorLog = Directory.GetCurrentDirectory() + "\\error.log";
        public static void SaveEntry(string msg,ErrorLevel err) {

            if (File.Exists(ErrorLog)) File.Delete(ErrorLog);
            using (var sw = new StreamWriter(ErrorLog))
            {
                sw.WriteLine($"{DateTime.Now} | {err.ToString().ToUpper()} - {msg}");
            }

        }
    }
}
