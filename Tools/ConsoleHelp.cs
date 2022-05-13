using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BalenaNebraUpdater.Tools
{
    public class ConsoleHelp
    {
        public string PromptWebhook() {
            Console.Write("Enter Webhook URL: ");
            string raw = Console.ReadLine();
            Console.WriteLine("");
            return raw;
        }
        public string FleetName()
        {
            Console.Write("Enter fleet full name: ");
            string raw = Console.ReadLine();
            Console.WriteLine("");
            return raw;
        }
        public string BalenaPath()
        {
            Console.Write("Enter Client bin filder path: ");
            string raw = Console.ReadLine();
            Console.WriteLine("");
            return raw;
        }
    }
}
