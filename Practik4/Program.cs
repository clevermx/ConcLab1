using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace Practick4
{
    class Program
    {
        static Dictionary<char, int> mLetterDic;
        static Dictionary<string, int> mWordDic;

        static ConcurrentDictionary<char, int> mLetterConcDic;
        static ConcurrentDictionary<string, int> mWordConcDic;
        static int mSentenceCount = 0;
        static int n = 25; //число файлов
        static string[] paths;
        static Queue<string> mLinesBuffer;
        static ConcurrentQueue<string> mLinesBufferConc;
        static StreamReader mCurStream;
        static int mCurFile;
        static Char[] separators = { ' ', ',', '-', '.', '!', '?', '\"', '\n', '\r' };
       static Char[] SentenceSeparators = { '.', '!', '?' };
        static Regex r = new Regex(@"\w+[\!\.\?]");
        public static void ReadAllTextFiles(string[] paths)
        {
            foreach (var path in paths)
            {
                Char[] separators = { ' ', ',', '-', '.', '!', '?', '\"', '\n', '\r' };
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string line;
                    
                    while ((line = sr.ReadLine()) != null)
                    {
                        String[] wordsArray = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        Char[] letterArray = line.ToCharArray();
                        foreach (var word in wordsArray)
                        {
                            String lowerWord = word.ToLower();
                            if (!mWordDic.ContainsKey(lowerWord))
                            {
                                mWordDic.Add(lowerWord, 1);
                            }
                            else
                            {
                                mWordDic[lowerWord] += 1;
                            }
                        }
                        foreach (var letter in letterArray)
                        {
                            if (!mLetterDic.ContainsKey(letter))
                            {
                                mLetterDic.Add(letter, 1);
                            }
                            else
                            {
                                mLetterDic[letter] += 1;
                            }
                        }



                    }

                }
                
                String fullText = File.ReadAllText(path, Encoding.GetEncoding(1251));
                
                MatchCollection matches = r.Matches(fullText);
                mSentenceCount += matches.Count;
               
                
              
            }
            Console.WriteLine("Alg0 letters: " + mLetterDic.Values.Sum());
            Console.WriteLine("Alg0 words: " + mWordDic.Values.Sum());
            Console.WriteLine("Alg0 sentences: " + mSentenceCount);
        }


        public static void Alg11Thread(Object param) // массив путей, левая граница, правая граница 
        {
            string[] paths = (string[]) ((Object[])param)[0];
            int left = (int)((Object[])param)[1];
            int right = (int)((Object[])param)[2];
            var dic = new Dictionary<string, int>();
            var LetterDic = new Dictionary<char, int>();
            int WordCount = 0;
            int sentenceCount = 0;
            int LetterCount = 0;
            Char[] separators = { ' ', ',', '-', '.', '!', '?', '\"', '\n', '\r' };
            for ( int pathNum=left; pathNum<right;pathNum++)
            {
                using (StreamReader sr = new StreamReader(paths[pathNum], System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        String[] wordsArray = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        Char[] letterArray = line.ToCharArray();

                        foreach (var word in wordsArray)
                        {
                           WordCount++;
                            String lowerWord = word.ToLower();
                            if (!dic.ContainsKey(lowerWord))
                            {
                                dic.Add(lowerWord, 1);
                            }
                            else
                            {
                                dic[lowerWord] += 1;
                            }
                        }
                        foreach (var letter in letterArray)
                        {
                            LetterCount++;
                            if (!LetterDic.ContainsKey(letter))
                            {
                                LetterDic.Add(letter, 1);
                            }
                            else
                            {
                                LetterDic[letter] += 1;
                            }
                        }
                    }

                }
                String fullText = File.ReadAllText(paths[pathNum], Encoding.GetEncoding(1251));

             
                MatchCollection matches = r.Matches(fullText);
                sentenceCount += matches.Count;
            }
            MergeLetterDic(LetterDic);
            MergeWordDic(dic);
            lock ("mergeSentence")
            {
                mSentenceCount += sentenceCount;
            }
        }
        public static void Alg11Conc(Object param) // массив путей, левая граница, правая граница 
        {
            string[] paths = (string[])((Object[])param)[0];
            int left = (int)((Object[])param)[1];
            int right = (int)((Object[])param)[2];
            var dic = new ConcurrentDictionary<string, int>();
            var LetterDic = new ConcurrentDictionary<char, int>();
            int WordCount = 0;
            int sentenceCount = 0;
            int LetterCount = 0;
            Char[] separators = { ' ', ',', '-', '.', '!', '?', '\"', '\n', '\r' };
            for (int pathNum = left; pathNum < right; pathNum++)
            {
                using (StreamReader sr = new StreamReader(paths[pathNum], System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        String[] wordsArray = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        Char[] letterArray = line.ToCharArray();

                        foreach (var word in wordsArray)
                        {
                            WordCount++;
                            String lowerWord = word.ToLower();
                            dic.AddOrUpdate(lowerWord, 1, (StrKey, IntVal) => IntVal + 1);
                        }
                        foreach (var letter in letterArray)
                        {
                            LetterCount++;
                            LetterDic.AddOrUpdate(letter, 1, (StrKey, IntVal) => IntVal + 1);
                        }
                    }

                }
                String fullText = File.ReadAllText(paths[pathNum], Encoding.GetEncoding(1251));


                MatchCollection matches = r.Matches(fullText);
                sentenceCount += matches.Count;
            }
            MergeLetterConc(LetterDic);
            MergeWordConc(dic);
            lock ("mergeSentence")
            {
                mSentenceCount += sentenceCount;
            }
        }
        public static void Alg12Thread(Object param) // массив путей, левая граница, правая граница 
        {
            string[] paths = (string[])((Object[])param)[0];
            int left = (int)((Object[])param)[1];
            int right = (int)((Object[])param)[2];
         
            
            for (int pathNum = left; pathNum < right; pathNum++)
            {
                using (StreamReader sr = new StreamReader(paths[pathNum], System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        String[] wordsArray = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        Char[] letterArray = line.ToCharArray();

                        foreach (var word in wordsArray)
                        {
                            
                            String lowerWord = word.ToLower();
                            if (!mWordDic.ContainsKey(lowerWord))
                            {
                                lock ("addword")
                                {

                                    if (!mWordDic.ContainsKey(lowerWord))
                                    {
                                        mWordDic.Add(lowerWord, 1);
                                    }
                                    else
                                    {
                                        mWordDic[lowerWord] += 1;
                                    }
                                    
                                }
                               
                            }
                            else
                            {
                                lock("addword")
                                { 
                                mWordDic[lowerWord] += 1;
                                }
                            }
                        }
                        foreach (var letter in letterArray)
                        {
                            
                            if (!mLetterDic.ContainsKey(letter))
                            {
                                lock ("addLetter")
                                {
                                    if (!mLetterDic.ContainsKey(letter))
                                    {
                                        mLetterDic.Add(letter, 1);
                                    }
                                    else
                                    {
                                        mLetterDic[letter] += 1;
                                    }
                                }
                               
                            }
                            else
                            {
                                lock ("addLetter")
                                {
                                    mLetterDic[letter] += 1;
                                }
                            }
                        }
                    }

                }
                String fullText = File.ReadAllText(paths[pathNum], Encoding.GetEncoding(1251));

                
                MatchCollection matches = r.Matches(fullText);
                lock ("plussentence")
                {
                    mSentenceCount += matches.Count;
                }
            }
           
        }
        public static void Alg12ThreadConc(Object param) // массив путей, левая граница, правая граница 
        {
            string[] paths = (string[])((Object[])param)[0];
            int left = (int)((Object[])param)[1];
            int right = (int)((Object[])param)[2];
            for (int pathNum = left; pathNum < right; pathNum++)
            {
                using (StreamReader sr = new StreamReader(paths[pathNum], System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        String[] wordsArray = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        Char[] letterArray = line.ToCharArray();

                        foreach (var word in wordsArray)
                        {

                            String lowerWord = word.ToLower();
                            mWordConcDic.AddOrUpdate(lowerWord, 1, (skey, intVal) => intVal + 1);
                        }
                        foreach (var letter in letterArray)
                        {
                            mLetterConcDic.AddOrUpdate(letter, 1, (ckey,intval)=>intval+1);
                        }
                    }

                }
                String fullText = File.ReadAllText(paths[pathNum], Encoding.GetEncoding(1251));


                MatchCollection matches = r.Matches(fullText);
                lock ("plussentence")
                {
                    mSentenceCount += matches.Count;
                }
            }

        }
        public static void Alg2ReadThread()
        { string line;
            while (mCurFile < n)
            {
                lock ("queue")
                {
                    if (mCurFile < n)
                    {
                        line = mCurStream.ReadLine();
                        if (line == null)
                        {
                            mCurFile++;
                            mCurStream.Dispose();
                            if (mCurFile < n)
                            {
                                mCurStream = new StreamReader(paths[mCurFile], System.Text.Encoding.Default);
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            mLinesBuffer.Enqueue(line);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
        public static void Alg2ReadThreadConc()
        {
            string line=null;
            while (mCurFile < n)
            {
              lock ("queue")
                {
                    if (mCurFile < n)
                    {
                        line = mCurStream.ReadLine();
                        if (line == null)
                        {
                            mCurFile++;
                            mCurStream.Dispose();
                            if (mCurFile < n)
                            {
                                mCurStream = new StreamReader(paths[mCurFile], System.Text.Encoding.Default);
                                line = null;
                                continue;
                            }
                            else
                            {
                                break;
                            }
                           
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                
                mLinesBufferConc.Enqueue(line);
                
                
            }
        }

        public static void Alg2ParseThread()
        {
            string line;
            while (true)
            {
                line = null;
                if (mLinesBuffer.Count > 0)
                {
                    lock ("queue")
                    {
                        if (mLinesBuffer.Count > 0)
                        {
                            line = mLinesBuffer.Dequeue();
                        }
                    }
                    if (line != null)
                    {
                        String[] wordsArray = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        Char[] letterArray = line.ToCharArray();

                        foreach (var word in wordsArray)
                        {

                            String lowerWord = word.ToLower();
                            if (!mWordDic.ContainsKey(lowerWord))
                            {
                                lock ("addword")
                                {

                                    if (!mWordDic.ContainsKey(lowerWord))
                                    {
                                        mWordDic.Add(lowerWord, 1);
                                    }
                                    else
                                    {
                                        mWordDic[lowerWord] += 1;
                                    }

                                }

                            }
                            else
                            {
                                lock ("addword")
                                {
                                    mWordDic[lowerWord] += 1;
                                }
                            }
                        }
                        foreach (var letter in letterArray)
                        {

                            if (!mLetterDic.ContainsKey(letter))
                            {
                                lock ("addLetter")
                                {
                                    if (!mLetterDic.ContainsKey(letter))
                                    {
                                        mLetterDic.Add(letter, 1);
                                    }
                                    else
                                    {
                                        mLetterDic[letter] += 1;
                                    }
                                }

                            }
                            else
                            {
                                lock ("addLetter")
                                {
                                    mLetterDic[letter] += 1;
                                }
                            }
                        }

                        
                        MatchCollection  matches= r.Matches(line);


                        lock ("addSentence")
                        {
                            mSentenceCount += matches.Count;
                        }
                        line = null;
                    }

                }
                else
                {
                    if (mCurFile >= n)
                    {
                        break;
                    }
                }
              
            }
        }
        public static void Alg2ParseThreadConc()
        {
            string line;
            while (true)
            {
                line = null;
                if (mLinesBufferConc.Count > 0)
                {
                    if (mLinesBufferConc.TryDequeue(out line))
                    {
                        String[] wordsArray = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                        Char[] letterArray = line.ToCharArray();

                        foreach (var word in wordsArray)
                        {
                            String lowerWord = word.ToLower();
                            mWordConcDic.AddOrUpdate(word, 1, (skey, intval) => intval + 1);
                        }
                        foreach (var letter in letterArray)
                        {
                            mLetterConcDic.AddOrUpdate(letter, 1, (skey, intval) => intval + 1);
                        }
                        MatchCollection matches = r.Matches(line);
                        lock ("addSentence")
                        {
                            mSentenceCount += matches.Count;
                        }
                        line = null;
                    }

                }
                else
                {
                    if (mCurFile>=n)
                    {
                        break;
                    }
                }
            }
        }
        public static void MergeWordDic(Dictionary<string, int> pWordDic)
        {
            foreach(var word in pWordDic)
            {
                if (!mWordDic.ContainsKey(word.Key))
                {
                    lock ("mergeWord")
                    {
                        mWordDic.Add(word.Key, word.Value);
                    }
                }
                else
                {
                    lock ("mergeWord")
                    {
                    mWordDic[word.Key] += word.Value;
                    }
                }
            }
        }

        public static void MergeWordConc(ConcurrentDictionary<string, int> pWordDic)
        {
            foreach (var word in pWordDic)
            {
                mWordConcDic.AddOrUpdate(word.Key, word.Value, (sKey, intVal) => intVal + word.Value);
            }
        }
        
        public static void MergeLetterDic(Dictionary<char, int> pLetterDic)
        {
            foreach (var letter in pLetterDic)
            {
                if (!mLetterDic.ContainsKey(letter.Key))
                {
                    lock ("mergeLetter")
                    {
                        mLetterDic.Add(letter.Key, letter.Value);
                    }
                }
                else
                {
                    lock ("mergeLetter")
                    {
                        mLetterDic[letter.Key] += letter.Value;
                    }
                }
            }
        }

        public static void MergeLetterConc(ConcurrentDictionary<char, int> pLetterDic)
        {
            foreach (var letter in pLetterDic)
            {
                mLetterConcDic.AddOrUpdate(letter.Key, letter.Value, (sKey, intVal) => intVal + letter.Value);
            }
        }
        public static void Alg11(int M)
        {
            Thread[] thrArr = new Thread[M];
         
            for (int i = 0; i < M ; i++){
                thrArr[i] = new Thread(Alg11Thread);
            }
            
            for (int i = 0; i < M; i++)
            {
                Object[] arg = new Object[3];
                arg[0] = paths;
                arg[1] = i * (n / M);
                arg[2] = i == (M - 1) ? n : (n / M) * (i + 1);
                thrArr[i].Start(arg);
            }
            for (int i = 0; i < M; i++)
            {
                thrArr[i].Join();
            }
            Console.WriteLine("Alg11 letters: " + mLetterDic.Values.Sum());
            Console.WriteLine("Alg11 words: " + mWordDic.Values.Sum());
            Console.WriteLine("Alg11 sentences: " + mSentenceCount);
        }
        public static void Alg11Conc(int M)
        {
            Thread[] thrArr = new Thread[M];

            for (int i = 0; i < M; i++)
            {
                thrArr[i] = new Thread(Alg11Conc);
            }

            for (int i = 0; i < M; i++)
            {
                Object[] arg = new Object[3];
                arg[0] = paths;
                arg[1] = i * (n / M);
                arg[2] = i == (M - 1) ? n : (n / M) * (i + 1);
                thrArr[i].Start(arg);
            }
            for (int i = 0; i < M; i++)
            {
                thrArr[i].Join();
            }
            Console.WriteLine("Alg11Conc letters: " + mLetterConcDic.Values.Sum());
            Console.WriteLine("Alg11Conc words: " + mWordConcDic.Values.Sum());
            Console.WriteLine("Alg11Conc sentences: " + mSentenceCount);
        }
        public static void Alg12(int M)
        {
            Thread[] thrArr = new Thread[M];

            for (int i = 0; i < M; i++)
            {
                thrArr[i] = new Thread(Alg12Thread);
            }

            for (int i = 0; i < M; i++)
            {
                Object[] arg = new Object[3];
                arg[0] = paths;
                arg[1] = i * (n / M);
                arg[2] = i == (M - 1) ? n : (n / M) * (i + 1);
                thrArr[i].Start(arg);
            }
            for (int i = 0; i < M; i++)
            {
                thrArr[i].Join();
            }
            Console.WriteLine("Alg12 letters: " + mLetterDic.Values.Sum());
            Console.WriteLine("Alg12 words: " + mWordDic.Values.Sum());
            Console.WriteLine("Alg12 sentences: " + mSentenceCount);
        }
        public static void Alg12Conc(int M)
        {
            Thread[] thrArr = new Thread[M];

            for (int i = 0; i < M; i++)
            {
                thrArr[i] = new Thread(Alg12ThreadConc);
            }

            for (int i = 0; i < M; i++)
            {
                Object[] arg = new Object[3];
                arg[0] = paths;
                arg[1] = i * (n / M);
                arg[2] = i == (M - 1) ? n : (n / M) * (i + 1);
                thrArr[i].Start(arg);
            }
            for (int i = 0; i < M; i++)
            {
                thrArr[i].Join();
            }
            Console.WriteLine("Alg12Conc letters: " + mLetterConcDic.Values.Sum());
            Console.WriteLine("Alg12Conc words: " + mWordConcDic.Values.Sum());
            Console.WriteLine("Alg12Conc sentences: " + mSentenceCount);
        }

        static void Alg2(int M1, int M2) // читатели, парсеры
        {
            mCurStream = new StreamReader(paths[0], System.Text.Encoding.Default);
            mLinesBuffer = new Queue<string>();
            Thread[] thrRead = new Thread[M1];
            Thread[] thrParse = new Thread[M2];
            for (int i = 0; i < M1; i++)
            {
                thrRead[i] = new Thread(Alg2ReadThread);
            }
            for (int i = 0; i < M2; i++)
            {
                thrParse[i] = new Thread(Alg2ParseThread);
            }
            for (int i = 0; i < M1; i++)
            {
                thrRead[i].Start();
            }
            for (int i = 0; i < M2; i++)
            {
                thrParse[i].Start();
            }

            for (int i = 0; i < M1; i++)
            {
                thrRead[i].Join();
            }
            for (int i = 0; i < M2; i++)
            {
                thrParse[i].Join();
            }
            Console.WriteLine("Alg2 letters: " + mLetterDic.Values.Sum());
            Console.WriteLine("Alg2 words: " + mWordDic.Values.Sum());
            Console.WriteLine("Alg2 sentences: " + mSentenceCount);
        }

        static void Alg2Conc(int M1, int M2) // читатели, парсеры
        {
            mCurFile = 0;
            mSentenceCount=0;
            mCurStream = new StreamReader(paths[0], System.Text.Encoding.Default);
            mLinesBufferConc = new ConcurrentQueue<string>();
            Thread[] thrRead = new Thread[M1];
            Thread[] thrParse = new Thread[M2];
            for (int i = 0; i < M1; i++)
            {
                thrRead[i] = new Thread(Alg2ReadThreadConc);
            }
            for (int i = 0; i < M2; i++)
            {
                thrParse[i] = new Thread(Alg2ParseThreadConc);
            }
            for (int i = 0; i < M1; i++)
            {
                thrRead[i].Start();
            }
            for (int i = 0; i < M2; i++)
            {
                thrParse[i].Start();
            }

            for (int i = 0; i < M1; i++)
            {
                thrRead[i].Join();
            }
            for (int i = 0; i < M2; i++)
            {
                thrParse[i].Join();
            }
            Console.WriteLine("Alg2Conc letters: " + mLetterConcDic.Values.Sum());
            Console.WriteLine("Alg2Conc words: " + mWordConcDic.Values.Sum());
            Console.WriteLine("Alg2Conc sentences: " + mSentenceCount);
        }

        static void Main(string[] args)
        {
             paths = new string[n];
       
            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = @"E:\temp\file" + i + ".txt";

            }
            mLetterDic = new Dictionary<char, int>();
            mWordDic = new Dictionary<string, int>();
            mSentenceCount = 0;
            Console.WriteLine("-****************************-*******************-********************-");
            System.Diagnostics.Stopwatch MyStopWatch = new System.Diagnostics.Stopwatch();
            MyStopWatch.Start();
            ReadAllTextFiles(paths);
            MyStopWatch.Stop();
            Console.WriteLine("time " + MyStopWatch.ElapsedMilliseconds);
            Console.WriteLine("-****************************-*******************-********************-");
            int M = 12;
            mLetterDic = new Dictionary<char, int>();
            mWordDic = new Dictionary<string, int>();
            mSentenceCount = 0;
            MyStopWatch = new System.Diagnostics.Stopwatch();
            MyStopWatch.Start();
            Alg11(M);
            MyStopWatch.Stop();
            Console.WriteLine("time " + MyStopWatch.ElapsedMilliseconds);
            Console.WriteLine("-****************************-********************-********************-");
            mLetterDic = new Dictionary<char, int>();
            mWordDic = new Dictionary<string, int>();
            mSentenceCount = 0;
            MyStopWatch = new System.Diagnostics.Stopwatch();
            MyStopWatch.Start();
            Alg12(M);
            MyStopWatch.Stop();
            Console.WriteLine("time " + MyStopWatch.ElapsedMilliseconds);
            Console.WriteLine("-****************************-********************-********************-");
            mLetterDic = new Dictionary<char, int>();
            mWordDic = new Dictionary<string, int>();
            mSentenceCount = 0;
            MyStopWatch = new System.Diagnostics.Stopwatch();
            MyStopWatch.Start();
            Alg2(M,M);
            MyStopWatch.Stop();
            Console.WriteLine("time " + MyStopWatch.ElapsedMilliseconds);
            Console.WriteLine("-****************************-********************-********************-");
            mLetterConcDic = new ConcurrentDictionary<char, int>();
            mWordConcDic = new ConcurrentDictionary<string, int>();
            mSentenceCount = 0;
            MyStopWatch = new System.Diagnostics.Stopwatch();
            MyStopWatch.Start();
            Alg11Conc(M);
            MyStopWatch.Stop();
            Console.WriteLine("time " + MyStopWatch.ElapsedMilliseconds);
            Console.WriteLine("-****************************-********************-********************-");
            mLetterConcDic = new ConcurrentDictionary<char, int>();
            mWordConcDic = new ConcurrentDictionary<string, int>();
            mSentenceCount = 0;
            MyStopWatch = new System.Diagnostics.Stopwatch();
            MyStopWatch.Start();
            Alg12Conc(M);
            MyStopWatch.Stop();
            Console.WriteLine("time " + MyStopWatch.ElapsedMilliseconds);
            Console.WriteLine("-****************************-********************-********************-");
            mLetterConcDic = new ConcurrentDictionary<char, int>();
            mWordConcDic = new ConcurrentDictionary<string, int>();
            mSentenceCount = 0;
            MyStopWatch = new System.Diagnostics.Stopwatch();
            MyStopWatch.Start();
            Alg2Conc(M, M);
            MyStopWatch.Stop();
            Console.WriteLine("time " + MyStopWatch.ElapsedMilliseconds);
            Console.WriteLine("-************конец****-");

            Console.ReadLine();
        }
    }
}
