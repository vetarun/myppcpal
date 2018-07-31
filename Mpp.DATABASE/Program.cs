using DbUp;
using DbUp.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mpp.DATABASE
{
    class Program
    {
        static int Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["MppConn"].ConnectionString;

            Func<string, bool> schemaFilterString = s => s.IndexOf("Schema") != -1 ? true : false;


            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), schemaFilterString)
                    .LogToConsole()
                    .WithTransaction()
                    .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                return -1;
            }

            Func<string, bool> spFilterString = s => s.IndexOf("StoredProcedures") != -1 ? true : false;
            Func<string, bool> vwFilterString = s => s.IndexOf("Views") != -1 ? true : false;
            Func<string, bool> tgFilterString = s => s.IndexOf("Triggers") != -1 ? true : false;
            Func<string, bool> fnFilterString = s => s.IndexOf("Functions") != -1 ? true : false;

            var schemaObjectRunner =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), fnFilterString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), vwFilterString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), spFilterString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(), tgFilterString)
                    .JournalTo(new NullJournal()) //Do not log that these items have been run. 
                    .LogToConsole()
                    .WithExecutionTimeout(TimeSpan.FromSeconds(180))
                    .WithTransaction()
                    .Build();

            var result2 = schemaObjectRunner.PerformUpgrade();


            if (!result2.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
                return -1;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            //return 0;
            return 0;

            
        }
    }
}
