using Mpp.BUSINESS;
using Mpp.BUSINESS.DataLibrary;
using Mpp.BUSINESS.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mpp.UTILITIES.Statics;

namespace Mpp.CONSOLE
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Job Process Has Started");
                JobScheduler_beta.start();

                Console.ReadKey();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception - {0}", ex.Message);
            }
        }
    }
}
