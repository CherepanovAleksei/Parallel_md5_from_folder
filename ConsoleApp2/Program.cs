using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static string HashFromFile(string filepath)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filepath))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    public static string GetHash(string path)
    {
        StringBuilder hash = new StringBuilder();
        string[] files = Directory.GetFiles(path);
        int len = files.Length;     
        int i=0;
        Thread[] threads = new Thread[len];
        foreach (string file in files)
        {
            threads[i] = new Thread(() => {
                //lock
                hash.Append(HashFromFile(file));
                //unlock
                });
            i++;
            //Console.WriteLine("Path: {0} Hash: {1}", file, HashFromFile(file));
        }
        foreach (Thread thread in threads)
            thread.Start();
        foreach (Thread thread in threads)
            thread.Join();
        //md5(hash)
        return hash.ToString();
    }

    static void Main(string[] args)
    {
        Input:
        Console.Write("Path of foulder:");
        string path = Console.ReadLine();
        if(!Directory.Exists(path))
        {
            Console.WriteLine("Path is incorrect! Try again...");
            goto Input;
        }
        Console.WriteLine(GetHash(path));

        Console.WriteLine("Press any key to quit...");
        Console.ReadKey();
    }
}
