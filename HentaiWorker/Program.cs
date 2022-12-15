
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HentaiWorker.Actions;

namespace HentaiWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "int")
            {
                InteractiveMode();
                return;
            }

            var threads = new List<Thread>();
            for (var workerId = 1; workerId <= 4; workerId++)
            {
                var task1 = new Thread(() => WorkerMode(workerId));
                task1.Start();
                threads.Add(task1);
            }
            threads.ForEach(x=>x.Join());
        }

        public static void WorkerMode(int workerId)
        {
            while (true)
            {
                if (!TaskRunner.HasAnyTasks())
                {
                    Console.WriteLine($"({workerId}) No tasks to execute...");
                    Thread.Sleep(30000);
                    continue;
                }
                TaskRunner.RunTask(workerId);
            }
        }

        public static void InteractiveMode()
        {
            Console.WriteLine("Utility program:\n" +
                              "0) Exit\n" +
                              "1) Run image simplification\n" +
                              "2) Scan for duplicates\n");

            while (true)
            {
                var option = Console.ReadLine();
                switch (option)
                {
                    case "0":
                        return;
                    case "1":
                        ImageSimplificator.SimplifyImages();
                        break;
                    case "2":
                        DbScanner.FindDuplicates();
                        break;
                }
            }
        }
    }
}