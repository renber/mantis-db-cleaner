using MantisDBCleaner.Connection;
using MantisDBCleaner.Logging;
using MantisDBCleaner.Tasks;
using MantisDBCleaner.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace MantisDBCleaner
{
    class TaskExecutor
    {
        private Options Options { get; }
        private List<ITask> Tasks { get; }

        public TaskExecutor(Options options, List<ITask> tasks)
        {
            Options = options;
            Tasks = tasks;
        }

        public void Run()
        {
            using (MantisDatabase db = new MantisDatabase(Options))
            {
                TaskDataStore dataStore = new TaskDataStore();

                foreach (var task in Tasks)
                {
                    try
                    {
                        LogService.Info(task.Description + "...");
                        task.Run(db, Options, dataStore);
                    }
                    catch (TaskFailedException failedExc)
                    {
                        LogService.Error($"Task execution failed: {failedExc.Message}");
                        if (failedExc.ContinueExecution)
                        {
                            LogService.Info($"But execution can be continued");
                        }
                        else
                        {
                            throw new Exception("Unrecoverable error");
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Unrecoverable error: " + e.Message);
                    }
                }
            }
        }

    }
}
