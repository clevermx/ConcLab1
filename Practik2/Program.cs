using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Practik2
{
    class Program
    {
        static bool[] IsComplex;
        static int[] SimpleBase;

        static int CurrentPrime;
        static int BaseLen;
        public static void SimpleErat(int pStop)
        {
            for (int i = 2; i <= Math.Sqrt(pStop); i++)
            {
                if (IsComplex[i])
                {
                    continue;
                }
                else
                {
                    for (int j = i * i; j <= pStop; j = j + i)
                    {
                        IsComplex[j] = true;
                    }
                }
            }
        }

        public static void CheckForExpand(Object param)
        {
            int[] arg = (int[])param; //  начало базы, конец базы , начало данных, конец данных
            for (int j = arg[0]; j < arg[1]; j++)
            {
                int temp = SimpleBase.ElementAt(j);
                for (int i = (arg[2] < temp * temp) ? temp * temp : (arg[2] % temp == 0) ? arg[2] : arg[2] + (temp - arg[2] % temp); i < arg[3]; i = i + temp)
                {
                    IsComplex[i] = true;
                }
            }
        }


        static int getCurrentIndex()
        {
            lock ("index")
            {
                return CurrentPrime++;
            }
        }
        public static void CheckWithPool(Object param)
        {
            ManualResetEvent ev = ((Object[])param)[0] as ManualResetEvent;
            Object[] arg = (Object[])param;  //событие, начало данных, конец данных
            int bottom = (int)arg[1];  
            int top = (int)arg[2];
            while (true)
            {
                int Index = getCurrentIndex();

                if (Index > BaseLen-1)
                {
                    break;
                }

                int temp = SimpleBase[Index];
              
                for (int i = (bottom < temp * temp) ? temp * temp : (bottom % temp == 0) ? bottom : bottom + (temp - bottom % temp); i < top; i = i + temp)
                {
                    IsComplex[i] = true;
                }
            }
            ev.Set();

        }
        static void Main(string[] args)
        {
            int n;
            for (n = 5000000; n <= 5.5*Math.Pow(10, 8); n = n +10000000)
            {
                Console.WriteLine("*********************");
                int SqrtN = (int)Math.Sqrt(n);
                int[] arg;
                int SimpleCount = 0;

                System.Diagnostics.Stopwatch MyStopWatch = new System.Diagnostics.Stopwatch();
                MyStopWatch.Start();
                for (int i = 0; i < 10; i++)
                {
                    IsComplex = new bool[n];
                    IsComplex[0] = true;
                    IsComplex[1] = true;
                    SimpleBase = new int[n / 2];
                    SimpleErat(SqrtN);
                    int c = 0;
                    for (int k = 2; k <= SqrtN; k++)
                    {
                        if (!IsComplex[k])
                        {
                            SimpleBase[c] = k;
                            c++;
                        }
                    }
                    arg = new int[4];
                    arg[0] = 0;
                    arg[1] = c;
                    arg[2] = SqrtN;
                    arg[3] = n;

                    CheckForExpand(arg);
                }
                MyStopWatch.Stop();
                for (int i = 0; i < IsComplex.Length; i++)
                {
                    if (!IsComplex[i])
                    {
                        SimpleCount++;
                    }
                }
                Console.WriteLine(1 + " " + n + " " + MyStopWatch.ElapsedMilliseconds / 10 + " " + SimpleCount);


                Thread[] ThrArr;
                //Декомпозиция поданным
                for (int ThrCount = 2; ThrCount < 6; ThrCount++)
                {
                   
                   MyStopWatch = new System.Diagnostics.Stopwatch();
                    MyStopWatch.Start();

                    for (int i = 0; i < 10; i++)
                    {
                        SimpleBase = null;
                        IsComplex = null;
                        IsComplex = new bool[n];
                        IsComplex[0] = true;
                        IsComplex[1] = true;
                        SimpleBase = new int[n / 2];
                        SimpleErat(SqrtN);

                        ThrArr = new Thread[ThrCount];
                        int c = 0;
                        for (int k = 2; k <= SqrtN; k++)
                        {
                            if (!IsComplex[k])
                            {
                                SimpleBase[c] = k;
                                c++;
                            }
                        }
                        for (int ThrId = 0; ThrId < ThrCount - 1; ThrId++)
                        {
                            ThrArr[ThrId] = new Thread(CheckForExpand);
                            arg = new int[4];
                            arg[0] = 0;
                            arg[1] = c;
                            arg[2] = SqrtN + (ThrId * ((n - SqrtN) / ThrCount));
                            arg[3] = (SqrtN + (ThrId + 1) * ((n - SqrtN) / ThrCount));
                            ThrArr[ThrId].Start(arg);
                        }
                        ThrArr[ThrCount - 1] = new Thread(CheckForExpand);
                        arg = new int[4];
                        arg[0] = 0;
                        arg[1] = c;
                        arg[2] = SqrtN + (ThrCount - 1) * ((n - SqrtN) / ThrCount);
                        arg[3] = n;
                        ThrArr[ThrCount - 1].Start(arg);
                        for (int ThrId = 0; ThrId < ThrCount; ThrId++)
                        {
                            ThrArr[ThrId].Join();
                        }
                    }
                    MyStopWatch.Stop();
                    SimpleCount = 0;
                    for (int i = 0; i < IsComplex.Length; i++)
                    {
                        if (!IsComplex[i])
                        {
                            SimpleCount++;
                        }
                    }
                    Console.WriteLine(ThrCount + " по данным " + n + " " + MyStopWatch.ElapsedMilliseconds / 10 + " " + SimpleCount);
                }
                SimpleBase = null;
                IsComplex = null;
                //Декомпозиция набора простых чисел
                for (int ThrCount = 2; ThrCount < 6; ThrCount++)
                {
             
                    MyStopWatch = new System.Diagnostics.Stopwatch();
                    MyStopWatch.Start();

                    for (int i = 0; i < 10; i++)
                    {
                        SimpleBase = null;
                        IsComplex = null;
                        IsComplex = new bool[n];
                        IsComplex[0] = true;
                        IsComplex[1] = true;
                        SimpleBase = new int[n / 2];
                        SimpleErat(SqrtN);

                        ThrArr = new Thread[ThrCount];
                        int c = 0;
                        for (int k = 2; k <= SqrtN; k++)
                        {
                            if (!IsComplex[k])
                            {
                                SimpleBase[c] = k;
                                c++;
                            }
                        }
                        for (int ThrId = 0; ThrId < ThrCount - 1; ThrId++)
                        {
                            ThrArr[ThrId] = new Thread(CheckForExpand);
                            arg = new int[4];
                            arg[0] = (ThrId * ((c) / ThrCount));
                            arg[1] = ((ThrId + 1) * ((c) / ThrCount));
                            arg[2] = SqrtN;
                            arg[3] = n;
                            ThrArr[ThrId].Start(arg);
                        }
                        ThrArr[ThrCount - 1] = new Thread(CheckForExpand);
                        arg = new int[4];
                        arg[0] = ((ThrCount - 1) * ((c) / ThrCount));
                        arg[1] = c;
                        arg[2] = SqrtN;
                        arg[3] = n;
                        ThrArr[ThrCount - 1].Start(arg);
                        for (int ThrId = 0; ThrId < ThrCount; ThrId++)
                        {
                            ThrArr[ThrId].Join();
                        }
                    }
                    MyStopWatch.Stop();
                    SimpleCount = 0;
                    for (int i = 0; i < IsComplex.Length; i++)
                    {
                        if (!IsComplex[i])
                        {
                            SimpleCount++;
                        }
                    }
                    Console.WriteLine(ThrCount + " по базе " + n + " " + MyStopWatch.ElapsedMilliseconds / 10 + " " + SimpleCount);
                }


                SimpleBase = null;
                IsComplex = null;

                //Пулпотоков алг4
                for (int ThrCount = 2; ThrCount < 6; ThrCount++)
                {

                    MyStopWatch = new System.Diagnostics.Stopwatch();
                    MyStopWatch.Start();
                 
                    for (int i = 0; i < 10; i++)
                    {
                         SimpleBase = null;
                        IsComplex = null;
                        CurrentPrime = 0;
                        IsComplex = new bool[n];
                        IsComplex[0] = true;
                        IsComplex[1] = true;
                        SimpleBase = new int[n / 2];
                        SimpleErat(SqrtN);

                        ThrArr = new Thread[ThrCount];
                        int c = 0;
                        BaseLen = 0;
                        for (int k = 2; k <= SqrtN; k++)
                        {
                            if (!IsComplex[k])
                            {
                                SimpleBase[c] = k;
                                BaseLen++;
                                c++;
                            }
                        }


                        ManualResetEvent[] events = new ManualResetEvent[ThrCount];
                        for (int h = 0; h < ThrCount; h++)
                        {
                            events[h] = new ManualResetEvent(false);

                            Object[] param = new Object[3];
                            param[0] = events[h];
                            param[1] = SqrtN;
                            param[2] = n;
                            ThreadPool.QueueUserWorkItem(CheckWithPool, param);
                        }
                        WaitHandle.WaitAll(events);

                    }
                    MyStopWatch.Stop();
                    SimpleCount = 0;
                    for (int i = 0; i < IsComplex.Length; i++)
                    {
                        if (!IsComplex[i])
                        {
                            SimpleCount++;
                        }
                    }
                    Console.WriteLine(ThrCount + " пул потоков " + n + " " + MyStopWatch.ElapsedMilliseconds / 10 + " " + SimpleCount);
                }


            }




            Console.ReadLine();
        }
    }
}
