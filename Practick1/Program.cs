using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Practick1
{
    class Program
    {


        static double[] Arr;
        static double[] ResArr;


        static void NeravTask(Object param)
        {
            int[] arg = (int[])param;

            for (int i = arg[0]; i < arg[1]; i++)
            {
                ResArr[i] = Arr[i];
                for (int k = 0; k < i; k++)
                {
                    ResArr[i] += Math.Pow(ResArr[i], 1.789);
                }
            }

        }

        static void RavnTask(Object param)
        {
            int[] arg = (int[])param;

            for (int i = arg[0]; i < arg[1]; i++)
            {
                ResArr[i] = Arr[i];
                for (int k = 0; k < arg[2]; k++)
                {
                    ResArr[i] += Math.Pow(ResArr[i], 1.789);
                }
            }
        }
        static void CircleTask(Object param)
        {
            int[] arg = (int[])param;

            for (int i = arg[0]; i < arg[1]; i = i + arg[2])
            {
                ResArr[i] = Arr[i];
                for (int k = 0; k < i; k++)
                {
                    ResArr[i] += Math.Pow(ResArr[i], 1.789);
                }
            }
        }


        static void Main(string[] args)
        {
            int N;

            int M = 1;
            int K = 1;
            int Povtorov = 8;
            Random Rand = new Random();


            int[] arg = new int[3];
            /*      for (K=1; K <= 11; K = K + 2)
                  {
                      Console.WriteLine("_____СЛОЖНОСТЬ К = " + K+" ______");
                      for (int i = 1; i < 9; i++)
                      {

                          M = 1;
                          N = (int)Math.Pow(10, i);
                          Arr = new double[N];
                          ResArr= new double[N];
                          for (int j = 0; j < N; j++)
                          {
                              Arr[j] = Rand.Next();
                          }

                          arg[0] = 0;
                          arg[1] = N;
                          arg[2] = 1;
                          //RavnTask(arg);
                          NeravTask(arg);
                           Console.WriteLine("Массив заполнен " + N + " элементами");
                          //однопоточная версия


                          System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
                          myStopwatch.Start();
                          for (int iter = 0; iter < Povtorov; iter++)
                          {
                              arg = new int[3];
                              arg[0] = 0;
                              arg[1] = N;
                              arg[2] = K;
                              //   RavnTask(arg);
                              NeravTask(arg);

                           }
                          myStopwatch.Stop();
                          Console.WriteLine(M + "   " + myStopwatch.ElapsedMilliseconds/ Povtorov);
                          //многопоточная версия
                           //Равномерная вычислительная сложность
                          for (M = 2; M <= 10; M++)
                          {

                              myStopwatch = new System.Diagnostics.Stopwatch();
                              myStopwatch.Start();
                              for (int iter = 0; iter < Povtorov; iter++)
                              {
                                  Thread[] lThrHold = new Thread[M];
                                  for (int k = 0; k < M; k++)
                                  {
                                      lThrHold[k] = new Thread(RavnTask);
                                      arg = new int[3];
                                      arg[0] = (N / M) * k;
                                      if ((N / M) * (k + 1) > N - 1)
                                      {
                                          arg[1] = N;
                                      }
                                      else
                                      {
                                          arg[1] = (N / M) * (k + 1);
                                      }
                                      arg[2] = K;
                                      lThrHold[k].Start(arg);
                                  }


                                  for (int h = 0; h < M; h++)
                                  {
                                      lThrHold[h].Join();
                                  }

                              }
                              myStopwatch.Stop();
                              Console.WriteLine(M + "   " + myStopwatch.ElapsedMilliseconds / Povtorov);

                          }


                          for (M = 2; M <= 10; M++)
                          {

                              myStopwatch.Start();
                              for (int iter = 0; iter < Povtorov; iter++)
                              {
                                  Thread[] lThrHold = new Thread[M];
                                  for (int k = 0; k < M; k++)
                                  {
                                      lThrHold[k] = new Thread(NeravTask);
                                      arg = new int[3];
                                      arg[0] = (N / M) * k;
                                      if ((N / M) * (k + 1) > N - 1)
                                      {
                                          arg[1] = N;
                                      }
                                      else
                                      {
                                          arg[1] = (N / M) * (k + 1);
                                      }
                                      arg[2] = K;
                                      lThrHold[k].Start(arg);
                                  }


                                  for (int h = 0; h < M; h++)
                                  {
                                      lThrHold[h].Join();
                                  }

                              }
                              myStopwatch.Stop();
                              Console.WriteLine(M + "   " + myStopwatch.ElapsedMilliseconds / Povtorov);

                          }
                      }
                      Console.WriteLine("Нажмите Enter для перехода к следующему этапу");





                  }*/
            //Круговое распределение
            for (int i = 1; i < 9; i++)
            {

                M = 1;
                N = (int)Math.Pow(10, i);
                Arr = new double[N];
                ResArr = new double[N];
                for (int j = 0; j < N; j++)
                {
                    Arr[j] = Rand.Next();
                }

                arg[0] = 0;
                arg[1] = N;
                arg[2] = 1;
                NeravTask(arg);
                Console.WriteLine("Массив заполнен " + N + " элементами");
                System.Diagnostics.Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
                for (M = 2; M <= 10; M++)
                {

                    myStopwatch = new System.Diagnostics.Stopwatch();
                    myStopwatch.Start();
                    for (int iter = 0; iter < Povtorov; iter++)
                    {
                        Thread[] lThrHold = new Thread[M];
                        for (int k = 0; k < M; k++)
                        {
                            lThrHold[k] = new Thread(CircleTask);
                            arg = new int[3];
                            arg[0] = 0;
                            arg[1] = N;
                            arg[2] = M;
                            lThrHold[k].Start(arg);
                        }


                        for (int h = 0; h < M; h++)
                        {
                            lThrHold[h].Join();
                        }

                    }
                    myStopwatch.Stop();
                    Console.WriteLine(M + "   " + myStopwatch.ElapsedMilliseconds / Povtorov);

                }
            }
                Console.WriteLine("===========================================================");
                Console.WriteLine("Конец работы программы");
                Console.WriteLine("===========================================================");
                Console.ReadLine();
            
        }
    }
}
