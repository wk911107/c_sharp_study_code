using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoResetEventDemo
{
    ///// <summary>
    ///// AutoRestEvent演示，waitOne()表示等待信号的设置，Set()表示释放信号，WatiOne才会继续执行下去。
    ///// </summary>y
    //class Program
    //{
    //    const int numIterations = 50;
    //    static AutoResetEvent myResetEvent = new AutoResetEvent(false);
    //    static AutoResetEvent changeEvent = new AutoResetEvent(false);
    //    static int number;

    //    static void Main(string[] args)
    //    {
    //        Thread payMoneyThread = new Thread(new ThreadStart(PayMoneyTask));
    //        payMoneyThread.Name = "付款";
    //        Thread getBookThread = new Thread(new ThreadStart(GetBookTask));
    //        getBookThread.Name = "取书";
    //        payMoneyThread.Start();
    //        getBookThread.Start();
    //        for (int i = 1; i <= numIterations; i++)
    //        {
    //            Console.WriteLine($"Thread :买书,数量{i}");
    //            number = i;
    //            myResetEvent.Set();
    //            changeEvent.Set();
    //            Task.Delay(100).Wait();
    //        }
    //        payMoneyThread.Abort();
    //        getBookThread.Abort();
    //        Console.ReadKey();
    //    }

    //    /// <summary>
    //    /// 付款线程
    //    /// </summary>
    //    static void PayMoneyTask()
    //    {
    //        while (true)
    //        {
    //            myResetEvent.WaitOne();
    //            Console.WriteLine($"Thread :{Thread.CurrentThread.Name},数量{number}");
    //        }
    //    }

    //    static void GetBookTask()
    //    {
    //        while (true)
    //        {
    //            changeEvent.WaitOne();

    //            Console.WriteLine($"Thread :{Thread.CurrentThread.Name},数量{number}");
    //            Console.WriteLine($"##############################################");
    //            Task.Delay(100).Wait();
    //        }
    //    }

    //}

    class Program
    {
        private static AutoResetEvent event_1 = new AutoResetEvent(true);
        private static AutoResetEvent event_2 = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            Console.WriteLine("按Enter键来创建3个线程并开始它们\r\n" +
                              "这些线程在等待AutoResetEvent1释放信号\r\n" +
                              "由于AutoResetEvent1是以信号状态创建,所以第一个线程被释放.\r\n" +
                              "然后AutoResetEvent1会被置为非信号状态.");
            //等待按Enter键后开启线程
            Console.ReadLine();

            for (int i = 1; i < 4; i++)
            {
                Thread t = new Thread(ThreadProc);
                t.Name = $"Thread_{i}";
                t.Start();
            }
            Thread.Sleep(250);

            for (int i =0; i < 2; i++)
            {
                Console.WriteLine("Press Enter to release another thread.");
                Console.ReadLine();
                event_1.Set();
                Thread.Sleep(250);
            }

            Console.WriteLine("\r\nAll threads are now waiting on AutoResetEvent #2.");
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine("Press Enter to release a thread.");
                Console.ReadLine();
                event_2.Set();
                Thread.Sleep(250);
            }

            Console.ReadKey();

        }


        static void ThreadProc()
        {
            string name = Thread.CurrentThread.Name;

            Console.WriteLine($"{name} waits on AutoResetEvent #1.");
            event_1.WaitOne();
            Console.WriteLine($"{name} is released from AutoResetEvent #1.");

            Console.WriteLine($"{name} watis on AutoResetEvent #2.");
            event_2.WaitOne();
            Console.WriteLine($"{name} is released from AutoResetEvent #2.");

            Console.WriteLine($"{name} ends.");
        }

    }


}
