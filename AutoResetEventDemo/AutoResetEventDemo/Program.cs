using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoResetEventDemo
{
    /// <summary>
    /// AutoRestEvent演示，waitOne()表示等待信号的设置，Set()表示释放信号，WatiOne才会继续执行下去。
    /// </summary>y
    class Program
    {
        const int numIterations = 50;
        static AutoResetEvent myResetEvent = new AutoResetEvent(false);
        static AutoResetEvent changeEvent = new AutoResetEvent(false);
        static int number;

        static void Main(string[] args)
        {
            Thread payMoneyThread = new Thread(new ThreadStart(PayMoneyTask));
            payMoneyThread.Name = "付款";
            Thread getBookThread = new Thread(new ThreadStart(GetBookTask));
            getBookThread.Name = "取书";
            payMoneyThread.Start();
            getBookThread.Start();
            for (int i = 1; i <= numIterations; i++)
            {
                Console.WriteLine($"Thread :买书,数量{i}");
                number = i;
                myResetEvent.Set();
                changeEvent.Set();
                Task.Delay(100).Wait();
            }
            payMoneyThread.Abort();
            getBookThread.Abort();
            Console.ReadKey();
        }

        /// <summary>
        /// 付款线程
        /// </summary>
        static void PayMoneyTask()
        {
            while (true)
            {
                myResetEvent.WaitOne();
                Console.WriteLine($"Thread :{Thread.CurrentThread.Name},数量{number}");
            }
        }

        static void GetBookTask()
        {
            while (true)
            {
                changeEvent.WaitOne();

                Console.WriteLine($"Thread :{Thread.CurrentThread.Name},数量{number}");
                Console.WriteLine($"##############################################");
                Task.Delay(100).Wait();
            }
        }

    }
}
