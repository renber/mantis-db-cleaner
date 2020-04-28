using CommandLine;
using MantisDBCleaner.Logging;
using MantisDBCleaner.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace MantisDBCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            LogService.RegisterLogger(new ConsoleLogger());            

            //LogService.Info("MantisDBCleaner");
            //LogService.Info("Copyright © 2020 René Bergelt");

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(opts => RunWithOptions(opts));            
        }

        static void RunWithOptions(Options opts)
        {            
            LogService.RegisterLogger(new FileLogger($"{SanitizeFilename(opts.Database)}.clean.log"));

            var tasks = GetTasks(opts);

            try
            {
                var executor = new TaskExecutor(opts, tasks);
                executor.Run();

                LogService.Info("All tasks completed successfully");
            }
            catch (Exception exc)
            {
                LogService.Critical("Task execution failed: " + exc);
            }
        }

        /// <summary>
        /// Remove illegal path characters from s
        /// </summary>
        static string SanitizeFilename(string s)
        {
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                s = s.Replace(c.ToString(), "");
            }

            return s;
        }

        static List<ITask> GetTasks(Options opts)
        {
            List<ITask> tasks = new List<ITask>();
            tasks.Add(new ConnectionTestTask());
            tasks.Add(new PrepareCleanTask());            
            tasks.Add(new CleanProjectsTask());
            tasks.Add(new CleanBugsTask());
            tasks.Add(new CleanCategoriesTask());
            // Todo: CleanConfigTasks
            tasks.Add(new CleanCustomFieldsTask());
            tasks.Add(new CleanUsersTask());
            tasks.Add(new RemoveConfidentialDataTask());
            return tasks;
        }        
    }
}
