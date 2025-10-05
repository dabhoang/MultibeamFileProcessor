// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using MultibeamFileProcessor;
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class Multibeam
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting File Watcher Multibeam");

        string inputDirectory = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Input" + Path.DirectorySeparatorChar;

        Console.WriteLine("Input Directory: " + inputDirectory);

        using var watcher = new FileSystemWatcher(@inputDirectory);

        watcher.NotifyFilter = NotifyFilters.Attributes
                             | NotifyFilters.CreationTime
                             | NotifyFilters.DirectoryName
                             | NotifyFilters.FileName
                             | NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Security
                             | NotifyFilters.Size;

        watcher.Created += OnCreated;
        watcher.Filter = "*.txt";
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;

        Console.WriteLine("Press enter to exit.");

        //unit tests
        if (args.Length > 0 && args[0].Equals("u"))
            UnitTest.RunTests();
        Console.ReadLine();
    }

    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        string value = $"Created: {e.FullPath}";
        Console.WriteLine(value);
        if (FileProcessor.IsFullyWritten(e.FullPath))
        {
            try
            {
                //checksum
                Console.WriteLine("File is fully written. Computing checksum.");
                byte[] checksum = FileProcessor.ComputeChecksum(e.FullPath);
                Console.WriteLine();

                //validate
                if (!FileProcessor.IsValid(checksum, Path.GetExtension(e.FullPath)))
                {
                    //validation failed, move to Failed
                    Console.WriteLine("Validation failed. Moving to Failed folder");
                    MoveFile(e.FullPath, "Failed");
                }
                else
                {
                    //validation succeeded, compress
                    Console.WriteLine("Validation succeeded. Compressing.");
                    FileProcessor.Compress(e.FullPath, Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Output");
                    MoveFile(e.FullPath, "Archive");
                }

                    

            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception occurred: " + ex.Message);
                //move to Failed
                MoveFile(e.FullPath, "Failed");
            }
        }
        else
        {
            Console.WriteLine("File not fully written.");
        }
        
        
    }

    private static void MoveFile(string inputFilePath,string destinationName)
    {
        string destinationPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + destinationName;
        if (!Directory.Exists(destinationPath))
        {
            Directory.CreateDirectory(destinationPath);
        }
        string destinationFullFilePath = destinationPath + Path.DirectorySeparatorChar + Path.GetFileName(inputFilePath);
        File.Move(inputFilePath, destinationFullFilePath);
        Console.WriteLine(inputFilePath + " moved to " + destinationFullFilePath);
    }

}
