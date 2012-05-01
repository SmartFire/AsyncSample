using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AsyncSample;

namespace AsyncExample
{
    class Program
    {

        static void Main(string[] args)
        {
            string[] files = { "test1", "test2", "test3" };
            Task[] tasks = new Task[5];
            for (int i = 0; i < 5; i++)
            {
                tasks[i] = new Task(i);
                tasks[i].MyTaskAsync(files);
                tasks[i].MyTaskCompleted += new EventHandler<MyTaskAsyncCompletedEventArgs>(task_MyTaskCompleted);
                tasks[i].MyTaskProgressChanged += new EventHandler<MyTaskProgressChangedEventArgs>(task_MyTaskProgressChanged);
                Thread.Sleep(300);
            }

            Thread.Sleep(1000);

            for (int i = 0; i < 5; i++)
            {
                tasks[i].CancelAsync();
            }

            Thread.Sleep(3000);
            Console.ReadLine();
        }

        static void task_MyTaskProgressChanged(object sender, MyTaskProgressChangedEventArgs e)
        {
            Console.WriteLine("[MyTask][{2}] Progress: {0} %, Current file: {1}", e.ProgressPercentage, e.CurrentFile, e.Id);
        }

        static void task_MyTaskCompleted(object sender, MyTaskAsyncCompletedEventArgs e)
        {
            Console.WriteLine("Main : task[{0}] completed :) ", e.Id);
        }
    }
}
