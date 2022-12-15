
using System;
using System.Linq;
using System.Threading;
using HentaiPages.Database;
using HentaiPages.Database.Tables;
using HentaiPages.Models.Enums;
using HentaiWorker.Actions;

namespace HentaiWorker
{
    public static class TaskRunner
    {
        private static Mutex mut = new Mutex();
        
        public static bool HasAnyTasks()
        {
            using var db = new HentaiDbContext();
            mut.WaitOne();
            var taskExists = db.Tasks.Any(x => !x.StartDate.HasValue);
            mut.ReleaseMutex();
            return taskExists;
        }

        public static void RunTask(int workerId)
        {
            using var db = new HentaiDbContext();
            WorkerTask? workerTask = null;
            mut.WaitOne();
            workerTask = db.Tasks
                .Where(x => !x.StartDate.HasValue)
                .OrderBy(x => x.PostDate)
                .FirstOrDefault();
            Console.WriteLine($"({workerId}) Starting task: {workerTask.Type}, {workerTask.WorkerTaskId}");
            var taskRunner = new DoTask(workerTask, db);
            workerTask.StartDate = DateTime.Now;
            db.SaveChanges();
            mut.ReleaseMutex();
            
            if (workerTask is null)
                return;
            
            try
            {
                switch (workerTask.Type)
                {
                    case TaskType.FindSimilar:
                        taskRunner.DoTask_FindSimilarity();
                        break;
                    default:
                        throw new NotSupportedException("Unknown task type");
                }
                
                workerTask.FinishDate = DateTime.Now;
            }
            catch (Exception e)
            {
                workerTask.ErrorMessage = e.Message;
            }
            finally
            {
                db.SaveChanges();
            }
        }
    }
}