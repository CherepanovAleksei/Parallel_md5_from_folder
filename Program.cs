using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static object locker = new object();

    static string HashFromString(MD5 md5Hash, string input)
    {
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        StringBuilder sBuilder = new StringBuilder();

        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        return sBuilder.ToString();
    }

    static string HashFromFile(string filepath)
    {
        //Console.WriteLine("computing hash for: {0}", filepath);
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filepath))
            {
                var hash = md5.ComputeHash(stream);
                //Console.WriteLine("End of computing hash for: {0}", filepath);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    public static List<String> GetListFiles(string path)
    {
        try
        {
            //обход
            string[] dirs = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);
            List<string> result = new List<string>();
            foreach (string dir in dirs)
            {
                result.AddRange(GetListFiles(dir));
                //Console.WriteLine("Dir: {0} in folder: {1}", dir, path);
            }
            foreach (string file in files)
            {
                result.Add(file);
                //Console.WriteLine("File: {0} in folder: {1}", file, path);
            }
            return result;
        }
        catch (Exception)
        {
            //Console.WriteLine(e);
        }
        List<string> emptyList = new List<string>();
        return emptyList;
    }

    public static string ParallelHash(string path)
    {
        
        StringBuilder hash = new StringBuilder();
        List<string> listFiles = new List<string>(GetListFiles(path));

        int count = 0;
        Thread[] threads = new Thread[listFiles.Count];
        foreach (string file in listFiles)
        {
            threads[count] = new Thread(() =>
            {
                string fileHash = HashFromFile(file);
                lock (locker)
                {
                    hash.Append(fileHash);
                }
            });
            count++;
            //Console.WriteLine("Path: {0} Hash: {1}", dir, hash);
        }

        foreach (Thread thread in threads)
            thread.Start();
        foreach (Thread thread in threads)
            thread.Join();

        return HashFromString(MD5.Create(), hash.ToString());
    }

    static void Main(string[] args)
    {
        Input:
        Console.Write("Path of folder:");
        string path = Console.ReadLine();

        if (!Directory.Exists(path))
        {
            Console.WriteLine("Path is incorrect! Try again...");
            goto Input;
        }

        Stopwatch time = new Stopwatch();
        time.Start();
        Console.WriteLine(ParallelHash(path));

        time.Stop();
        Console.WriteLine(time.Elapsed);

        Console.WriteLine("Press any key to quit...");
        Console.ReadKey();
    }

    private static string SimpleHash(string path)
    {
        StringBuilder hash = new StringBuilder();
        List<string> listFiles = new List<string>(GetListFiles(path));
        foreach (string file in listFiles)
        {
            hash.Append(HashFromFile(file));
        }
        return HashFromString(MD5.Create(), hash.ToString());
    }
}
