using System;
using System.Threading;

namespace MyPattern_MasterClient
{
    class Program
    {
        static void Main(string[] args)
        {
            MasterClient masterClient = null;
            using (masterClient = new MasterClient())
            {
                while (true)
                {
                    Thread.Sleep(10000);
                    Console.WriteLine("service master client rabbitmq working...");
                }
            }
        }
    }
}
