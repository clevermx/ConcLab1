using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Practik6
{
    class Program
    {
        public static int N;
               
        public static Dictionary<Tuple<int, int>,int> mEdges;
        public static int[] mTreeMarker;
        public static Dictionary<Tuple<int, int>, int> MST;
        public static Tuple< Tuple<int,int>,int>[] mMinEdges;
        public static Boolean finish;
        public static int[] curMarkers;


        public static void OneStep()
        {
        
            foreach (var edge in mEdges.Keys)
            {
                if (mTreeMarker[edge.Item1] != mTreeMarker[edge.Item2])
                {
                  
                    if ((mMinEdges[mTreeMarker[edge.Item1]]==null) ||(mEdges[edge] < mMinEdges[mTreeMarker[edge.Item1]].Item2))
                    {
                        mMinEdges[mTreeMarker[edge.Item1]] = new Tuple<Tuple<int, int>, int>(edge, mEdges[edge]);
                    }
                    if ((mMinEdges[mTreeMarker[edge.Item2]] == null) || (mEdges[edge] < mMinEdges[mTreeMarker[edge.Item2]].Item2))
                    {
                        mMinEdges[mTreeMarker[edge.Item2]] = new Tuple<Tuple<int, int>, int>(edge, mEdges[edge]);
                    }
                }
            }

            for (int c = 0; c < mMinEdges.Length;c++) {
              
                if (mMinEdges[c] != null)
                {
                    try
                    {
                        MST.Add(mMinEdges[c].Item1, mMinEdges[c].Item2);
                    }
                    catch (Exception ex)
                    {
                    }
                    int lTreeMarkerRight = mTreeMarker[mMinEdges[c].Item1.Item2];
                    int lTreeMarkerLeft = mTreeMarker[mMinEdges[c].Item1.Item1];
                    for (int v = 0; v < mTreeMarker.Length; v++)
                    {
                        if (mTreeMarker[v] == lTreeMarkerRight)
                        {
                            mTreeMarker[v]= lTreeMarkerLeft;
                        }
                    }
                }
            }
           
           
        }

        public static void Boruvka()
        {
            while (MST.Count < N - 1)
            {
                mMinEdges = new Tuple<Tuple<int, int>, int>[N];
                OneStep();
            }

        }

       
        public static void OneThreadWork(Object param)
        {
            int[] arg = (int[])param;
            int[] lTreeMarkerList= (curMarkers.Skip(arg[0]).Take(arg[1])).ToArray<int>();
            foreach (var lTreeMarker in lTreeMarkerList)
            {
                Dictionary<Tuple<int, int>, int> lEdges = (mEdges.Where(p =>
                                                                            ((mTreeMarker[p.Key.Item1] == lTreeMarker) && (mTreeMarker[p.Key.Item2] != lTreeMarker))
                                                                           || ((mTreeMarker[p.Key.Item2] == lTreeMarker) && (mTreeMarker[p.Key.Item1] != lTreeMarker))
                                                                        )
                                                           ).ToDictionary(k => k.Key, k => k.Value); 
                  Tuple <Tuple<int, int>, int> MinEdge = null;
                foreach (var edge in lEdges)
                {
                    if ((MinEdge == null) || (edge.Value < MinEdge.Item2))
                    {
                        MinEdge = new Tuple<Tuple<int, int>, int>(edge.Key, edge.Value);
                    }
                }
                mMinEdges[lTreeMarker] = MinEdge;
            }
        }

        public static void UpdateMarkers()
        {
            for (int c = 0; c < mMinEdges.Length; c++)
            {
                if (mMinEdges[c] != null)
                {
                    try
                    {
                        MST.Add(mMinEdges[c].Item1, mMinEdges[c].Item2);
                    }
                    catch (Exception ex)
                    {
                    }
                    int lTreeMarkerRight = mTreeMarker[mMinEdges[c].Item1.Item2];
                    int lTreeMarkerLeft = mTreeMarker[mMinEdges[c].Item1.Item1];
                    for (int v = 0; v < mTreeMarker.Length; v++)
                    {
                        if (mTreeMarker[v] == lTreeMarkerRight)
                        {
                            mTreeMarker[v] = lTreeMarkerLeft;
                        }
                    }
                }
            }
        }

        public static void ManyThreads()
        {
            int p = 4;
            Thread[] ThrArr = new Thread[p];
            while (MST.Count < N - 1)
            {

                for (int i = 0; i < p; i++)
                {
                    ThrArr[i] = new Thread(OneThreadWork);
                }
                curMarkers = mTreeMarker.Distinct().ToArray<int>();
                int[] arg = null;
                for (int i = 0; i < p - 1; i++)
                {
                    arg = new int[2];
                    arg[0] = N / p * i;
                    arg[1] = N / p;
                    ThrArr[i].Start(arg );
                
                }
                arg = new int[2];
                arg[0] = N / p * (p - 1);
                arg[1] = N - N / p * (p - 1);
                ThrArr[p - 1].Start(arg);
                for (int i = 0; i < p; i++)
                {
                    ThrArr[i].Join();
                }
                
                UpdateMarkers();
            }
        }

        public static void OneTaskWork(Object param)
        {
            Object[] AllArg = (Object[])param;
            ManualResetEvent ev = (AllArg[2] as ManualResetEvent);
            int[] arg = new int[2];
            arg[0] = (int)AllArg[0];
            arg[1] = (int)AllArg[1];
            int[] lTreeMarkerList = (curMarkers.Skip(arg[0]).Take(arg[1])).ToArray<int>();
            foreach (var lTreeMarker in lTreeMarkerList)
            {
                Dictionary<Tuple<int, int>, int> lEdges = (mEdges.Where(p =>
                                                                            ((mTreeMarker[p.Key.Item1] == lTreeMarker) && (mTreeMarker[p.Key.Item2] != lTreeMarker))
                                                                           || ((mTreeMarker[p.Key.Item2] == lTreeMarker) && (mTreeMarker[p.Key.Item1] != lTreeMarker))
                                                                        )
                                                           ).ToDictionary(k => k.Key, k => k.Value);
                Tuple<Tuple<int, int>, int> MinEdge = null;
                foreach (var edge in lEdges)
                {
                    if ((MinEdge == null) || (edge.Value < MinEdge.Item2))
                    {
                        MinEdge = new Tuple<Tuple<int, int>, int>(edge.Key, edge.Value);
                    }
                }
                mMinEdges[lTreeMarker] = MinEdge;
            }
            ev.Set();
        }

        public static void BoruvkaPool(int p)
        {
            while (MST.Count < N - 1)
            {
                curMarkers = mTreeMarker.Distinct().ToArray<int>();
                ManualResetEvent[] events = new ManualResetEvent[p];
                for (int i = 0; i < p; i++)
                {
                    events[i] = new ManualResetEvent(false);
                }
                Object[] arg = null;
                for (int i = 0; i < p - 1; i++)
                {
                    arg = new Object[3];
                    arg[0] = N / p * i;
                    arg[1] = N / p;
                    arg[2] = events[i];
                    ThreadPool.QueueUserWorkItem(OneTaskWork, arg);
                }
                arg = new Object[3];
                arg[0] = N / p * (p - 1);
                arg[1] = N - N / p * (p - 1);
                arg[2] = events[p-1];
                ThreadPool.QueueUserWorkItem(OneTaskWork, arg);
                WaitHandle.WaitAll(events);
                UpdateMarkers();
            }
        }
        static void Main(string[] args)
        {
            N = 100;
             mEdges= new Dictionary<Tuple<int, int>, int>();
            mTreeMarker = new int[N];
            MST= new Dictionary<Tuple<int, int>, int>();
            mMinEdges = new Tuple<Tuple<int, int>, int>[N];
            StreamReader sr = new StreamReader(@"E:/C#/graph0.txt");
            string line;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                string[] fields =line.Split(' ');
                mEdges.Add(new Tuple<int, int>(int.Parse(fields[0]), int.Parse(fields[1])), int.Parse(fields[2]));
            }
            sr.Close();
            Console.WriteLine("Файл прочтен");
            System.Diagnostics.Stopwatch MyStopWatch;
            MyStopWatch = new System.Diagnostics.Stopwatch();
            MyStopWatch.Start();
            for (int i = 0; i < N; i++)
            {
                mTreeMarker[i] = i;
            }
            BoruvkaPool(4);
            MyStopWatch.Stop();
            Console.WriteLine("Параллельная пул" + MyStopWatch.ElapsedMilliseconds);
            mTreeMarker = new int[N];
            MST = new Dictionary<Tuple<int, int>, int>();
            mMinEdges = new Tuple<Tuple<int, int>, int>[N];
            MyStopWatch = new System.Diagnostics.Stopwatch();
            MyStopWatch.Start();
            for (int i = 0; i < N; i++)
            {
                mTreeMarker[i] = i;
            }
            ManyThreads();
            MyStopWatch.Stop();
            Console.WriteLine("Параллельная обычная" + MyStopWatch.ElapsedMilliseconds);
            
            mTreeMarker = new int[N];
            MST = new Dictionary<Tuple<int, int>, int>();
            mMinEdges = new Tuple<Tuple<int, int>, int>[N];
            MyStopWatch = new System.Diagnostics.Stopwatch();
            MyStopWatch.Start();
            for (int i = 0; i < N; i++)
            {
                mTreeMarker[i] = i;
            }
            Boruvka();
            MyStopWatch.Stop();
            Console.WriteLine("Последовательная " + MyStopWatch.ElapsedMilliseconds);

            Console.WriteLine("**********************************************");
            Console.ReadLine();
        }
    }
}
