using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;



namespace MultibeamFileProcessor
{
    public static class FileProcessor
    {
        public static bool IsFullyWritten(string fullPath)
        {
            if (!File.Exists(fullPath))
                return false;
            try
            {
                FileStream inputStream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                inputStream.Close();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static byte[] ComputeChecksum(string fullPath)
        {
            //https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.sha256?view=net-9.0
            FileInfo file = new FileInfo(fullPath);
            byte[] hashValue = {};
            using (SHA256 mySHA256 = SHA256.Create())
            {
                using (FileStream fileStream = file.Open(FileMode.Open))
                {
                    try
                    {
                        fileStream.Position = 0;
                        // Compute the hash of the fileStream.
                        hashValue = mySHA256.ComputeHash(fileStream);
                        Console.WriteLine("Checksum computed: ");
                        PrintByteArray(hashValue);

                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Exception occurred: " + ex.Message);
                    }
                }
            }
            return hashValue;
        }

        public static void PrintByteArray(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.Write($"{array[i]:X2}");
                if ((i % 4) == 3) Console.Write(" ");
            }
            Console.WriteLine();
        }

        public static bool IsValid(byte[] checksum, string extension)
        {
            Console.WriteLine("Beginning validation. Reading configurations from appsettings.json ");
            string appSettingsJson = "appsettings.json";
            string jsonString = File.ReadAllText(appSettingsJson);
            Configurations config = JsonSerializer.Deserialize<Configurations>(jsonString)!;
            Console.WriteLine("Configurations read: ");
            Console.WriteLine("MaxFileSizeMB: " + config.MaxFileSizeMB);
            Console.WriteLine("AllowedExtensions: ");
            foreach (var ext in config.AllowedExtensions)
                Console.WriteLine(ext);
            if (checksum.Length > config.MaxFileSizeMB * 1000000 || !Array.Exists(config.AllowedExtensions, ext => ext.Equals(extension)))
                return false;
            return true;
        }


        //example 4 https://learn.microsoft.com/en-us/dotnet/standard/io/how-to-compress-and-extract-files
        public static void Compress(string inputFilePath, string outputDirectory) { 
            FileInfo fileToCompress = new FileInfo(inputFilePath);
            

            using (FileStream originalFileStream = fileToCompress.OpenRead())
            {
                if ((File.GetAttributes(fileToCompress.FullName) &
                    FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                {
                    if (!Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }
                    string outputFilePath = outputDirectory + Path.DirectorySeparatorChar + Path.GetFileName(inputFilePath);
                    using (FileStream compressedFileStream = File.Create(outputFilePath + ".gz"))
                    {
                        using (GZipStream compressionStream = new GZipStream(compressedFileStream,
                            CompressionMode.Compress))
                        {
                            originalFileStream.CopyTo(compressionStream);
                        }
                    }
                    FileInfo info = new FileInfo(outputDirectory + Path.DirectorySeparatorChar + fileToCompress.Name + ".gz");
                    Console.WriteLine($"Compression finished.");
                }
            }

        }
    }
}
