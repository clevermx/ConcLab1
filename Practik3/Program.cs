using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Practik3
{
    class Program
    {
        static string StrBuf;
        static bool finish;
        static bool bEmpty;
        static int mAllowRead;
        static int mAllowWrite;
        static int n; // число сообщений
        static int mMessageLen;
        static int mWriterCount;
        static int mReaderCount;

        static Semaphore mReadSem;
        static Semaphore mWriteSem;
       static public void ReadWork()
        {
            
            List<string> LocalBuf=new List<string> (n*mWriterCount);
            while (!finish)
            {
                if (!bEmpty)
                {
                    LocalBuf.Add( StrBuf+ " " +"R" + Thread.CurrentThread.ManagedThreadId);
                    bEmpty = true;
                }
            }

           // Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " считал " + LocalBuf.Count);
        }
        static public void InterlockedReadWork()
        {

            List<string> LocalBuf = new List<string>(n * mWriterCount);
           
            while (!finish)
            {
                if (1==Interlocked.CompareExchange( ref mAllowRead,0,1))
                {
                    LocalBuf.Add(StrBuf + " " + "R" + Thread.CurrentThread.ManagedThreadId);
                    Interlocked.CompareExchange(ref mAllowWrite, 1, 0);
                }
            }

           // Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " считал " + LocalBuf.Count);
        }
        static public void LockReadWork()
        {

            List<string> LocalBuf = new List<string>(n * mWriterCount);
            while (!finish)
            {
                if (!bEmpty)
                {
                    lock ("Читатель")
                    {
                        if (!bEmpty)
                        {
                            LocalBuf.Add(StrBuf + " " + "R" + Thread.CurrentThread.ManagedThreadId);
                            bEmpty = true;
                        }
                    }
                
                }
            }

         //   Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " считал " + LocalBuf.Count);
        }

        static public void AutoResetReadWork(Object pState) // empEv , fullEv
        {
            var lEmptyEv = ((Object[])pState)[0] as AutoResetEvent;
            var lFullEv = ((Object[])pState)[1] as AutoResetEvent;
            List<string> LocalBuf = new List<string>(n * mWriterCount);
            while (true)
            {
                lFullEv.WaitOne();
                if (finish) break;
                LocalBuf.Add(StrBuf + " " + "R" + Thread.CurrentThread.ManagedThreadId);
                lEmptyEv.Set();                
            }
            if (finish)
            {
                lFullEv.Set();                
               // Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " считал " + LocalBuf.Count);
            }
            
        }
        static public void SemaphoreReadWork() 
        {
            
            List<string> LocalBuf = new List<string>(n * mWriterCount);
            while (true)
            {
                mReadSem.WaitOne();
                if (finish) break;
                LocalBuf.Add(StrBuf + " " + "R" + Thread.CurrentThread.ManagedThreadId);
                mWriteSem.Release();
            }
            if (finish)
            {
                mReadSem.Release();
               // Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " считал " + LocalBuf.Count);
               
            }

        }
        static public void SemaphoreWriteWork()
        {
            
            string[] LocalBuf = new string[n];
            int lMessageCount = 0;
            for (int i = 0; i < n; i++)
            {
                LocalBuf[i] = new String(Convert.ToChar(65 + Thread.CurrentThread.ManagedThreadId), mMessageLen) + i;
            }
            while (lMessageCount < n)
            {
                mWriteSem.WaitOne();

                StrBuf = "W" + Thread.CurrentThread.ManagedThreadId + " " + LocalBuf[lMessageCount];
                lMessageCount++;
                mReadSem.Release();
            }
          //  Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " записал " + lMessageCount);
        }
        static public void AutoResetWriteWork(Object pState)
        {
            var lEmptyEv = ((Object[])pState)[0] as AutoResetEvent;
            var lFullEv = ((Object[])pState)[1] as AutoResetEvent;
            string[] LocalBuf = new string[n];
            int lMessageCount = 0;
            for (int i = 0; i < n; i++)
            {
                LocalBuf[i] = new String(Convert.ToChar(65 + Thread.CurrentThread.ManagedThreadId), mMessageLen) + i;
            }
            while (lMessageCount < n)
            {
                lEmptyEv.WaitOne();

                    StrBuf = "W" + Thread.CurrentThread.ManagedThreadId + " " + LocalBuf[lMessageCount];
                    lMessageCount++;
                lFullEv.Set();
            }
           // Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " записал " + lMessageCount);
        }
        static public void WriteWork()
        {
            string[] LocalBuf = new string[n];
            int lMessageCount = 0;
            for(int i=0; i<n; i++)
            {
                LocalBuf[i] =new String (Convert.ToChar(65 + Thread.CurrentThread.ManagedThreadId), mMessageLen) + i;
            }
            while (lMessageCount< n)
            {
                if (bEmpty)
                {
                    StrBuf = "W"+ Thread.CurrentThread.ManagedThreadId + " "+ LocalBuf[lMessageCount];
                    bEmpty = false;
                    lMessageCount++;
                }
            }
          //  Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " записал " + lMessageCount);


        }
        static public void InterlockedWriteWork()
        {
            string[] LocalBuf = new string[n];
            int lMessageCount = 0;
            for (int i = 0; i < n; i++)
            {
                LocalBuf[i] = new String(Convert.ToChar(65 + Thread.CurrentThread.ManagedThreadId), mMessageLen) + i;
            }
            while (lMessageCount < n)
            {
                if (1 == Interlocked.CompareExchange(ref mAllowWrite,0, 1))
                {
                    StrBuf= "W" + Thread.CurrentThread.ManagedThreadId + " " + LocalBuf[lMessageCount];
                    Interlocked.CompareExchange(ref mAllowRead, 1, 0);
                    lMessageCount++;
                }
            }
           // Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " записал " + lMessageCount);


        }
        static public void LockWriteWork()
        {
            string[] LocalBuf = new string[n];
            int lMessageCount = 0;
            for (int i = 0; i < n; i++)
            {
                LocalBuf[i] = new String(Convert.ToChar(65 + Thread.CurrentThread.ManagedThreadId), mMessageLen) + i;
            }
            while (lMessageCount < n)
            {
                if (bEmpty)
                {
                    lock("Писатель")
                    {
                        if (bEmpty)
                        {
                            StrBuf = "W" + Thread.CurrentThread.ManagedThreadId + " " + LocalBuf[lMessageCount];
                            bEmpty = false;
                        }
                    }
                    lMessageCount++;
                }
            }
           // Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " записал " + lMessageCount);


        }
        static void Main(string[] args)
        {
            n = 10000;
            mWriterCount = 3;
           mReaderCount = 3;
            mMessageLen = 10;


            Thread[] Readers = new Thread[mReaderCount];
            Thread[] Writers = new Thread[mWriterCount];

            // без синхронизации
            Console.WriteLine("============без синхронизации =================");
            System.Diagnostics.Stopwatch MyStopWatch = new System.Diagnostics.Stopwatch();
            MyStopWatch.Start();

            for (int j = 0; j < 5; j++)
            {
                finish = false;
                bEmpty = true;
                for (int i = 0; i < mReaderCount; i++)
                {
                    Readers[i] = new Thread(ReadWork);
                    Writers[i] = new Thread(WriteWork);
                }
                for (int i = 0; i < mReaderCount; i++)
                {
                    Readers[i].Start();
                    Writers[i].Start();
                }
                for (int i = 0; i < mWriterCount; i++)
                {
                    Writers[i].Join();
                }
                finish = true;

                for (int i = 0; i < mReaderCount; i++)
                {
                    Readers[i].Join();
                }
            }
                MyStopWatch.Stop();
                Console.WriteLine("без синх " + MyStopWatch.ElapsedMilliseconds/5);
                // используя lock
                Console.WriteLine("============lock =================");
                MyStopWatch = new System.Diagnostics.Stopwatch();
                MyStopWatch.Start();
            for (int j = 0; j < 5; j++)
            {
                finish = false;
                bEmpty = true;
                for (int i = 0; i < mReaderCount; i++)
                {
                    Readers[i] = new Thread(LockReadWork);
              
                    Writers[i] = new Thread(LockWriteWork);
                }
                for (int i = 0; i < mReaderCount; i++)
                {
                    Readers[i].Start();
                    Writers[i].Start();
                }
                for (int i = 0; i < mWriterCount; i++)
                {
                    Writers[i].Join();
                }
                finish = true;
                for (int i = 0; i < mReaderCount; i++)
                {
                    Readers[i].Join();
                }
            }
            MyStopWatch.Stop();
            Console.WriteLine("Lock " + MyStopWatch.ElapsedMilliseconds/5);
            //AutoResetEvent
            Console.WriteLine("============AutoResetEvent =================");
            MyStopWatch = new System.Diagnostics.Stopwatch();
            MyStopWatch.Start();
            for (int j = 0; j < 5; j++)
            {
                finish = false;
                AutoResetEvent[] EmpFulEvents = new AutoResetEvent[2];
                EmpFulEvents[0] = new AutoResetEvent(true);
                EmpFulEvents[1] = new AutoResetEvent(false);
                Object arg = EmpFulEvents;
                for (int i = 0; i < mReaderCount; i++)
                {
                    Readers[i] = new Thread(AutoResetReadWork);
               
                    Writers[i] = new Thread(AutoResetWriteWork);
                }
                for (int i = 0; i < mReaderCount; i++)
                {
                    Readers[i].Start(arg);
                    Writers[i].Start(arg);
                }
                for (int i = 0; i < mWriterCount; i++)
                {
                    Writers[i].Join();
                }
                finish = true;
                EmpFulEvents[1].Set();
                for (int i = 0; i < mReaderCount; i++)
                {
                    Readers[i].Join();
                }
            }
            MyStopWatch.Stop();
            Console.WriteLine("AutoRe " + MyStopWatch.ElapsedMilliseconds/5);

            //Semaphore
            Console.WriteLine("============Semaphore =================");
            MyStopWatch = new System.Diagnostics.Stopwatch();
            MyStopWatch.Start();
            for (int j = 0; j < 5; j++)
            {
                finish = false;

                mWriteSem = new Semaphore(1, mWriterCount);

                mReadSem = new Semaphore(0, mReaderCount);

                for (int i = 0; i < mReaderCount; i++)
                {
                    Readers[i] = new Thread(SemaphoreReadWork);
                
                    Writers[i] = new Thread(SemaphoreWriteWork);
                }
                for (int i = 0; i < mReaderCount; i++)
                {
                    Readers[i].Start();
                    Writers[i].Start();
                }
                for (int i = 0; i < mWriterCount; i++)
                {
                    Writers[i].Join();
                }
                finish = true;
                mReadSem.Release();
                for (int i = 0; i < mReaderCount; i++)
                {
                    Readers[i].Join();
                }
            }
            MyStopWatch.Stop();
            Console.WriteLine("Sem " + MyStopWatch.ElapsedMilliseconds/5);
            //interlocked
            Console.WriteLine("============Interlocked =================");
            MyStopWatch = new System.Diagnostics.Stopwatch();
            MyStopWatch.Start();
            for (int j = 0; j < 5; j++)
            {
                finish = false;
                mAllowRead = 0;
                mAllowWrite = 1;
                for (int i = 0; i < mReaderCount; i++)
                {
                    Readers[i] = new Thread(InterlockedReadWork);
              
                    Writers[i] = new Thread(InterlockedWriteWork);
                }
                for (int i = 0; i < mReaderCount; i++)
                {
                    Readers[i].Start();
                    Writers[i].Start();
                }

                for (int i = 0; i < mWriterCount; i++)
                {
                    Writers[i].Join();
                }
                finish = true;

                for (int i = 0; i < mReaderCount; i++)
                {
                    Readers[i].Join();
                }
            }
            MyStopWatch.Stop();
            Console.WriteLine("Interlocked " + MyStopWatch.ElapsedMilliseconds/5);
            Console.WriteLine("Конец работы алгоритма");
            Console.ReadLine();
        }
    }
}
