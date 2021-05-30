using DbUp;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;



namespace DbUpDemo
{
	class Program
	{
		static int Main(string[] args)
		{
			var configuration = new ConfigurationBuilder()
				.AddJsonFile("dev_settings.json")
				.AddEnvironmentVariables()
				.AddCommandLine(args)
				.Build();

			// Sets the connection string value from the command line or loaded from app settings
			var connectionString = args.FirstOrDefault() ?? configuration.GetConnectionString("DbUpSqlConnectionString");

            EnsureDatabase.For.SqlDatabase(connectionString);

            // Creates the DbUp builder, setting the connection string to use, scripts to apply, and to log output to the console. Can be configured as desired
            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();


            // Performs the upgrade as per the configuration and scripts loaded above
            var result = upgrader.PerformUpgrade();

            // Determine what to do if the result was unsuccessful
            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
#if DEBUG
                Console.ReadLine();
#endif
                return -1;
            }

            // Completed update successfully
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
            return 0;

        }
	}
}
