using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Practik5
{
    class Program
    {
        static int N; // размер всего массива
        static int p; //число потоков
        static int[] mData;

        static int[] mLeftBorders;
        static int[] mBlockSizes;
        static int[] mLocalLiders; //лидеры в каждом из блоков, поток с номером i пишет в ячейки от i*p до i*(p+1)-1
        static int[] mFinalBorders;  // границы для окончательного разделения
        static int[][] mSubBlockLefts; //начала подблоков
        static int[][] mSubBlockSizes; //размеры подблоков
        static int[][] mMergeData; //массив для слияния
        static int[] mFinalSize;
        static System.Diagnostics.Stopwatch MyStopWatch;
        static public void startInitMnogo()
        {
            p = 4;
            mLeftBorders = new int[p];
            mBlockSizes = new int[p];
            mData = File.ReadAllText(@"E:/C#/in.txt").
            Split(new Char[] { ',', ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).
            Select(x => int.Parse(x)).ToArray();
            Console.WriteLine("Файл прочтен");
            N = mData.Length;
            for (int i = 0; i < p; i++)
            {
                mBlockSizes[i] = N / p;
                mLeftBorders[i] = i * N / p;
            }

            mLocalLiders = new int[p * p]; //каждый блок, разбиваем на p поменьше
            mFinalBorders = new int[p - 1]; //границ на 1 меньше чем кусков
            mSubBlockLefts = new int[p][];
            mSubBlockSizes = new int[p][]; //для каждого из потоков их p штук
            mFinalSize = new int[p];
            mMergeData = new int[p][];
            for (int i = 0; i < p; i++)
            {
                mSubBlockLefts[i] = new int[p];
                mSubBlockSizes[i] = new int[p];
            }
        }

        static public void startInitAdin()
        {
            p = 4;
            mData = File.ReadAllText(@"E:/C#/in.txt").
            Split(new Char[] { ',', ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).
            Select(x => int.Parse(x)).ToArray();
            Console.WriteLine("Файл прочтен");
            N = mData.Length;
        }

        public static void Swap(int[] pData, int i, int j)
        {
            int s = pData[i];
            pData[i] = pData[j];
            pData[j] = s;
        }
        public static void SerialQuickSort(int[] pData, int first, int last)
        {
            if (first >= last)
                return;
            int PivotPos = first;
            int Pivot = pData[first];
            for (int i = first + 1; i <= last; i++)
            {
                if (pData[i] < Pivot)
                {
                    if (i != PivotPos + 1)
                        Swap(pData, i, PivotPos + 1);
                    PivotPos++;
                }
            }
            Swap(pData, first, PivotPos);
            SerialQuickSort(pData, first, PivotPos - 1);
            SerialQuickSort(pData, PivotPos + 1, last);
        }
        public static void OneThreadWork(Object param)
        {
            Object[] arg = (Object[])param;
            int lCurThr = (int)arg[0];
            ManualResetEvent DoPause1 = (ManualResetEvent)arg[1];
            ManualResetEvent DoPause2 = (ManualResetEvent)arg[2];
            ManualResetEvent ResumeFlag1 = (ManualResetEvent)arg[3];
            ManualResetEvent ResumeFlag2 = (ManualResetEvent)arg[4];
            Array.Sort(mData, mLeftBorders[lCurThr], mBlockSizes[lCurThr]);
            for (int i = 0; i < p; i++)
            {
                mLocalLiders[p * lCurThr + i] = mData[mLeftBorders[lCurThr] + i * N / (p * p)];
            }

            DoPause1.Set();
            ResumeFlag1.WaitOne();

            mSubBlockLefts[lCurThr][0] = mLeftBorders[lCurThr];
            int Pos = 0, OldPos = 0;
            for (int i = 0; i < p - 1; i++)
            {
                OldPos = Pos;
                Pos = Array.BinarySearch(mData, mLeftBorders[lCurThr] + Pos, N / p - Pos, mFinalBorders[i]);
                if (Pos < 0)
                {
                    Pos = ~Pos;
                }
                Pos = Pos - mLeftBorders[lCurThr];

                mSubBlockLefts[lCurThr][i + 1] = mLeftBorders[lCurThr] + Pos;
                mSubBlockSizes[lCurThr][i] = Pos - OldPos;
            }
            mSubBlockSizes[lCurThr][p - 1] = N / p - Pos;

            DoPause2.Set();
            ResumeFlag2.WaitOne();

            //СЛИЯНИЕ
            int lDobav = 0;
            while (mFinalSize[lCurThr] > lDobav)
            {
                int lMinData = int.MaxValue; //большое число
                int lMinInd = 0;
                int lMinBlockNum = 0;
                for (int blockNum = 0; blockNum < p; blockNum++)
                {
                    if (mSubBlockSizes[blockNum][lCurThr] > 0)
                    {
                        if (mData[mSubBlockLefts[blockNum][lCurThr]] <= lMinData)
                        {
                            lMinData = mData[mSubBlockLefts[blockNum][lCurThr]];
                            lMinInd = mSubBlockLefts[blockNum][lCurThr];
                            lMinBlockNum = blockNum;
                        }
                    }
                }
                mMergeData[lCurThr][lDobav] = mData[lMinInd];
                mSubBlockSizes[lMinBlockNum][lCurThr]--;
                mSubBlockLefts[lMinBlockNum][lCurThr]++;
                lDobav++;
            }
            Console.WriteLine(lCurThr + " " + lDobav);
        }

        static void Main(string[] args)
        {
            
            if (String.Compare(args[0], "-S") == 0)
            {
                startInitAdin();
                //ОДИН ПОТОК 
                MyStopWatch = new System.Diagnostics.Stopwatch();
                MyStopWatch.Start();
                SerialQuickSort(mData, 0, mData.Length - 1);
                MyStopWatch.Stop();
                Console.WriteLine("N=" + N);
                Console.WriteLine("Однопоточная быстрая " + MyStopWatch.ElapsedMilliseconds);

              /* File.WriteAllText(@"E:/out.txt", "");//переписываем файл
                for (int i = 0; i < mData.Length; i++)
                {
                    File.AppendAllText(@"E:/C#/out.txt", (mData[i].ToString() + ",\n\r"));
                }
                Console.WriteLine("Файл out записан");*/
            }
            else if (String.Compare(args[0], "-P") == 0)
            {
                startInitMnogo();
                MyStopWatch = new System.Diagnostics.Stopwatch();
                MyStopWatch.Start();
                ManualResetEvent[] PauseEvent1 = new ManualResetEvent[p];
                ManualResetEvent ResumeEvent1 = new ManualResetEvent(false);
                ManualResetEvent[] PauseEvent2 = new ManualResetEvent[p];
                ManualResetEvent ResumeEvent2 = new ManualResetEvent(false);
                for (int i = 0; i < p; i++)
                {
                    PauseEvent1[i] = new ManualResetEvent(false);
                    PauseEvent2[i] = new ManualResetEvent(false);
                }
                Thread[] thrArr = new Thread[p];
                for (int i = 0; i < p; i++)
                {
                    thrArr[i] = new Thread(OneThreadWork);
                }
                for (int i = 0; i < p; i++)
                {
                    Object[] arg = new Object[5];
                    arg[0] = i;
                    arg[1] = PauseEvent1[i];
                    arg[2] = PauseEvent2[i];
                    arg[3] = ResumeEvent1;
                    arg[4] = ResumeEvent2;
                    thrArr[i].Start(arg);
                }
                WaitHandle.WaitAll(PauseEvent1);
                Array.Sort(mLocalLiders);
                // Global samples determination  
                for (int i = 1; i < p; i++)
                {
                    mFinalBorders[i - 1] = mLocalLiders[i * p];
                }
                ResumeEvent1.Set();
                WaitHandle.WaitAll(PauseEvent2);
                Console.WriteLine("**************************************");
                for (int i = 0; i < p; i++)
                {
                    for (int j = 0; j < p; j++)
                    {
                        mFinalSize[j] += mSubBlockSizes[i][j];
                    }
                }
                for (int i = 0; i < p; i++)
                {
                    mMergeData[i] = new int[mFinalSize[i]];
                }
                ResumeEvent2.Set();

                for (int i = 0; i < p; i++)
                {
                    thrArr[i].Join();
                }
             
                MyStopWatch.Stop();
                Console.WriteLine("N=" + N);
                Console.WriteLine("Многопоточная " + MyStopWatch.ElapsedMilliseconds);

            /*    File.WriteAllText(@"E:/out.txt", "");//переписываем файл
                for (int j = 0; j < p; j++)
                {
                    for (int i = 0; i < mMergeData[j].Length; i++)
                    {
                        File.AppendAllText(@"D:/C#/out.txt", (mMergeData[j][i].ToString() + ",\n\r"));
                    }
                }
                Console.WriteLine("Файл out записан");*/

            }


         
                Console.ReadLine();

        }
    }
}
