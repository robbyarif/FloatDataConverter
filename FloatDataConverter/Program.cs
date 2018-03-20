using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FloatDataConverter
{
    class Program
    {
        private static string toExtractPath = @"C:\Workspace\FloatData\to_be_extracted";
        private static string extractedPath = @"C:\Workspace\FloatData\extracted";

        /// <summary>
        /// Main Program Float Data Converter
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            while (true)
            {
                // Print welcome message
                var command = PrintWelcomeMessage();

                // Menu 1. Extract Files to Folder
                if (command == "1")
                {
                    BatchExtract();
                }
                // Menu 2. Batch Convert Files
                else if (command == "2")
                {
                    MapExtensions.ConvertData();
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter correct menu number.");
                }

            }
        }

        static string PrintWelcomeMessage()
        {
            Console.Clear();

            Console.WriteLine("Data Converter Program");
            Console.WriteLine("===================================");
            Console.WriteLine("1. Batch Extract .dat.gz Files");
            Console.WriteLine("2. Batch Convert .dat Files");
            Console.WriteLine("Pilih menu:");
            return Console.ReadLine();
        }

        static void BatchExtract()
        {
            Console.WriteLine("1. Batch Extract .dat.gz Files");
            Console.WriteLine("===================================");
            Console.WriteLine("Instruksi:");
            Console.WriteLine("1. Siapkan folder bernama {0} yang berisi file-file .dat.gz.", toExtractPath);
            Console.WriteLine("2. Program akan mengekstrak file-file ke folder {0}.", extractedPath);
            Console.WriteLine("Tekan enter untuk memulai!"); Console.ReadKey();
            ExtractFiles();
        }

        static void ExtractFiles()
        {
            // Check input folder exists
            if (!Directory.Exists(toExtractPath))
            {
                throw new DirectoryNotFoundException("");
            }

            // Delete output folder if exists
            if (Directory.Exists(extractedPath))
            {
                Directory.Delete(extractedPath);
            }
            Directory.CreateDirectory(extractedPath);

            // Decompress
            DirectoryInfo directorySelected = new DirectoryInfo(toExtractPath);
            foreach (FileInfo fileToDecompress in directorySelected.GetFiles("*.gz"))
            {
                Decompress(fileToDecompress, new DirectoryInfo(extractedPath));
            }
        }

        static void Decompress(FileInfo fileToDecompress, DirectoryInfo outputPath)
        {
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.Name;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                using (FileStream decompressedFileStream = File.Create(outputPath.FullName + "\\" + newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
                    }
                }
            }
        }
    }
}
