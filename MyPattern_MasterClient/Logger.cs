using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLogger
{
    class Logger
    {
        static object obj = new object();
      
        public static void Info(string message)
        {
            RecordEntry("INFO", message);
        }
        public static void Error(string message)
        {
            RecordEntry("ERROR", message);
        }
        private static void RecordEntry(string fileEvent, string EventData)
        {
            lock (obj)
            {
                //using (StreamWriter writer = new StreamWriter(@"/home/" + "RabitMQ_MasterClientlog.txt", true))
                using (StreamWriter writer = new StreamWriter(Environment.CurrentDirectory + "RabitMQ_MasterClientlog.txt", true))
                {
                    writer.WriteLine(String.Format($"{DateTime.UtcNow.ToString("dd/MM/yyyy hh:mm:ss")} : [{fileEvent}] - {EventData}"));
                    writer.Flush();
                }
            }
        }
    }
}
